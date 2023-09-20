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
    void Start()
    {
        col = GetComponent<Collider>();
        col.enabled = false;

        enemyMovementController = GetComponentInParent<EnemyMovementController>();
        enemyMovementController.FireReceived += OnFire;
        owner = enemyMovementController.gameObject;

        enemyAnimationController = GetComponentInParent<EnemyAnimationController>();
        enemyAnimationController.StartAnimReceived += OnStartAttackAnimation;
        enemyAnimationController.EndAnimReceived += OnEndAttatckAnimation; 
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
            Debug.Log("Trigger entered!");
            Rigidbody rb = other.attachedRigidbody;
            if (rb)
            { 
                Vector3 contactPoint = other.ClosestPointOnBounds(transform.position);
                Vector3 direction = Vector3.Normalize(other.gameObject.transform.position - owner.transform.position);
                //Vector3 direction = Vector3.one;
                rb.AddForce(direction * fireEventArgs.FireForce, ForceMode.Impulse);
                GameObject tempFx = Instantiate(ref_FX, contactPoint, Quaternion.identity);
                tempFx.AddComponent<ParticleEffectController>();

                if (!other.CompareTag("Enemy"))
                {
                    float TotalDamage = fireEventArgs.FireDamage;
                    PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                    if (playerHealth)
                    { 
                        playerHealth.OnDamage(TotalDamage);
                    }
                }
            }
        }
    }
}
