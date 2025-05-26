using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TafraKitEditor
{
    public static class SerializedObjectExtentions
    {
        public static IEnumerable<SerializedProperty> GetVisibleProperties(this SerializedObject serializedObject, bool ignoreScriptProperty = true)
        {
            SerializedProperty property = serializedObject.GetIterator();

            property.NextVisible(true);

            if (!ignoreScriptProperty || property.name != "m_Script")
                yield return property;

            while(property.NextVisible(false))
            {
                if(!ignoreScriptProperty || property.name != "m_Script")
                    yield return property;
            }
        }
    }
}