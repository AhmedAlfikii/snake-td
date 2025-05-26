using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class InspectorDisplayNameAttribute : PropertyAttribute
    {
        public string DisplayName { get; private set; }

        /// <summary>
        /// Change the field's display name in the inspector.
        /// </summary>
        /// <param name="displayName"></param>
        public InspectorDisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}