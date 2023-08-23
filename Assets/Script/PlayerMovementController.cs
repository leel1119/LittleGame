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
        // Start is called before the first frame update
        void Start()
        {
            capsuleCollider = GetComponent<CapsuleCollider>(); 
            rb = GetComponent<Rigidbody>();
            cam = Camera.main;
        }

        private void FixedUpdate()
        {
            if (deltaPos.magnitude != 0) rb.MovePosition(transform.position + deltaPos);
            if (deltaRot.eulerAngles.magnitude != 0) rb.MoveRotation(deltaRot);
        }

        // Update is called once per frame
        void Update()
        {
            OnMove();
            OnRotation();
            OnJump();
            OnFire();
        }
        private void OnMove()
        {
            deltaVertical = Input.GetAxis("Vertical");
            deltaHorizontal = Input.GetAxis("Horizontal");

            Vector3 movement = new Vector3(deltaHorizontal, 0f, deltaVertical);
            Quaternion noTiltRotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);
            deltaPos = noTiltRotation * movement * MoveSpeed * Time.deltaTime;
        }
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
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            //Gizmos.DrawRay(ScreenPointToRay.origin, ScreenPointToRay.direction * 100f);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position+(Vector3.down*groundCheckDistance));
            //Gizmos.DrawSphere(GetPoint, .5f);
        }
        
        private void OnJump()
        {
            //print(IsGrounded());
            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            }
        }

        public bool IsGrounded()
        {
            RaycastHit hit;
            //if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance))
            if (Physics.Raycast(capsuleCollider.bounds.center, Vector3.down, capsuleCollider.bounds.extents.y + groundCheckDistance))
            {
                return true;
            }
            else { return false; }
        }
        /*float Raycast()
        {
            float number = 1;
            return number;
        }

        bool Raycast(out float rayLength)
        {
            float number = 1;
            rayLength = number;
            return false;
        }*/
        private void OnFire()
        {

        }
    }

}
