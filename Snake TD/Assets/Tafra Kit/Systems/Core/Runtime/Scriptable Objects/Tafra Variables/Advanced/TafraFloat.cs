using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.ContentManagement;
using UnityEngine;

namespace TafraKit
{
    [Serializable]
    public class TafraString : TafraVariable<string, ScriptableString>
    {
        public TafraString() { }
        public TafraString(string value) : base(value) { }
        public TafraString(string value, TafraAsset<ScriptableString> scriptableObjectAsset) : base(value, scriptableObjectAsset) { }
    }
}