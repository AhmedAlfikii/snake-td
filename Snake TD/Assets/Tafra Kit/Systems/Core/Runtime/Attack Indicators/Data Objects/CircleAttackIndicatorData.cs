using UnityEngine;

namespace TafraKit
{
    public class CircleAttackIndicatorData : AttackIndicatorData
    {
        public Vector3 Position { get; private set; }
        public float Radius { get; private set; }

        public void SetData(Vector3 position, float radius)
        {
            Position = position;
            Radius = radius;
        }
    }
}