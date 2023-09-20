using System;
using UnityEngine;

namespace BasicCode
{
    public class EnemyAnimationController : MonoBehaviour
    {
        private EnemyMovementController enemyMovementController;
        private EnemyHealth enemyHealth;
        private Animator animator;

        private int velYHash;
        private int attackHash;
       
        private int hitTriggetHash, deadTriggerHash;
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
            startAnimReceived?.Invoke(this, EventArgs.Empty);
        }
        void EndAttackAnimation()
        {
            endAnimReceived?.Invoke(this, EventArgs.Empty);
        }
        
        // Start is called before the first frame update
        void Start()
        {
            enemyHealth = GetComponentInParent<EnemyHealth>();
            enemyHealth.HitReceived += OnHit;
            enemyHealth.DeadReceived += OnDead;

            enemyMovementController = GetComponentInParent<EnemyMovementController>();
            enemyMovementController.FireReceived += OnFire;

            animator = GetComponentInChildren<Animator>();
            velYHash = Animator.StringToHash("VelY"); ;

            attackHash = Animator.StringToHash("AttackTrigger");

            hitTriggetHash = Animator.StringToHash("HitTrigger");
            deadTriggerHash = Animator.StringToHash("DeadTrigger");
        }
        // Update is called once per frame
        void Update() { OnMove(); }
        private void OnMove() { animator.SetFloat(velYHash, enemyMovementController.DeltaPos.magnitude); }
        private void OnHit(object sender, EventArgs e) { animator.SetTrigger(hitTriggetHash); }
        private void OnDead(object sender, EventArgs e) { animator.SetTrigger(deadTriggerHash); }
        void OnFire(object sender, FireEventArgs e) { animator.SetTrigger(attackHash); }
    }

}
