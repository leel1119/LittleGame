using UnityEngine;

namespace BasicCode
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private float damping = 5f;
        [SerializeField] private Vector3 offset;
        [SerializeField] private float minDistance = 1f;
        [SerializeField] private float maxDistance = 10f;
        [SerializeField] private float scrollSpeed = 45f;
        [SerializeField] private float tiltAngle = 45F;
        [SerializeField] private float panAngle = -45f;

        private string triggerEnterTag = "Player";
        private Transform target;
        // Start is called before the first frame update
        void Start()
        {
            target = GameObject.FindGameObjectWithTag(triggerEnterTag).transform;
        }

        private void LateUpdate()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            float distance = offset.magnitude;

            distance -= scroll * scrollSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            offset = Quaternion.Euler(tiltAngle, panAngle, 0f) * new Vector3 (0f, 0f, -maxDistance);
            offset = offset.normalized * distance;
            Vector3 targetposition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetposition, Time.deltaTime * damping);

            //Rot
            Vector3 targetDirection = Vector3.Normalize(target.position - transform.position);
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * damping);
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}
