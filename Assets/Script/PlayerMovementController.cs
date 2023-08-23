using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

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

        // 開始時執行的方法
        void Start()
        {
            capsuleCollider = GetComponent<CapsuleCollider>();
            rb = GetComponent<Rigidbody>();
            cam = Camera.main;
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
            deltaVertical = Input.GetAxis("Vertical");
            deltaHorizontal = Input.GetAxis("Horizontal");

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
                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            }
        }

        // 檢測角色是否在地面上的方法
        public bool IsGrounded()
        {
            RaycastHit hit;
            // 若在地面上有碰撞，則返回true
            if (Physics.Raycast(capsuleCollider.bounds.center, Vector3.down, capsuleCollider.bounds.extents.y + groundCheckDistance))
            {
                return true;
            }
            else { return false; }
        }

        // 角色開火的方法
        private void OnFire()
        {
            // 留空，以實現開火功能
        }
    }
}