using System;
using System.Collections;
using TafraKit.MotionFactory;
using UnityEngine;

namespace TafraKit
{
    public abstract class AttackIndicator : MonoBehaviour
    {
        #region Private Serialized Fields
        [SerializeField] private VisibilityMotionController motionController;
        #endregion

        #region Private Fields
        private IEnumerator chargeEnum;
        private Action onHideAction;
        protected bool isCharging;
        #endregion

        private IEnumerator Charging(float duration)
        {
            isCharging = true;

            OnStartCharging();

            float startTime = Time.time;

            while(Time.time - startTime < duration)
            {
                OnCharging((Time.time - startTime) / duration);
                yield return null;
            }

            isCharging = false;

            OnCompleteCharging();
        }
        private void StopCharging()
        {
            isCharging = false;

            StopCoroutine(chargeEnum);

            chargeEnum = null;
        }
        protected virtual void OnStartCharging() { }
        protected virtual void OnCharging(float t) { }
        protected virtual void OnCompleteCharging() { }

        #region Public Functions
        public abstract void Initialize(AttackIndicatorData data);
        public void Show(bool immediate = false)
        {
            motionController.Show(immediate);
        }
        public void Hide(bool immediate = false, Action hideAction = null)
        {
            if(isCharging)
                StopCharging();

            onHideAction = hideAction;

            motionController.OnHideComplete.AddListener(OnHideCompleted);

            motionController.Hide(immediate);
        }
        public void StartCharging(float duration)
        {
            if(chargeEnum != null)
                StopCoroutine(chargeEnum);

            chargeEnum = Charging(duration);

            StartCoroutine(chargeEnum);
        }
        #endregion

        #region Callbacks
        private void OnHideCompleted()
        {
            motionController.OnHideComplete.RemoveListener(OnHideCompleted);

            onHideAction?.Invoke();

            onHideAction = null;
        }
        #endregion
    }
}