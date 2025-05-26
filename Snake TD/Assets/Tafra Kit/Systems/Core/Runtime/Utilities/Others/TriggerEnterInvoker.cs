using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit
{
    public class TriggerEnterInvoker : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask;

        public UnityEvent OnEnter;

        private void OnTriggerEnter(Collider other)
        {
            if(layerMask != (layerMask | (1 << other.gameObject.layer)))
                return;

            OnEnter?.Invoke();
        }
    }
}