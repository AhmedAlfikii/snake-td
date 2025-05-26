using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.AI3.Demo
{
    public class PhysicsAgent : FixedUpdateReceiver
    {
        [SerializeField] private Transform target;
        [SerializeField] private float speed = 10;

        private Rigidbody rb;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public override void FixedTick()
        {
            Vector3 targetPosition = target.position;
            targetPosition.y = transform.position.y;

            rb.linearVelocity = (targetPosition - transform.position).normalized * speed;
        }
    }
}