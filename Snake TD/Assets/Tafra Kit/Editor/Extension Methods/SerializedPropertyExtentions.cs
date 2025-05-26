using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using TafraKit;

namespace TafraKitEditor
{
    public static class SerializedPropertyExtentions
    {
        private const BindingFlags allBindingFlags = (BindingFlags)(-1);

        public static string GetTypeActualName(this SerializedProperty serializedProperty)
        {
            string typeName = serializedProperty.type;
            if(serializedProperty.propertyType == SerializedPropertyType.ManagedReference)
            {
                int sIndex = typeName.IndexOf('<') + 1;
                int count = (typeName.Length - sIndex) - 1;

                typeName = typeName.Substring(sIndex, count);
            }

            return typeName;
        }
        public static string GetFixedPath(this SerializedProperty serializedProperty)
        {
            return serializedProperty.propertyPath.Replace(".Array.data[", "[");
        }
        /// <summary>
        /// Returns the parent serialized property of this serialized property if any.
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <returns></returns>
        public static SerializedProperty Parent(this SerializedProperty serializedProperty)
        {
            var propertyPaths = serializedProperty.propertyPath.Split('.');
            if(propertyPaths.Length <= 1)
            {
                return default;
            }

            var parentSerializedProperty = serializedProperty.serializedObject.FindProperty(propertyPaths.First());
            for(var index = 1; index < propertyPaths.Length - 1; index++)
            {
                if(propertyPaths[index] == "Array" && propertyPaths.Length > index + 1 && Regex.IsMatch(propertyPaths[index + 1], "^data\\[\\d+\\]$"))
                {
                    var match = Regex.Match(propertyPaths[index + 1], "^data\\[(\\d+)\\]$");
                    var arrayIndex = int.Parse(match.Groups[1].Value);
                    parentSerializedProperty = parentSerializedProperty.GetArrayElementAtIndex(arrayIndex);
                    index++;
                }
                else
                {
                    parentSerializedProperty = parentSerializedProperty.FindPropertyRelative(propertyPaths[index]);
                }
            }

            return parentSerializedProperty;
        }
        /// <summary>
        /// Returns this serialized property's index in it's parent array if found.
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <returns></returns>
        public static int GetMyIndexInParentArray(this SerializedProperty serializedProperty)
        {
            string[] propertyPaths = serializedProperty.propertyPath.Split('.');

            //This is not an array.
            if(propertyPaths.Length <= 1)
                return -1;

            string data = propertyPaths[propertyPaths.Length - 1];
            int firstBracketIndex = data.IndexOf('[');

            string numberString = data.Substring(firstBracketIndex + 1, data.Length - firstBracketIndex - 2);

            if(int.TryParse(numberString, out int elementIndex))
                return elementIndex;
            else
                return -1;
        }
        public static IEnumerable<SerializedProperty> GetVisibleChildren(this SerializedProperty serializedProperty)
        {
            serializedProperty = serializedProperty.Copy();
            var nextElement = serializedProperty.Copy();
            bool hasNextElement = nextElement.NextVisible(false);
            if(!hasNextElement)
            {
                nextElement = null;
            }

            bool hasChildren = serializedProperty.NextVisible(true);
            while(hasChildren)
            {
                if((SerializedProperty.EqualContents(serializedProperty, nextElement)))
                {
                    yield break;
                }

                yield return serializedProperty;

                bool hasNext = serializedProperty.NextVisible(false);
                if(!hasNext)
                {
                    break;
                }
            }
        }
        public static void MoveArrayElementUp(this SerializedProperty serializedProperty, int elementIndex, bool isSerializedByReference)
        {
            if(elementIndex <= 0)
                return;

            serializedProperty.InsertArrayElementAtIndex(elementIndex - 1);
            SerializedProperty newElement = serializedProperty.GetArrayElementAtIndex(elementIndex - 1);
            SerializedProperty oldElement = serializedProperty.GetArrayElementAtIndex(elementIndex + 1);

            if(isSerializedByReference)
                newElement.managedReferenceValue = serializedProperty.GetArrayElementAtIndex(elementIndex + 1).managedReferenceValue;
            else
                newElement.boxedValue = oldElement.boxedValue;

            serializedProperty.DeleteArrayElementAtIndex(elementIndex + 1);

            serializedProperty.serializedObject.ApplyModifiedProperties();
        }
        public static void MoveArrayElementDown(this SerializedProperty serializedProperty, int elementIndex, bool isSerializedByReference)
        {
            if(elementIndex >= serializedProperty.arraySize - 1)
                return;

            serializedProperty.InsertArrayElementAtIndex(elementIndex + 2);
            SerializedProperty newElement = serializedProperty.GetArrayElementAtIndex(elementIndex + 2);
            SerializedProperty oldElement = serializedProperty.GetArrayElementAtIndex(elementIndex);

            if(isSerializedByReference)
                newElement.managedReferenceValue = serializedProperty.GetArrayElementAtIndex(elementIndex).managedReferenceValue;
            else
                newElement.boxedValue = oldElement.boxedValue;

            serializedProperty.DeleteArrayElementAtIndex(elementIndex);

            serializedProperty.serializedObject.ApplyModifiedProperties();
        }

