using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace TafraKit
{
    public static class FloatExtentions
    {
        public static string ToCompactNumberString(this float value)
        {
            return ZHelper.CompactNumberString(value);
        }
    }
}