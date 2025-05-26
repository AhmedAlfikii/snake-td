#if TAFRA_ANIMATION_RIGGING
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace TafraKit
{
    public class AnimationRigLookAtController : MonoBehaviour
    {
        [SerializeField] private Rig rig;
        [SerializeField] private MonoBehaviour[] constraintBehaviours;
        [SerializeField] private float weightBlendSpeed = 1f;
        [SerializeField] private float targetShiftDuration = 0.5f;
        [SerializeField] private EasingType targetShiftEasing;
        [Range(0f, 1f)]
        [SerializeField] private float weight = 1f;
        [SerializeField] private Transform aimPoint;

        private Transform lookAtTarget;
        private Transform switchingTolookAtTarget;
        private bool isSwitchingTargets;
        private float appliedWeight;
        private float appliedWeightTarget;
        private IEnumerator switchingTargetEnum;
        private List<IRigConstraint> rigConstraints = new List<IRigConstraint>();
        private List<float> defaultRigConstraintsWeight = new List<float>();
        private List<float> curRigConstraintsWeight = new List<float>();
        private int curSetterHash = -1;

        private void Awake()
        {
            for(int i = 0; i < constraintBehaviours.Length; i++)
            {
                IRigConstraint constraint = constraintBehaviours[i] as IRigConstraint;

                if(constraint != null)
                {
                    rigConstraints.Add(constraint);
                    defaultRigConstraintsWeight.Add(constraint.weight);
                    curRigConstraintsWeight.Add(0);
                }
            }
        }
        private void LateUpdate()
        {
            if(!rig)
                return;

            appliedWeight = Mathf.MoveTowards(appliedWeight, appliedWeightTarget, weightBlendSpeed * Time.deltaTime);
            rig.weight = appliedWeight;

            if(lookAtTarget)
                aimPoint.position = lookAtTarget.position;
        }

        public void SetLookAtTarget(int setterHash, Transform target, int influencedConstraints = 9999)
        {
            curSetterHash = setterHash;

            if (target == lookAtTarget)
                return;

            bool wasSwitchingTargets = isSwitchingTargets;

            if(switchingTargetEnum != null)
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
                    switchingTolookAtTarget = target;
                    Vector3 startPos = aimPoint.position;
                    switchingTargetEnum = CompactCouroutines.CompactCoroutine(0, targetShiftDuration, false,
                    execute: (t) =>
                    {
                        t = MotionEquations.GetEaseFloat(t, targetShiftEasing);
                        if(target != null)
                        {
                            //TODO: make it rotate in a circle pivoted to the head instead of moving in a straight line towards the target position.
                            aimPoint.position = Vector3.LerpUnclamped(startPos, target.position, t);
                        }
                    },
                    onEnd: () =>
                    {
                        lookAtTarget = target;
                        switchingTolookAtTarget = null;
                        isSwitchingTargets = false;
                    });

                    StartCoroutine(switchingTargetEnum);

                    appliedWeightTarget = weight;
                }
                //If there's a new target and there were no current targets.
                else
                    lookAtTarget = target;

                appliedWeightTarget = weight;

                for(int i = 0; i < rigConstraints.Count; i++)
                {
                    if (i < influencedConstraints)
                        rigConstraints[i].weight = defaultRigConstraintsWeight[i];
                    else
                        rigConstraints[i].weight = 0;
                }
            }

        }
        public void RemoveLookAtTarget(int removerHash = -1, Transform target = null)
        {
            if((removerHash != -1 && curSetterHash != removerHash) || (target != null && lookAtTarget != target && switchingTolookAtTarget != target))
                return;

            if(switchingTolookAtTarget == target && switchingTargetEnum != null)
                StopCoroutine(switchingTargetEnum);

            curSetterHash = -1;

            lookAtTarget = null;

            appliedWeightTarget = 0;
        }

        public void SetConstraintsProperties(WeaponRigConstraintsProfile constraintsProfile)
        {
            for(int i = 0; i < constraintsProfile.RigConstraintProperties.Length; i++)
            {
                RigConstraintProperties properties = constraintsProfile.RigConstraintProperties[i];

                defaultRigConstraintsWeight[i] = properties.Weight;

                MonoBehaviour rigMono = constraintBehaviours[i];

                if(rigMono is MultiAimConstraint multiAimConstraint)
                {
                    multiAimConstraint.weight = properties.Weight;
                    multiAimConstraint.data.offset = properties.Offset;
                }
                else if(rigMono is MultiRotationConstraint multiRotationConstraint)
                {
                    multiRotationConstraint.weight = properties.Weight;
                    multiRotationConstraint.data.offset = properties.Offset;
                }
            }
        }
    }
}
#endif