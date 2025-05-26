using System;
using UnityEditor;
using UnityEngine;

namespace TafraKit
{
    /// <summary>
    /// Link the visibility of a field to the value of a property.
    /// </summary>
    public class PropertyDependentFieldAttribute : PropertyAttribute
    {
        private readonly string targetPropertyPath;
        private readonly object targetPropertyValue;

        public string TargetPropertyPath => targetPropertyPath;
        public object TargetPropertyValue => targetPropertyValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetPropertyPath">The path of the target property relative to this property's parent.</param>
        /// <param name="targetPropertyValue">The value of the target property that this field should be visible if matched.</param>
        public PropertyDependentFieldAttribute(string targetPropertyPath, object targetPropertyValue)
        { 
            this.targetPropertyPath = targetPropertyPath;
            this.targetPropertyValue = targetPropertyValue;
        }
    }
}