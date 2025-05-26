using UnityEngine;

namespace TafraKit
{
    public class LineAttackIndicator : AttackIndicator
    {
        #region Private Serialized Fields
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private LineRenderer chargeLineRenderer;
        #endregion

        #region Private Fields
        private LineAttackIndicatorData linearIndicatorData;
        #endregion

        #region Monobehaviour Messages
        #endregion

        #region Private Functions
        #endregion

        #region Public Functions
        public override void Initialize(AttackIndicatorData data)
        {
            linearIndicatorData = (LineAttackIndicatorData)data;

            lineRenderer.SetPosition(0, linearIndicatorData.StartPosition);
            lineRenderer.SetPosition(1, linearIndicatorData.EndPosition);
            lineRenderer.startWidth = linearIndicatorData.Width;
            lineRenderer.endWidth = linearIndicatorData.Width;

            chargeLineRenderer.SetPosition(0, linearIndicatorData.StartPosition);
            chargeLineRenderer.SetPosition(1, linearIndicatorData.EndPosition);
            chargeLineRenderer.startWidth = 0;
            chargeLineRenderer.endWidth = 0;
        }
        public void SetStartPosition(Vector3 startPos)
        {
            lineRenderer.SetPosition(0, startPos);

            chargeLineRenderer.SetPosition(0, startPos);
        }
        public void SetEndPosition(Vector3 endPos)
        {
            lineRenderer.SetPosition(1, endPos);

            chargeLineRenderer.SetPosition(1, endPos);
        }
        public Vector3 GetStartPosition()
        {
            return lineRenderer.GetPosition(0);
        }
        public Vector3 GetEndPosition()
        {
            return lineRenderer.GetPosition(1);
        }
        #endregion

        protected override void OnStartCharging()
        {
        }

        protected override void OnCharging(float t)
        {
            chargeLineRenderer.startWidth = Mathf.Lerp(0, linearIndicatorData.Width, t);
            chargeLineRenderer.endWidth = Mathf.Lerp(0, linearIndicatorData.Width, t);
        }

        protected override void OnCompleteCharging()
        {
        }
    }
}