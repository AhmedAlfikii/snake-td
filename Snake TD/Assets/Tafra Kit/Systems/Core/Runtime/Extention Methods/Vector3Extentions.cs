using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public static class Vector3Extentions
    {
        /// <summary>
        /// Returns the squared distance between a and b. Use this for better performance than the normal Distance function.
        /// </summary>
        /// <param name="vector3"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float DistanceSqr(Vector3 a, Vector3 b)
        {
            Vector3 dir = a - b;

            return dir.sqrMagnitude;
        }

        #region Vector3 Int
        /// <summary>
        /// Multiplies a vector by 10 and rounds it to integer.
        /// </summary>
        /// <param name="vector3"></param>
        /// <returns></returns>
        public static Vector3Int RoundToIntOf10(this Vector3 vector3)
        {
            Vector3 vector3MultBy10 = vector3 * 10;

            return new Vector3Int(Mathf.RoundToInt(vector3MultBy10.x), Mathf.RoundToInt(vector3MultBy10.y), Mathf.RoundToInt(vector3MultBy10.z));
        }
        /// <summary>
        /// Rounds a vector to integer.
        /// </summary>
        /// <param name="vector3"></param>
        /// <returns></returns>
        public static Vector3Int RoundToInt(this Vector3 vector3)
        {
            return new Vector3Int(Mathf.RoundToInt(vector3.x), Mathf.RoundToInt(vector3.y), Mathf.RoundToInt(vector3.z));
        }

        public static Vector3 DeroundIntOf10(this Vector3 vector3)
        {
            return vector3 / 10;
        }
        public static Vector3 DeroundIntOf10(this Vector3Int vector3Int)
        {
            return vector3Int / 10;
        }
        #endregion
    }
}