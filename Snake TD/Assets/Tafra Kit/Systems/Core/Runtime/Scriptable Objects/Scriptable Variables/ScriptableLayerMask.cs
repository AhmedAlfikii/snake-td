using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TafraKit
{
    [CreateAssetMenu(menuName = "Tafra Kit/Scriptables/Layer Mask", fileName = "Scriptable Layer Mask")]
    public class ScriptableLayerMask : ScriptableVariable<LayerMask>
    {
        public ScriptableLayerMask() 
        {
            autoSave = false;
        }

        protected override LayerMask GetSavedValue()
        {
            return defaultValue;
        }

        protected override void OnSavedValue()
        {

        }
    }
}