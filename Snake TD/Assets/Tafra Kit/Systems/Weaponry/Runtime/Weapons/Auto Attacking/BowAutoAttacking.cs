using System.Collections;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using TafraKit.Healthies;
using TafraKit.ZTweeners;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit.Weaponry
{
    public class BowAutoAttacking : RangedWeaponAutoAttacking
    {
        #region Serialized Fields
        [Header("Bow")]
        [SerializeField] private Transform stringCenterBone;
        [SerializeField] private Transform armingArrow;

        [Space(20)]

        [SerializeField] private Vector3 stringStressLocalPos;
        [SerializeField] private EasingType stressingMotionType;

        [Space(20)]

        [SerializeField] private Vector3 stringRestingLocalPos;
        [SerializeField] private EasingType restingMotionType;
        [SerializeField] private float restingAnimationDuration = 0.75f;

        [Space(20)]

        [SerializeField] private float armingCooldownDivider = 2f;
        #endregion

        #region Private Fields
        private ZTweenVector3 armingArrowScaleTween = new ZTweenVector3();
        private IEnumerator arrowCooldownEnum;
        private IEnumerator stringMotionEnum;
        #endregion

        #region Properties
        #endregion

        #region MonoBehaviour Messages
        protected override void Start()
        {
            base.Start();

            ArmArrow(false);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            armingArrowScaleTween.Stop();
        }
        #endregion

        public override void StartAttackingTarget(Healthy target)
        {
            StartCoroutine(AttackProcess(target));
        }

        private IEnumerator AttackProcess(Healthy target)
        {
            animator.SetTrigger(animatorAttackTriggerHash);

            if(!armingArrow.gameObject.activeSelf)
                armingArrow.gameObject.SetActive(true);

            if(stringMotionEnum != null)
                StopCoroutine(stringMotionEnum);

            stringMotionEnum = AnimateString(stringRestingLocalPos, stringStressLocalPos, stressingMotionType, shootDelay);

            StartCoroutine(stringMotionEnum);

            yield return Yielders.GetWaitForSeconds(shootDelay);

            armingArrow.gameObject.SetActive(false);

            //if(!fakeAttack)
            {
                if (target)
                    PerformAttackAction(target);
            }

            if(stringMotionEnum != null)
                StopCoroutine(stringMotionEnum);

            stringMotionEnum = AnimateString(stringStressLocalPos, stringRestingLocalPos, restingMotionType, restingAnimationDuration);

            StartCoroutine(stringMotionEnum);

            if(arrowCooldownEnum != null)
                StopCoroutine(arrowCooldownEnum);

            arrowCooldownEnum = ArmArrowDelayed(curAttackCooldown / armingCooldownDivider);
            StartCoroutine(arrowCooldownEnum);
        }

        private void ArmArrow(bool animate = true)
        {
            armingArrow.gameObject.SetActive(true);

            armingArrow.localScale = Vector3.zero;

            armingArrow.ZTweenScale(armingArrowScaleTween, Vector3.one, animate ? 0.25f : 0);
        }
        IEnumerator ArmArrowDelayed(float delay)
        {
            yield return Yielders.GetWaitForSeconds(delay);

            ArmArrow();
        }
        IEnumerator AnimateString(Vector3 from, Vector3 to, EasingType motionType, float duration = 0.1f)
        {
            float startTime = Time.time, t;

            while(Time.time < startTime + duration)
            {
                t = (Time.time - startTime) / duration;
                t = MotionEquations.GetEaseFloat(t, motionType);

                stringCenterBone.localPosition = Vector3.LerpUnclamped(from, to, t);

                yield return null;
            }
        }
    }
}