using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TafraKit
{
    [CreateAssetMenu(menuName = "Tafra Kit/Scriptables/String", fileName = "Scriptable String")]
    public class ScriptableString : ScriptableVariable<string>
    {
        [NonSerialized] private bool defaultValueHashGenerated;
        [NonSerialized] private int defaultValueHash;

        protected override string GetSavedValue()
        {
            return PlayerPrefs.GetString(ID + "_STRING_SV_VALUE", defaultValue);
        }

        protected override void OnSavedValue()
        {
            PlayerPrefs.SetString(ID + "_STRING_SV_VALUE", currentValue);
        }

        public int GetDefaultValueHash()
        {
            if(!defaultValueHashGenerated)
            {
                defaultValueHash = Animator.StringToHash(defaultValue);
                defaultValueHashGenerated = true;
            }

            return defaultValueHash;
        }
    }
}