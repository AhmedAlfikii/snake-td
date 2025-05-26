using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class LineAttackIndicatorData : AttackIndicatorData
    {
        public Vector3 StartPosition { get; private set; }
        public Vector3 EndPosition { get; private set; }
        public float Width { get; private set; }

        public void SetData(Vector3 startPosition, Vector3 endPostion, float width)
        {
            StartPosition = startPosition;
            EndPosition = endPostion;
            Width = width;
        }
    }
}