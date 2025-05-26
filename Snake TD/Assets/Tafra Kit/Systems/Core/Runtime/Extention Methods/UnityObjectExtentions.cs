using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace TafraKit
{
    public static class UnityObjectExtentions
    {
        public static GameObject GameObject(this UnityEngine.Object obj)
        {
            if (obj is GameObject)
                return (GameObject)obj;
            else if (obj is Component)
                return ((Component)obj).gameObject;
            else
                return null;
        }
    }
}
