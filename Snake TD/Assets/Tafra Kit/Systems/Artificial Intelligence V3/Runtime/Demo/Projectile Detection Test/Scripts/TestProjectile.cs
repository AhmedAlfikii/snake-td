using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.AI3.Demo
{
    public class TestProjectile : FixedUpdateReceiver
    {
        private static int shot;
        private static int missed;

        [SerializeField] private float speed = 10;
        [Header("Collision Detection")]
        [SerializeField] protected LayerMask hittableLayers;

        protected Vector3 lastPosition;
        private float timer = 2;

        private void Start()
        {
            Destroy(gameObject, 5);
            lastPosition = transform.position;
            shot++;
        }
        public override void FixedTick()
        {
            transform.position += transform.forward * speed * Time.deltaTime;

            if (Physics.Linecast(lastPosition, transform.position, out RaycastHit hit, hittableLayers))
            {
                Destroy(gameObject);
            }

            #if UNITY_EDITOR
            Debug.DrawLine(lastPosition, transform.position);
            #endif

            lastPosition = transform.position;

            if (timer > 0)
                timer -= Time.deltaTime;
            else
            {
                missed++;

                Debug.Log($"<b>Miss rate = {((missed / (float)shot) * 100):#.0}. Shot = {shot}. Missed = {missed}.</b>");
                Destroy(gameObject);
            }

        }
    }
}