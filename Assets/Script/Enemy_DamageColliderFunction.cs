using BasicCode;
using System;
using UnityEngine;

public class Enemy_DamageColliderFunction : MonoBehaviour
{
    [SerializeField] private GameObject ref_FX;

    private EnemyMovementController enemyMovementController;
    private EnemyAnimationController enemyAnimationController;
    private FireEventArgs fireEventArgs;
    private Collider col;
    private GameObject owner;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();
        col.enabled = false;

        enemyMovementController = GetComponentInParent<EnemyMovementController>();
        enemyMovementController.Fire1Received += OnFire;

        enemyAnimationController = GetComponentInParent<EnemyAnimationController>();
        enemyAnimationController.StartAnimReceived += OnStartAttackAnimation;
        enemyAnimationController.EndAnimReceived += OnEndAttatckAnimation;

        owner = enemyMovementController.gameObject;
    }

    private void OnFire(object sender, FireEventArgs e)
    {
        fireEventArgs = e;
    }
    private void OnStartAttackAnimation(object sender, EventArgs e)
    {
        col.enabled = true;
    }
    private void OnEndAttatckAnimation(object sender, EventArgs e)
    {
        col.enabled=false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb)
            { 
                Vector3 contactPoint = other.ClosestPointOnBounds(transform.position);
                //Vector3 direction = Vector3.Normalize(other.gameObject.transform.position - owner.transform.position);
                Vector3 direction = Vector3.one;
                rb.AddForce(direction * fireEventArgs.FireForce, ForceMode.Impulse);
                GameObject tempFx = Instantiate(ref_FX, contactPoint, Quaternion.identity);
                tempFx.AddComponent<ParticleEffectController>();

                if (!other.CompareTag("Player"))
                { 
                
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
