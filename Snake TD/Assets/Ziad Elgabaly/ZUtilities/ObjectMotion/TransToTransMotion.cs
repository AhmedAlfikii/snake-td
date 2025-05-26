using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class TransToTransMotion : ObjectMotion
    {
        [SerializeField] private Transform pointA;
        [SerializeField] private Transform pointB;

        [SerializeField] private bool controlPosition = true;
        [SerializeField] private bool controlScale = true;
        [SerializeField] private bool controlRotation = true;

        protected override void OnEnable()
        {
            transform.position = pointA.position;

            base.OnEnable();
        }

        protected override void ApplyMotion(float easedT, float rawT, bool inverted)
        {
            Transform start = inverted ? pointB : pointA;
            Transform end = inverted ? pointA : pointB;

            if (controlPosition)
                transform.position = Vector3.Lerp(start.position, end.position, easedT);

            if (controlScale)
                transform.localScale = Vector3.Lerp(start.localScale, end.localScale, easedT);

            if (controlRotation)
                transform.rotation = Quaternion.Slerp(start.rotation, end.rotation, easedT);
        }
    }
}