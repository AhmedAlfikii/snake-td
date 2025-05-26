using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class AnimatorIKLookAt : MonoBehaviour
    {
        [SerializeField] private Transform head;
        [SerializeField] private float weightBlendSpeed = 1f;
        [SerializeField] private float targetShiftDuration = 0.5f;
        [SerializeField] private EasingType targetShiftEasing;
        [Range(0f, 1f)]
        [SerializeField] private float weight = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float bodyWeight = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float headWeight = 1f;

        private Transform aimPoint;
        private Transform lookAtTarget;
        private Animator animator;
        private bool isSwitchingTargets;
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
            { 
                aimPoint.position = lookAtTarget.position;
            }
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
            bool wasSwitchingTargets = isSwitchingTargets;

            if (switchingTargetEnum != null)
            {
                StopCoroutine(switchingTargetEnum);
                switchingTargetEnum = null;
            }

            isSwitchingTargets = false;

            //If we should remove the target.
            if(target == null)
                RemoveLookAtTarget();
            else
            {
                //If a new target should be set but there's an IK weight applied (either because we're looking at something or blending in or out of looking at something).
                //Then we should blend towards that new target.
                if(target != null && appliedWeight > 0.001f)
                {
                    lookAtTarget = null;

                    isSwitchingTargets = true;

                    Vector3 startPos = aimPoint.position;
                    switchingTargetEnum = CompactCouroutines.CompactCoroutine(0, targetShiftDuration, false,
                    execute: (t) =>
                    {
                        t = MotionEquations.GetEaseFloat(t, targetShiftEasing);
                        //TODO: make it rotate in a circle pivoted to the head instead of moving in a straight line towards the target position.
                        aimPoint.position = Vector3.LerpUnclamped(startPos, target.position, t);
                    },
                    onEnd: () =>
                    {
                        lookAtTarget = target;
                        isSwitchingTargets = false;
                    });

                    StartCoroutine(switchingTargetEnum);

                    appliedWeightTarget = weight;
                }
                //If there's a new target and there were no current targets.
                else
                    lookAtTarget = target;

                appliedWeightTarget = weight;
            }

        }
        public void RemoveLookAtTarget()
        {
            lookAtTarget = null;

            appliedWeightTarget = 0;
        }
    }
}
