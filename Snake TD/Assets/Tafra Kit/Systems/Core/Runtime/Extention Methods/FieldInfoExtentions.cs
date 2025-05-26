using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace TafraKit
{
    public static class FieldInfoExtentions
    {
        public static bool IsList(this FieldInfo fieldInfo)
        {
            return typeof(IList).IsAssignableFrom(fieldInfo.FieldType);

            Type fieldType = fieldInfo.FieldType;
            return fieldType.IsGenericType && (fieldType.GetGenericTypeDefinition() == typeof(List<>));
        }
    }
}
