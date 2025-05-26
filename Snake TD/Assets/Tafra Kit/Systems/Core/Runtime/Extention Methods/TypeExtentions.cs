using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace TafraKit
{
    public static class TypeExtentions
    {
        public static bool IsList(this Type type)
        {
            return typeof(IList).IsAssignableFrom(type);
        }
        public static FieldInfo GetFieldUnambiguous(this Type type, string name, BindingFlags flags)
        {
            flags |= BindingFlags.DeclaredOnly;

            while(type != null)
            {
                var field = type.GetField(name, flags);

                if(field != null)
                {
                    return field;
                }

                type = type.BaseType;
            }

            return null;
        }
        public static List<FieldInfo> GetFieldsUnambiguous(this Type type, BindingFlags flags)
        {
            List<FieldInfo> list = new List<FieldInfo>();

            flags |= BindingFlags.DeclaredOnly;

            while(type != null)
            {
                FieldInfo[] fields = type.GetFields(flags);
                
                list.AddRange(fields);

                type = type.BaseType;
            }

            return list;
        }

    }
}