        public static FieldInfo GetFieldInfo(this SerializedProperty serializedProperty)
        { 
            FieldInfo fieldInfo = null;
            
            string propertyPath = serializedProperty.propertyPath.Replace(".Array.data[", "[");
            string[] pathSegments = propertyPath.Split('.');

            object obj = serializedProperty.serializedObject.targetObject;
            foreach(string element in pathSegments)
            {
                //Is an array/list
                if(element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetChildFieldOrPropertyValue(obj, elementName, index, true, out fieldInfo);
                }
                else
                {
                    obj = GetChildFieldOrPropertyValue(obj, element, true, out fieldInfo);
                }
            }

            return fieldInfo;
        }
        private static object GetChildFieldOrPropertyValue(object parentObject, string childName, bool searchInFieldsOnly, out FieldInfo fieldInfo)
        {
            fieldInfo = null;

            if(parentObject == null)
                return null;

            var type = parentObject.GetType();

            while(type != null)
            {
                var f = type.GetField(childName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if(f != null)
                {
                    fieldInfo = f;
                    return f.GetValue(parentObject);
                }

                if(!searchInFieldsOnly)
                {
                    var p = type.GetProperty(childName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if(p != null)
                        return p.GetValue(parentObject, null);
                }

                type = type.BaseType;
            }

            return null;
        }
        private static object GetChildFieldOrPropertyValue(object parentObject, string childName, int index, bool searchInFieldsOnly, out FieldInfo fieldInfo)
        {
            var enumerable = GetChildFieldOrPropertyValue(parentObject, childName, searchInFieldsOnly, out fieldInfo) as System.Collections.IEnumerable;

            if(enumerable == null) 
                return null;
          
            var enm = enumerable.GetEnumerator();

            for(int i = 0; i <= index; i++)
            {
                if(!enm.MoveNext())
                    return null;
            }
            object childObject = enm.Current;
            
            return childObject;
        }

        private static bool IsPropertyAList(string pathSegment, out string fieldName, out int index)
        {
            var regex = new Regex(@"(.+)\[(\d+)\]");
            var match = regex.Match(pathSegment);

            if(match.Success) // Property refers to an array or list
            {
                fieldName = match.Groups[1].Value;
                index = int.Parse(match.Groups[2].Value);
                return true;
            }
            else
            {
                fieldName = pathSegment;
                index = -1;
                return false;
            }
        }

        public static Type GetActualType(this SerializedProperty serializedProperty)
        {
            Type type = serializedProperty.serializedObject.targetObject.GetType();
            object obj = serializedProperty.serializedObject.targetObject;
            string[] pathSegments = serializedProperty.GetFixedPath().Split('.');

            foreach(var segment in pathSegments)
            {
                type = GetPropertySegmentType(segment, type, obj, out obj);
            }

            return type;
        }
        public static object GetActualValue(this SerializedProperty serializedProperty)
        {
            Type type = serializedProperty.serializedObject.targetObject.GetType();
            object obj = serializedProperty.serializedObject.targetObject;
            string[] pathSegments = serializedProperty.GetFixedPath().Split('.');

            foreach(var segment in pathSegments)
            {
                type = GetPropertySegmentType(segment, type, obj, out obj);
            }

            return obj;
        }
        private static Type GetPropertySegmentType(string pathSegment, Type type, object holdingObj, out object resultObj)
        {
            string fieldName;
            int index;

            if(IsPropertyAList(pathSegment, out fieldName, out index))
            {
                FieldInfo listFieldInfo = GetSerializedFieldInfo(type, fieldName);
                Type listType = listFieldInfo.FieldType;

                if(listType.IsArray)
                {
                    object arrayObj = listFieldInfo.GetValue(holdingObj);

                    IEnumerable enumerable = arrayObj as IEnumerable;
                    object targetElementObj = null;
                    if(enumerable != null)
                    {
                        int curIndex = 0;
                        foreach(object element in enumerable)
                        {
                            if(curIndex == index)
                            {
                                targetElementObj = element;
                                break;
                            }
                            curIndex++;
                        }
                    }

                    resultObj = targetElementObj;
                    return targetElementObj.GetType();
                }
                else // List<T> is the only other Unity-serializable collection
                {
                    IList list = listFieldInfo.GetValue(holdingObj) as IList;
                    resultObj = list[index];
                    return resultObj.GetType();
                }
            }
            else
            {
                FieldInfo fInfo = GetSerializedFieldInfo(type, fieldName);
                resultObj = fInfo.GetValue(holdingObj);
                return resultObj.GetType();
            }
        }
        private static FieldInfo GetSerializedFieldInfo(Type type, string name)
        {
            var field = type.GetFieldUnambiguous(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if(field == null)
                throw new MissingMemberException(type.FullName, name);

            return field;
        }

        public static SerializedProperty GetSibilingProperty(this SerializedProperty property, string siblingPropertyPath)
        {
            if(property.depth == 0) 
                return property.serializedObject.FindProperty(siblingPropertyPath);

            SerializedProperty parentProeprty = property.Parent();

            if(parentProeprty == null)
                return null;

            return parentProeprty.FindPropertyRelative(siblingPropertyPath);
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <author>
//   HiddenMonk
//   http://answers.unity3d.com/users/496850/hiddenmonk.html
//   
//   Johannes Deml
//   send@johannesdeml.com
// </author>
// --------------------------------------------------------------------------------------------------------------------


namespace Supyrb
{
    using System;
    using UnityEngine;
    using UnityEditor;
    using System.Reflection;

    /// <summary>
    /// Extension class for SerializedProperties
    /// See also: http://answers.unity3d.com/questions/627090/convert-serializedproperty-to-custom-class.html
    /// </summary>
    public static class SerializedPropertyExtensions
    {
        /// <summary>
        /// Get the object the serialized property holds by using reflection
        /// </summary>
        /// <typeparam name="T">The object type that the property contains</typeparam>
        /// <param name="property"></param>
        /// <returns>Returns the object type T if it is the type the property actually contains</returns>
        public static T GetValue<T>(this SerializedProperty property)
        {
            return GetNestedObject<T>(property.propertyPath, GetSerializedPropertyRootComponent(property));
        }

        /// <summary>
        /// Set the value of a field of the property with the type T
        /// </summary>
        /// <typeparam name="T">The type of the field that is set</typeparam>
        /// <param name="property">The serialized property that should be set</param>
        /// <param name="value">The new value for the specified property</param>
        /// <returns>Returns if the operation was successful or failed</returns>
        public static bool SetValue<T>(this SerializedProperty property, T value)
        {

            object obj = GetSerializedPropertyRootComponent(property);
            //Iterate to parent object of the value, necessary if it is a nested object
            string[] fieldStructure = property.propertyPath.Split('.');
            for (int i = 0; i < fieldStructure.Length - 1; i++)
            {
                obj = GetFieldOrPropertyValue<object>(fieldStructure[i], obj);
            }
            string fieldName = fieldStructure.Last();

            return SetFieldOrPropertyValue(fieldName, obj, value);

        }

        ///


        /// Get the component of a serialized property
        ///

        /// The property that is part of the component
        /// The root component of the property
        public static Component GetSerializedPropertyRootComponent(SerializedProperty property)
        {
            return (Component)property.serializedObject.targetObject;
        }
        ///


        /// Iterates through objects to handle objects that are nested in the root object
        ///

        /// The type of the nested object
        /// Path to the object through other properties e.g. PlayerInformation.Health
        /// The root object from which this path leads to the property
        /// Include base classes and interfaces as well
        /// Returns the nested object casted to the type T
        public static T GetNestedObject<T>(string path, object obj, bool includeAllBases = false)
        {
            foreach (string part in path.Split('.'))
            {
                Debug.Log(part);
                obj = GetFieldOrPropertyValue<object>(part, obj, includeAllBases);
            }
            return (T)obj;
        }
        public static T GetFieldOrPropertyValue<T>(string fieldName, object obj, bool includeAllBases = false, BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            Debug.Log(fieldName + ": " + obj.GetType());
            FieldInfo field = obj.GetType().GetField(fieldName, bindings);
            if (field != null) return (T)field.GetValue(obj);

            PropertyInfo property = obj.GetType().GetProperty(fieldName, bindings);
            if (property != null) return (T)property.GetValue(obj, null);

            if (includeAllBases)
            {

                foreach (Type type in GetBaseClassesAndInterfaces(obj.GetType()))
                {
                    field = type.GetField(fieldName, bindings);
                    if (field != null) return (T)field.GetValue(obj);

                    property = type.GetProperty(fieldName, bindings);
                    if (property != null) return (T)property.GetValue(obj, null);
                }
            }

            return default(T);
        }

        public static bool SetFieldOrPropertyValue(string fieldName, object obj, object value, bool includeAllBases = false, BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
        {
            FieldInfo field = obj.GetType().GetField(fieldName, bindings);
            if (field != null)
            {
                field.SetValue(obj, value);
                return true;
            }

            PropertyInfo property = obj.GetType().GetProperty(fieldName, bindings);
            if (property != null)
            {
                property.SetValue(obj, value, null);
                return true;
            }

            if (includeAllBases)
            {
                foreach (Type type in GetBaseClassesAndInterfaces(obj.GetType()))
                {
                    field = type.GetField(fieldName, bindings);
                    if (field != null)
                    {
                        field.SetValue(obj, value);
                        return true;
                    }

                    property = type.GetProperty(fieldName, bindings);
                    if (property != null)
                    {
                        property.SetValue(obj, value, null);
                        return true;
                    }
                }
            }
            return false;
        }

        public static IEnumerable<Type> GetBaseClassesAndInterfaces(this Type type, bool includeSelf = false)
        {
            List<Type> allTypes = new List<Type>();

            if (includeSelf) allTypes.Add(type);

            if (type.BaseType == typeof(object))
            {
                allTypes.AddRange(type.GetInterfaces());
            }
            else
            {
                allTypes.AddRange(
                Enumerable
                .Repeat(type.BaseType, 1)
                .Concat(type.GetInterfaces())
                .Concat(type.BaseType.GetBaseClassesAndInterfaces())
                .Distinct());
            }

            return allTypes;
        }
    }
}