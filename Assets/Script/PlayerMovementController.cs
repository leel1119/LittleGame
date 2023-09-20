using UnityEngine;
using System;

namespace BasicCode
{
    public class PlayerMovementController : MonoBehaviour
    {
        [Header("移動參數")]
        [SerializeField, Header("移動速度")]
        private float moveSpeed = 3f;

        public float MoveSpeed => moveSpeed;

        private Vector3 deltaPos;

        public Vector3 DeltaPos => deltaPos;

        [SerializeField, Header("旋轉速度")]
        private float RotationSpeed = 5f;
        private float deltaVertical, deltaHorizontal;
        private Quaternion deltaRot;

        [SerializeField, Header("跳躍力道")]
        private float jumpForce = 5f;

        public float groundCheckDistance = 0.1f;
        private Rigidbody rb;
        private Camera cam;
        private Vector3 GetPoint;
        private Ray ScreenPointToRay;
        public ForceMode forceMode = ForceMode.Force;
        CapsuleCollider capsuleCollider;

        private Vector2 deltaInput;
        public Vector2 DeltaInput => deltaInput;

        [Header("攻擊1參數")]
        [SerializeField] private float Fire1Interval = 0.1f;
        [SerializeField] private float Fire1Damage = 1.0f;
        [SerializeField] private float Fire1Force = 1.0f;
        private float fire1Cooldown;

        private EventHandler<FireEventArgs> fire1Received, fire2Received;
        private PlayerHealth playerHealth;

        public event EventHandler<FireEventArgs> Fire1Received
        {
            add { fire1Received += value; } 
            remove {  fire1Received -= value; }
        }


        private EventHandler jumpReceived;
        public event EventHandler JumpReceived
        {
            add { jumpReceived += value; }
            remove { jumpReceived -= value; }
        }

        // 開始時執行的方法
        void Start()
        {
            playerHealth = GetComponent<PlayerHealth>();
            playerHealth.DeadReceived += OnDead;
            playerHealth.RevivalReceived += OnRevival;
            
            capsuleCollider = GetComponent<CapsuleCollider>();
            rb = GetComponent<Rigidbody>();
            cam = Camera.main;
        }

        private void OnRevival(object sender, EventArgs e)
        {
            enabled = true;
        }

        private void OnDead(object sender, EventArgs e)
        { 
            enabled = false;
        }
        
        private void FixedUpdate()
        {
            // 若有位移，則在FixedUpdate中更新物體位置
            if (deltaPos.magnitude != 0) rb.MovePosition(transform.position + deltaPos);
            // 若有旋轉，則在FixedUpdate中更新物體旋轉
            if (deltaRot.eulerAngles.magnitude != 0) rb.MoveRotation(deltaRot);
        }

        // 每幀執行的方法
        void Update()
        {
            OnMove();
            OnRotation();
            OnJump();
            OnFire();
        }

        // 角色移動的方法
        private void OnMove()
        {
            deltaVertical = deltaInput.y = Input.GetAxis("Vertical");
            deltaHorizontal = deltaInput.x = Input.GetAxis("Horizontal");

            Vector3 movement = new Vector3(deltaHorizontal, 0f, deltaVertical);
            Quaternion noTiltRotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);
            deltaPos = noTiltRotation * movement * MoveSpeed * Time.deltaTime;
        }

        // 角色旋轉的方法
        private void OnRotation()
        {
            Ray camerRay = cam.ScreenPointToRay(Input.mousePosition);
            ScreenPointToRay = camerRay;
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayLength;
            if (groundPlane.Raycast(camerRay, out rayLength))
            {
                Vector3 targetPoint = camerRay.GetPoint(rayLength);
                GetPoint = targetPoint;
                Quaternion targetRot = Quaternion.LookRotation(targetPoint - transform.position);
                targetRot = targetRot.normalized;
                targetRot.x = targetRot.z = 0;
                deltaRot = Quaternion.Slerp(transform.rotation, targetRot, RotationSpeed * Time.deltaTime);
            }
        }

        // 繪製Gizmos以可視化射線
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            // Gizmos.DrawRay(ScreenPointToRay.origin, ScreenPointToRay.direction * 100f);
            Gizmos.color = Color.blue;
            // 在Scene視圖中繪製一條藍色的線，表示從物體位置向下延伸的射線，用於檢查地面
            Gizmos.DrawLine(transform.position, transform.position + (Vector3.down * groundCheckDistance));
            // Gizmos.DrawSphere(GetPoint, .5f);
        }

        // 角色跳躍的方法
        private void OnJump()
        {
            // 若按下跳躍按鈕且在地面上，則給予向上的力量實現跳躍
            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                jumpReceived?.Invoke(this, EventArgs.Empty);
                //if (jumpReceived != null)
                //{ 
                //    jumpReceived.Invoke(this, EventArgs.Empty);
                //}
                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            }
        }
        private void OnDestroy()
        {
            jumpReceived = null;
        }

        // 檢測角色是否在地面上的方法
        public bool IsGrounded()
        {
            //RaycastHit hit;
            // 若在地面上有碰撞，則返回true
            if (Physics.Raycast(capsuleCollider.bounds.center, Vector3.down, capsuleCollider.bounds.extents.y + groundCheckDistance))
            {
                return true;
            }
            else { return false; }
        }

        // 角色開火的方法
        public void OnFire()
        {
            if (Input.GetButtonDown("Fire1") && fire1Cooldown <= 0f)
            {
                FireEventArgs fireEventArgs = new FireEventArgs(Fire1Damage, Fire1Force);
                fire1Received?.Invoke(this, fireEventArgs);
                fire1Cooldown = Fire1Interval;
            }

            if (fire1Cooldown > 0f) { fire1Cooldown -= Time.deltaTime; }
        }
    }
}
//public class FireEventArgs : EventArgs
//{
//    public float FireDamage { get; private set; }
//    public float FireForce { get; private set; }

//    public FireEventArgs(float damageDate, float forceData)
//    {
//        FireDamage = damageDate;
//        FireForce = forceData;
//    }
//}