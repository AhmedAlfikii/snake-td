using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public static class ZBezier
    {
        public static Vector3 GetPointOnQuadraticCurve(float t, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * p1 +
                2f * oneMinusT * t * p2 +
                t * t * p3;
        }
    }
}
