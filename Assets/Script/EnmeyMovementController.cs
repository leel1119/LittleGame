using UnityEngine;
using System;
using UnityEngine.AI;

namespace BasicCode
{
    public class EnemyMovementController : MonoBehaviour
    {
        [Header("移動參數")]
        [SerializeField] private float maxMoveDistance = 10f;
        [SerializeField] private float moveSpeed = 1.5f;

        public float MoveSpeed => moveSpeed;

        [SerializeField, Header("旋轉速度")]
        private float deltaVertical, deltaHorizontal;
        private Quaternion deltaRot;

        public float groundCheckDistance = 0.1f;
        private Rigidbody rb;
        public ForceMode forceMode = ForceMode.Force;
        CapsuleCollider capsuleCollider;

        public Vector3 DeltaPos => deltaPos;

        [Header("攻擊參數")]
        [SerializeField] private float closeAttackDistance = 2.0f;
        [SerializeField] private Vector2 FireRandomInterval = new Vector2(1f, 2f);
        [SerializeField] private float FireDamage = 1.0f;
        [SerializeField] private float FireForce = 1.0f;
        private float fireCooldown = 0f;

        private EventHandler<FireEventArgs> fireReceived;

        public event EventHandler<FireEventArgs> Fire1Received
        {
            add { fireReceived += value; }
            remove { fireReceived -= value; }
        }

        private Vector3 originPos;
        private Vector3 deltaPos;
        private EnemyHealth enemyHealth;
        private NavMeshAgent agent;
        private GameObject target;

        // 開始時執行的方法
        void Start()
        {
            originPos = transform.position;
            enemyHealth = GetComponent<EnemyHealth>();
            enemyHealth.DeadReceived += OnDead;

            agent = GetComponent<NavMeshAgent>();
            agent.speed = moveSpeed;
            agent.stoppingDistance = closeAttackDistance;

            fireCooldown = UnityEngine.Random.Range(FireRandomInterval.x, FireRandomInterval.y);

            capsuleCollider = GetComponent<CapsuleCollider>();
            rb = GetComponent<Rigidbody>();
            //cam = Camera.main;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            { 
                target = other.gameObject;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                target = other.gameObject;
                PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
                if(enemyHealth.IsDead) target =null;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                target = null;
            }
        }

        private void FixedUpdate()
        {
            // 若有位移，則在FixedUpdate中更新物體位置
            if (deltaPos.magnitude != 0) rb.MovePosition(transform.position + deltaPos);
            // 若有旋轉，則在FixedUpdate中更新物體旋轉
            //if (deltaRot.eulerAngles.magnitude != 0) rb.MoveRotation(deltaRot);
        }

        // 每幀執行的方法
        void Update()
        {
            if (target)
            {
                OnMove();
                OnFire();
            }
            else
            { 
                if(Vector3.Distance(transform.position, originPos) >= maxMoveDistance || target ==null) 
                {
                    agent.SetDestination(originPos);
                }
            }
        }

        // 角色移動的方法
        void OnMove()
        {
            agent.SetDestination(target.transform.position);
            deltaPos = agent.velocity;
        }

        private void OnDead(object sender, EventArgs e)
        {
            agent.isStopped = true;
            enabled = false;
        }

        // 角色開火的方法
        public void OnFire()
        {
            float distance = Vector3.Distance(target.transform.position, transform.position);
            if (distance <= closeAttackDistance)
            { 
                Quaternion targetRot = Quaternion.LookRotation(target.transform.position - agent.transform.position);
                targetRot = targetRot.normalized;
                targetRot = targetRot.normalized;
                targetRot.x = targetRot.z = 0;
                Quaternion deltaRot = Quaternion.Slerp(agent.transform.rotation, targetRot, agent.speed * Time.deltaTime);
                if (deltaRot.eulerAngles.magnitude != 0) agent.transform.rotation = targetRot;

                if (fireCooldown <= 0)
                {
                    FireEventArgs fireEventArgs = new FireEventArgs(FireDamage, FireForce);
                    fireReceived?.Invoke(this,fireEventArgs);
                    fireCooldown = UnityEngine.Random.Range(FireRandomInterval.x, FireRandomInterval.y);
                }
                if (fireCooldown > 0f)
                { 
                    fireCooldown -= Time.deltaTime;
                }
            }
        }
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
    }
}