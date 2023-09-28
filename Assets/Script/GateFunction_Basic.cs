using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasicCode
{
    public class GateFunction_Basic : MonoBehaviour
    {
        public List<EnemyHealth> enemyHealth;
        private int clearConditionAmout = 0;
        private int maxCount = 0;

        private Vector3 targetPos;
        private bool isTrigger = false;

        private float lerp = 0.01f;
        private float AfterDestoryTime = 3f;
        // Start is called before the first frame update
        void Start()
        {
            var size = GetComponent<BoxCollider>().size;
            targetPos = transform.position + (Vector3.up * -size.y);

            maxCount = enemyHealth.Count;

            foreach (EnemyHealth health in enemyHealth)
            {
                health.DeadReceived += OnDead;
            }
        }

        private void OnDead(object sender, EventArgs e)
        {
            clearConditionAmout += 1;
            if (clearConditionAmout >= maxCount)
            { 
                isTrigger = true;
                StartCoroutine(AfterDestory());
            }
        }

        private void Update()
        {
            if (isTrigger) 
            {
                transform.position = Vector3.Lerp(transform.position, targetPos, lerp);
            }
        }

        IEnumerator AfterDestory()
        { 
            yield return new WaitForSeconds(AfterDestoryTime);
            Destroy(gameObject);
        }
    }
}
