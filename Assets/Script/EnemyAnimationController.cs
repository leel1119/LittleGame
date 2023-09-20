using System;
using UnityEngine;

namespace BasicCode
{
    public class EnemyAnimationController : MonoBehaviour
    {
        private EnemyMovementController enemyMovementController;
        private Animator animator;
        private int velYHash;
        private int jumpTriggerHash, isGroundedHash;
        private int attackIndex = 0, attackMaxIndex = 2;
        private int attackHash;
        private EventHandler startAnimReceived, endAnimReceived;
        private int hitTriggetHash, deadTriggerHash;
        private EnemyHealth enemyHealth;

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
            isGroundedHash = Animator.StringToHash("IsGrounded");
            enemyMovementController = GetComponentInParent<EnemyMovementController>();
            enemyMovementController.Fire1Received += OnFire;
            attackHash = Animator.StringToHash("AttackTrigger");
            animator = GetComponentInChildren<Animator>();
            velYHash = Animator.StringToHash("VelY"); ;

            enemyHealth = GetComponentInParent<EnemyHealth>();
            enemyHealth.HitReceived += OnHit;
            enemyHealth.DeadReceived += OnDead;

            hitTriggetHash = Animator.StringToHash("HitTrigger");
            deadTriggerHash = Animator.StringToHash("DeadTrigger");
        }

        private void OnHit(object sender, EventArgs e) { animator.SetTrigger(hitTriggetHash); }
        private void OnDead(object sender, EventArgs e) { animator.SetTrigger(deadTriggerHash); }
        public void OnFire(object sender, FireEventArgs e)
        {
            animator.SetTrigger(attackHash);
        }

        // Update is called once per frame
        void Update()
        {
            OnMove();
        }
        void OnJump(object sender, EventArgs e)
        { 
            animator.SetTrigger(jumpTriggerHash);
        }

        private void OnMove()
        {
            animator.SetFloat(velYHash, enemyMovementController.DeltaPos.magnitude);
        }
        void StartAttackAnimation()
        
    }

}
