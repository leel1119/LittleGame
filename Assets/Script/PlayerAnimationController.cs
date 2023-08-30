using BasicCode;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace BasicCode
{
    public class PlayerAnimationController : MonoBehaviour
    {
        private PlayerMovementController playerMovementController;
        private Animator animator;
        private int velXHash, velYHash;
        private int jumpTriggerHash, isGroundedHash;
        private int attack1Index = 0, attack1MaxIndex = 2;
        private int attack1Hash, attack1IndexHash;
        private EventHandler startAnimReceived, endAnimReceived;
        public event EventHandler StartAnimReceived 
        {
            add { startAnimReceived += value; }
            remove { startAnimReceived -= value; }
        }
        public event EventHandler EndAnimReceived
        {
            add { endAnimReceived += value; }
            remove { endAnimReceived -= value; }
        }

        void StartAttackAnimation()
        {
            print("StartAttck");
            startAnimReceived?.Invoke(this, EventArgs.Empty);
        }
        void EndAttackAnimation()
        {
            print("EndAttck");
            endAnimReceived?.Invoke(this, EventArgs.Empty);
        }
        
        // Start is called before the first frame update
        void Start()
        {
            jumpTriggerHash = Animator.StringToHash("JumpTrigger");
            isGroundedHash = Animator.StringToHash("IsGrounded");
            playerMovementController = GetComponentInParent<PlayerMovementController>();
            playerMovementController.JumpReceived += OnJump;
            playerMovementController.Fire1Received += OnFire1;
            attack1Hash = Animator.StringToHash("AttackTrigger");
            attack1IndexHash = Animator.StringToHash("AttackIndex");
            animator = GetComponent<Animator>();
            velXHash = Animator.StringToHash("VelX");
            velYHash = Animator.StringToHash("VelY"); ;
        }

        public void OnFire1(object sender, FireEventArgs e)
        {
            attack1Index++;
            if(attack1Index > attack1MaxIndex) attack1Index = 0;
            animator.SetTrigger(attack1Hash);
            animator.SetInteger(attack1IndexHash, attack1Index);
        }

        // Update is called once per frame
        void Update()
        {
            OnMove();
            animator.SetBool(isGroundedHash, playerMovementController.IsGrounded());
        }
        void OnJump(object sender, EventArgs e)
        { 
            animator.SetTrigger(jumpTriggerHash);
        }

        private void OnMove()
        {
            //animator.SetFloat("VelX", 0f);

            // 將全域空間中的移動方向轉換為本地空間（相對於此物體的座標系）中的移動方向
            Vector3 localMoveDirection = transform.InverseTransformDirection(playerMovementController.DeltaPos);
            //animator.SetFloat(velXHash, localMoveDirection.x * playerMovementController.MoveSpeed * 10f);
            //animator.SetFloat(velYHash, localMoveDirection.z * playerMovementController.MoveSpeed * 10f);

            // 計算本地移動方向的標準化向量
            Vector3 normalVel = Vector3.Normalize(localMoveDirection);
            // 使用輸入變化的振幅來對本地移動方向和標準化向量之間進行插值，得到目標移動方向
            Vector3 targetVel = Vector3.Lerp(localMoveDirection, normalVel, playerMovementController.DeltaInput.magnitude);
            // 將目標移動方向的 x 分量設置為動畫的 VelX 參數
            animator.SetFloat(velXHash, targetVel.x);
            // 將目標移動方向的 z 分量設置為動畫的 VelY 參數
            animator.SetFloat(velYHash, targetVel.z);

        }
        
    }

}
