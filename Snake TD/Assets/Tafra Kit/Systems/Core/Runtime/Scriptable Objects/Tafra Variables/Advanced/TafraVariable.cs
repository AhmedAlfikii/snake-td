using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.ContentManagement;

namespace TafraKit
{
    [Serializable]
    public class TafraVariable<VariableType, ScriptableVariableType> where ScriptableVariableType : ScriptableVariable<VariableType>
    {
        /// <summary>
        /// Used for the custom property drawer only. Intended to change the array element name.
        /// </summary>
        [SerializeField] protected VariableType editorDisplayValue;
        [SerializeField] protected TafraVariableValueType valueType;
        [SerializeField] protected VariableType value;
        [SerializeField] protected TafraAsset<ScriptableVariableType> scriptableVariableAsset = new TafraAsset<ScriptableVariableType>();

        [NonSerialized] protected bool isLoaded;
        [NonSerialized] protected ScriptableVariableType scriptableVariable;

        public TafraVariable() { }
        public TafraVariable(VariableType value)
        {
            this.value = value;
        }
        public TafraVariable(VariableType value, TafraAsset<ScriptableVariableType> scriptableObjectAsset)
        {
            this.value = value;
            this.scriptableVariableAsset = scriptableObjectAsset;
        }

        public virtual VariableType Value
        {
            get
            {
                if(!isLoaded)
                    LoadAsset();

                if (valueType == TafraVariableValueType.Value)
                    return value;
                else
                    return scriptableVariable.Value;
            }
            set
            {
                if(!isLoaded)
                    LoadAsset();

                if (valueType == TafraVariableValueType.Value)
                    this.value = value;
                else
                    scriptableVariable.Set(value);
            }
        }
        public ScriptableVariableType ScriptableVariable {
            get 
            {
                if(!isLoaded)
                    LoadAsset();

                return scriptableVariable;
            }
        }
        public TafraAsset<ScriptableVariableType> ScriptableVariableAsset => scriptableVariableAsset;

        public void LoadAsset()
        {
            if(isLoaded)
                return;

            if (scriptableVariableAsset.HasReference)
                scriptableVariable = scriptableVariableAsset.Load();

            isLoaded = true;
        }
        public void UnloadAsset()
        {
            if(!isLoaded)
                return;

            scriptableVariableAsset.Release();

            isLoaded = false;
        }
    }
}