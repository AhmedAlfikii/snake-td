using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace TafraKit
{
    public static class IntExtentions
    {
        public static string ToCompactNumberString(this int value)
        {
            return ZHelper.CompactNumberString(value);
        }
    }
}