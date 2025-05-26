using System;
using TafraKit.Mathematics;
using UnityEngine;

namespace TafraKit.Internal
{
    public class ScriptableFloatsLink : MonoBehaviour
    {
        [SerializeField] private ScriptableFloat master;
        [SerializeField] private ScriptableFloat follower;
        [SerializeField] private FormulasContainer formula;

        private void OnEnable()
        {
            UpdateDifficulty();

            master.OnValueChange.AddListener(OnClearsCountValueChange);
        }
        private void OnDisable()
        {
            master.OnValueChange.RemoveListener(OnClearsCountValueChange);
        }

        private void OnClearsCountValueChange(float newValue)
        {
            UpdateDifficulty();
        }

        private void UpdateDifficulty()
        {
            follower.Set(formula.Evaluate(master.Value));
        }
    }
}