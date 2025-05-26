using UnityEngine;
using UnityEngine.AI;

namespace TafraKit
{
    public static class NavMeshPathExtentions
    {
        public static float GetLength(this NavMeshPath path, float lol)
        {
            float length = 0;

            for (int i = 1; i < path.corners.Length; i++)
            {
                length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }

            return length;
        }
    }
}