using UnityEngine;

namespace TafraKit
{
    public class RectAttackIndicator : AttackIndicator
    {
        #region Private Serialized Fields
        [SerializeField] private Transform baseTransform;
        [SerializeField] private Transform chargeTransform;
        #endregion

        private Vector3 endPoint;

        public override void Initialize(AttackIndicatorData data)
        {
            RectAttackIndicatorData rectData = (RectAttackIndicatorData)data;

            transform.position = rectData.StartPosition;

            Vector3 normalizedDir = rectData.Direction.normalized;

            transform.forward = normalizedDir;

            endPoint = rectData.StartPosition + normalizedDir * rectData.Length;

            baseTransform.localScale = new Vector3(rectData.Width, rectData.Length, 1);

            chargeTransform.localScale = new Vector3(1, 0, 1);
        }
        public void Refresh(Vector3 startPosition, Vector3 direction, float width, float length)
        {
            transform.position = startPosition;

            transform.forward = direction;

            baseTransform.localScale = new Vector3(width, length, 1);
        }
        public void Refresh(Vector3 startPosition, float length)
        {
            transform.position = startPosition;

            baseTransform.localScale = new Vector3(baseTransform.localScale.x, length, 1);
        }
        public void RefreshAndMaintainEndPoint(Vector3 startPosition)
        {
            transform.position = startPosition;

            float newLength = (endPoint - startPosition).magnitude;

            baseTransform.localScale = new Vector3(baseTransform.localScale.x, newLength, 1);
        }
        public void SetStartPosition(Vector3 startPosition)
        {
            transform.position = startPosition;
        }
        protected override void OnCharging(float t)
        {
            chargeTransform.localScale = new Vector3(1, Mathf.Lerp(0, 1, t), 1);
        }

        protected override void OnCompleteCharging()
        {
            chargeTransform.localScale = Vector3.one;
        }
    }
}