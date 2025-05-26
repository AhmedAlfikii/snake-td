using UnityEngine;

namespace TafraKit
{
    public class RectAttackIndicatorData : AttackIndicatorData
    {
        public Vector3 StartPosition { get; private set; }
        public Vector3 Direction { get; private set; }
        public float Width { get; private set; }
        public float Length { get; private set; }

        public void SetData(Vector3 startPosition, Vector3 direction, float width, float length)
        {
            StartPosition = startPosition;
            Direction = direction;
            Width = width;
            Length = length;
        }
    }
}