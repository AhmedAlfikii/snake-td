using System;
using UnityEngine;

namespace TafraKit.Mathematics
{
    [Serializable]
    public class FormulasContainerWithInput
    {
        [SerializeField] private ScriptableFloat input;
        [SerializeField] private FormulasContainer formulas;

        public FormulasContainerWithInput(ScriptableFloat input, FormulasContainer formulas)
        {
            this.input = input;
            this.formulas = formulas;
        }

        public float Evaluate()
        {
            return formulas.Evaluate(input.Value);
        }
        public float EvaluateWithDifferentInput(float newInput)
        {
            return formulas.Evaluate(newInput);
        }
    }
}

