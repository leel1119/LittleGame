using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasicCode
{
    public class CameraControler : MonoBehaviour
    {
        public GameObject player;
        public float sensitivity;

        private Vector3 offset;
        private Rigidbody rb;

        void Start()
        {
            offset = transform.position - player.transform.position;
            rb = GetComponent<Rigidbody>();
        }

        void LateUpdate()
        {
            transform.position = player.transform.position + offset;
        }

        void FixedUpdate()
        {
            float rotateHorizontal = Input.GetAxis("Mouse X");
            float rotateVertical = Input.GetAxis("Mouse Y");

            Vector3 rotation = new Vector3(rotateHorizontal, 0.0f, rotateVertical);

            rb.AddForce(rotation * sensitivity);

        }
    }
}
