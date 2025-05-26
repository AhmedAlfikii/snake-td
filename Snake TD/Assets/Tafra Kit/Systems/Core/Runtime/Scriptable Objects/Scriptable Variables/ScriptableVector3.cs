using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TafraKit
{
    [CreateAssetMenu(menuName = "Tafra Kit/Scriptables/Vector3", fileName = "Scriptable Vector3")]
    public class ScriptableVector3 : ScriptableVariable<Vector3>
    {
        public ScriptableVector3() 
        {
            autoSave = false;
        }

        protected override Vector3 GetSavedValue()
        {
            return defaultValue;
        }

        protected override void OnSavedValue()
        {

        }
    }
}