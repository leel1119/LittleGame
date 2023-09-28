using UnityEngine;


namespace BasicCode
{
    public class EntryPoint_Basic : MonoBehaviour
    {
        private GameManager_Basic gameManager;
        // Start is called before the first frame update
        void Start()
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_Basic>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            { 
                gameManager.SetRevivalPos(transform.position);
                Destroy(gameObject);
            }
        }
    }
}
