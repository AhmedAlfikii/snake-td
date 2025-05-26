using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.AI3.Demo
{
    public class StickingObject : MonoBehaviour
    {
        [SerializeField] private Transform target;
        private Rigidbody rb;
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }
        void FixedUpdate()
        {
            rb.position = target.position;
        }
    }
}