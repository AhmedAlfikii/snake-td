using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Demos
{
    public class CameraFollow : MonoBehaviour
    {
        #region Private Serialized Fields
        [SerializeField] private Transform target;
        #endregion
        #region Private Fields
        private Vector3 offset;
        #endregion

        #region MonoBehaviour Messages
        private void Start()
        {
            offset = transform.position - target.position;
        }
        void LateUpdate()
        {
            transform.position = target.position + offset;
        }
        #endregion
    }
}