using UnityEngine;
using UnityEngine.InputSystem.UI;

namespace TafraKit
{
    public class CircleAttackIndicator : AttackIndicator
    {
        #region Private Serialized Fields
        [SerializeField] private Transform circleGraphicTransform;
        [SerializeField] private Transform chargeCircleTransform;
        public float radius;
        #endregion

        #region Public Functions
        public override void Initialize(AttackIndicatorData data)
        {
            CircleAttackIndicatorData circularData = (CircleAttackIndicatorData)data;

            transform.position = circularData.Position;
            radius = circularData.Radius;
            circleGraphicTransform.localScale = new Vector3(circularData.Radius, 1, circularData.Radius);

            chargeCircleTransform.localScale = Vector3.zero;
        }
        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        protected override void OnStartCharging()
        {
        }
        protected override void OnCharging(float t)
        {
            chargeCircleTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
        }
        protected override void OnCompleteCharging()
        {
        }
        #endregion
    }
}