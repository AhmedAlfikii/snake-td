using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.ContentManagement;
using UnityEngine;

namespace TafraKit
{
    [Serializable]
    public class TafraFloat : TafraVariable<float, ScriptableFloat>
    {
        public TafraFloat() { }
        public TafraFloat(float value) : base(value) { }
        public TafraFloat(float value, TafraAsset<ScriptableFloat> scriptableObjectAsset) : base(value, scriptableObjectAsset) { }

        public virtual int ValueInt => Mathf.RoundToInt(Value);
    }
}