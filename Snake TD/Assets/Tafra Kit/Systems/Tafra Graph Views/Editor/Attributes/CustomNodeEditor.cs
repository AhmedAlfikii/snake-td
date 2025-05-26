using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKitEditor.GraphViews
{
    /// <summary>
    /// Add this attribute to node editors in order to identify which node type this editor is created for.
    /// </summary>
    public class CustomNodeEditor : Attribute
    {
        private Type nodeType;

        public Type NodeType => nodeType;

        public CustomNodeEditor(Type nodeType)
        {
            this.nodeType = nodeType;
        }
    }
}
