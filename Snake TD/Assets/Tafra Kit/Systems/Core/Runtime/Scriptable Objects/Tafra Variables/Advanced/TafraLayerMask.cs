using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.ContentManagement;
using UnityEngine;

namespace TafraKit
{
    [Serializable]
    public class TafraLayerMask : TafraVariable<LayerMask, ScriptableLayerMask>
    {
        public TafraLayerMask() { }
        public TafraLayerMask(LayerMask value) : base(value) { }
        public TafraLayerMask(LayerMask value, TafraAsset<ScriptableLayerMask> scriptableObjectAsset) : base(value, scriptableObjectAsset) { }
    }
}