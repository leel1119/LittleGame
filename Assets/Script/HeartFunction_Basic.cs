using UnityEngine;


namespace BasicCode
{
    public class HeartFunction_Basic : MonoBehaviour
    {
        [SerializeField] public int HealAmount = 30;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                playerHealth.OnHealing(HealAmount);
                Destroy(gameObject);
            }
        }
    }
}