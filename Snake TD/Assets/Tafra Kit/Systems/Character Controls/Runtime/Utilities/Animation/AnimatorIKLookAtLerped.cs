using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class AnimatorIKLookAtLerped : MonoBehaviour
    {
        [SerializeField] private Transform head;
        [SerializeField] private float weightBlendSpeed = 1f;
        [SerializeField] private float targetAimSpeed = 6f;
        [Range(0f, 1f)]
        [SerializeField] private float weight = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float bodyWeight = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float headWeight = 1f;

        [SerializeField] private Transform aimPoint;
        private Transform lookAtTarget;
        private Animator animator;
        private float appliedWeight;
        private float appliedWeightTarget;
        private IEnumerator switchingTargetEnum;

        private void Awake()
        { 
            animator = GetComponent<Animator>();

            if (aimPoint == null)
                aimPoint = new GameObject(name + "_AimPoint").transform;
        }

        private void LateUpdate()
        {
            appliedWeight = Mathf.MoveTowards(appliedWeight, appliedWeightTarget, weightBlendSpeed * Time.deltaTime);

            if (lookAtTarget)
                aimPoint.position = Vector3.Lerp(aimPoint.position, lookAtTarget.position, targetAimSpeed * Time.deltaTime);
        }
        private void OnAnimatorIK(int layerIndex)
        {
            if(appliedWeight < 0.0001f)
                return;

            Vector3 lookPoint = aimPoint.position;

            animator.SetLookAtWeight(appliedWeight, bodyWeight, headWeight);
            animator.SetLookAtPosition(lookPoint);
        }

        public void SetLookAtTarget(Transform target)
        {
            if (switchingTargetEnum != null)
            {
                StopCoroutine(switchingTargetEnum);
                switchingTargetEnum = null;
            }

            if(target)
            {
                lookAtTarget = target;
                appliedWeightTarget = weight;
            }
            else
                RemoveLookAtTarget();
        }
        public void RemoveLookAtTarget()
        {
            lookAtTarget = null;

            appliedWeightTarget = 0;
        }
    }
}
