using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit
{
    public class AreaDetector : MonoBehaviour
    {
        #region Private Serialized Fields
        [SerializeField] private bool handleActivationOnAwake = true;
        [SerializeField] private bool activateOnAwake = true;
        [SerializeField] private bool deactivateAfterDetectingTarget;
        [Tooltip("How often should the script attempt to look for colliders in range.")]
        [SerializeField] private float detectionRate = 0.5f;
        [SerializeField] private float detectionRadius = 5f;
        [SerializeField] private Transform detectionCenter;
        [SerializeField] private int detectionMaxCount = 50;
        [SerializeField] private LayerMask detectionLayers;
        #endregion

        #region Public Events
        public ColliderArrayIntEvent OnUpdate = new ColliderArrayIntEvent();
        public ColliderArrayIntEvent OnTargetDetected = new ColliderArrayIntEvent();
        #endregion

        #region Private Fields
        private Collider[] detectedColliders;
        private int detectedCollidersCount;
        private float nextDetectionTime;
        private bool isActive;
        #endregion

        #region MonoBehaviour Messages
        void Awake()
        {
            if (handleActivationOnAwake)
            {
                if(activateOnAwake)
                    Activate();
                else
                    Deactivate();
            }

            detectedColliders = new Collider[detectionMaxCount];
        }
        void Update()
        {
            if (isActive && Time.time >= nextDetectionTime)
            {
                detectedCollidersCount = Physics.OverlapSphereNonAlloc(detectionCenter.position, detectionRadius, detectedColliders, detectionLayers);
                
                if (detectedCollidersCount > 0)
                {
                    if(deactivateAfterDetectingTarget) 
                        Deactivate();
                    
                    OnTargetDetected?.Invoke(detectedColliders,detectedCollidersCount);
                }

                nextDetectionTime = Time.time + detectionRate;

                OnUpdate?.Invoke(detectedColliders, detectedCollidersCount);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(detectionCenter.position, detectionRadius);
        }
        #endregion

        #region Public Functions
        public void Activate()
        {
            nextDetectionTime = Time.time + detectionRate;

            isActive = true;
        }
        public void Deactivate()
        {
            isActive = false;
        }

        public void ForceDetect()
        {
            nextDetectionTime = Time.time - 1;
        }

        public Collider[] GetDetectedColliders()
        {
            return detectedColliders;
        }

        public int GetDetectedCollidersCount()
        {
            return detectedCollidersCount;
        }

        public bool IsActive()
        {
            return isActive;
        }
        #endregion
    }
}