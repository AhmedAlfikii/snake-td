using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public static class Vector3IntExtentions
    {
        public static Vector3 ToVector3(this Vector3Int vector3Int)
        {
            return new Vector3(vector3Int.x / 10f, vector3Int.y / 10f, vector3Int.z / 10f);
        }
    }
}