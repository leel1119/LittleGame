using UnityEngine;

namespace BasicCode
{
    public class GemsFunction_Basic : MonoBehaviour
    {
        private GameManager_Basic gameManager;
        // Start is called before the first frame update
        void Start()
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager_Basic>();
        }

        // Update is called once per frame
        void Update()
        {

        }
        private void OnTriggerEnter(Collider other)
        {
            gameManager.AddGems();
            Destroy(gameObject);
        }
    }
}
