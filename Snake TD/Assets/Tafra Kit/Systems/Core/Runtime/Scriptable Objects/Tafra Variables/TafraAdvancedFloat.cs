using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    [System.Serializable]
    public class TafraAdvancedFloat : TafraFloat
    {
        public enum FloatType
        { 
            Value,
            ScriptableObject,
            TafraFloatRange
        }
        [SerializeField] private FloatType floatType;
        [SerializeField] private TafraFloat min;
        [SerializeField] private TafraFloat max;
        [SerializeField] private bool randomizeSign;

        [Header("Operation")]
        [SerializeField] private bool performOperation;
        [SerializeField] private TafraFloat operationValue;
        [SerializeField] private NumberOperation operation;

        public TafraAdvancedFloat() { }
        public TafraAdvancedFloat(TafraAdvancedFloat otherAdvancedFloat) : base(otherAdvancedFloat.value) 
        {
            value = otherAdvancedFloat.value;
            scriptableVariable = otherAdvancedFloat.scriptableVariable;
            floatType = otherAdvancedFloat.floatType;
            min = new TafraFloat(otherAdvancedFloat.min.Value, otherAdvancedFloat.min.ScriptableVariableAsset);
            max = new TafraFloat(otherAdvancedFloat.max.Value, otherAdvancedFloat.max.ScriptableVariableAsset);
            randomizeSign = otherAdvancedFloat.randomizeSign;
            performOperation = otherAdvancedFloat.performOperation;
            operationValue = new TafraFloat(otherAdvancedFloat.operationValue.Value, otherAdvancedFloat.operationValue.ScriptableVariableAsset);
            operation = otherAdvancedFloat.operation;
        }
        public TafraAdvancedFloat(float value) : base(value) { }

        public FloatType MyType 
        {
            get
            {
                return floatType;
            }
            set
            { 
                floatType = value;
            }
        }

        public override float Value 
        {
            get
            {
                if (!isLoaded)
                    LoadAsset();

                float result = 0;
                switch(floatType)
                {
                    case FloatType.Value:
                        result = value;
                        break;
                    case FloatType.ScriptableObject:
                        result = scriptableVariable.Value;
                        break;
                    case FloatType.TafraFloatRange:
                        float val = Random.Range(min.Value, max.Value);
                        if (randomizeSign)
                            val *= Random.value > 0.5f ? 1f : -1f;
                        result = val;
                        break;
                    default:
                        result = value;
                        break;
                }

                if(performOperation)
                    result = ZHelper.PerformOperationOnNumber(result, operationValue.Value, operation);

                return result;
            }
            set 
            {
                if (!isLoaded)
                    LoadAsset();

                switch (floatType)
                {
                    case FloatType.Value:
                        this.value = value;
                        break;
                    case FloatType.ScriptableObject:
                        scriptableVariable.Set(value);
                        break;
                    case FloatType.TafraFloatRange:
                        #if UNITY_EDITOR
                        TafraDebugger.Log("Tafra Advanced Float", "Can't set the value of a range advanced float.", TafraDebugger.LogType.Error);
                        #endif
                        break;
                    default:
                        this.value = value;
                        break;
                }
            }
        }
        public override int ValueInt
        {
            get
            {
                if (!isLoaded)
                    LoadAsset();

                int result = 0;
                switch(floatType)
                {
                    case FloatType.Value:
                        result = Mathf.RoundToInt(value);
                        break;
                    case FloatType.ScriptableObject:
                        result = scriptableVariable.ValueInt;
                        break;
                    case FloatType.TafraFloatRange:
                        int val = Random.Range(min.ValueInt, max.ValueInt + 1);
                        if(randomizeSign)
                            val *= Random.value > 0.5f ? 1 : -1;
                        result = val;
                        break;
                    default:
                        result = Mathf.RoundToInt(value);
                        break;
                }

                if(performOperation)
                    result = Mathf.RoundToInt(ZHelper.PerformOperationOnNumber(result, operationValue.Value, operation));

                return result;
            }
        }
    }
}
