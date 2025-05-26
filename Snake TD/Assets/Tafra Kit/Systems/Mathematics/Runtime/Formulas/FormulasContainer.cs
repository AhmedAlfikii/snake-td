using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.Internal.Mathematics;

namespace TafraKit.Mathematics
{
    [System.Serializable]
    public class FormulasContainer
    {
        [SerializeReference] private List<Formula> formulas = new List<Formula>();

        public float Evaluate(float x)
        {
            if(formulas.Count == 0)
                return 0;
            else if(formulas.Count == 1)
                return formulas[0].Evaluate(x);
            else
            {
                for(int i = 0; i < formulas.Count; i++)
                {
                    Formula equation = formulas[i];

                    if (equation.InputRange.IsInRange(x))
                        return equation.Evaluate(x);
                }

                return formulas[formulas.Count - 1].Evaluate(x);
            }
        }

        public List<Formula> GetEquations()
        {
            return formulas;
        }

        public void SetEquations(List<Formula> formulas)
        {
            this.formulas = formulas;
        }
    }
}