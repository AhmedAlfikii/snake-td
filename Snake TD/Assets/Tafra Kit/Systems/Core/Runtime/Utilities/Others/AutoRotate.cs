using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class AutoRotate : MonoBehaviour
    {
        [SerializeField] private Vector3 direction;
        [SerializeField] private Space space = Space.Self;
        [SerializeField] private bool useUnscaledTime;

        [SerializeField] private bool applyRandomRotationOnEnable;
        [SerializeField] private Vector3 minRandomRoation;
        [SerializeField] private Vector3 maxRandomRoation;
        private void OnEnable()
        {
            if(applyRandomRotationOnEnable)
                transform.eulerAngles = new Vector3(Random.Range(minRandomRoation.x, maxRandomRoation.x),
                    Random.Range(minRandomRoation.y, maxRandomRoation.y),
                    Random.Range(minRandomRoation.z, maxRandomRoation.z));
        }
        void Update()
        {
            transform.Rotate(direction * (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime), space);
        }
    }
}