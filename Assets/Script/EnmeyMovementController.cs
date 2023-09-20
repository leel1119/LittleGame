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

        [Header("攻擊參數")]
        [SerializeField] private float closeAttackDistance = 2.0f;
        [SerializeField] private Vector2 FireRandomInterval = new Vector2(1f, 2f);
        [SerializeField] private float FireDamage = 1.0f;
        [SerializeField] private float FireForce = 1.0f;
        private float fireCooldown = 0f;

        private Vector3 deltaPos;
        public Vector3 DeltaPos => deltaPos;

        private EventHandler<FireEventArgs> fireReceived;
        public event EventHandler<FireEventArgs> FireReceived
        {
            add { fireReceived += value; }
            remove { fireReceived -= value; }
        }
        private Vector3 originPos;
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
                if(playerHealth.IsDead) target =null;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                target = null;
            }
        }
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
        void OnFire()
        {
            float distance = Vector3.Distance(target.transform.position, transform.position);
            if (distance <= closeAttackDistance)
            { 
                Quaternion targetRot = Quaternion.LookRotation(target.transform.position - agent.transform.position);
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
    }
}