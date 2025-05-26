using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using System;
using TafraKit;

namespace TafraKitEditor
{
    public class ClassesGatherer
    {
        private static Dictionary<Type, Type[]> classChildren = new Dictionary<Type, Type[]>();

        public static void GatherChildrenOf(Type type)
        {
            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract && p.FullName != type.FullName);
             
            classChildren.Add(type, types.ToArray());
        }
        public static Type[] GetChildrenOf(Type type)
        {
            if(!classChildren.ContainsKey(type))
            {
                //TafraDebugger.Log("Class Gatherer", "The requested class's children weren't gathered, will gather them now.", TafraDebugger.LogType.Info);
                GatherChildrenOf(type);
            }

            return classChildren[type];
        }
    }
}