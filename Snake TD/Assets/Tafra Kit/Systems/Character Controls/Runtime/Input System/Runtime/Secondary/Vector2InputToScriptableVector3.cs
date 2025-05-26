using System;
using UnityEngine;

namespace TafraKit.InputSystem
{
    public class Vector2InputToScriptableVector3 : MonoBehaviour
    {
        #if TAFRA_INPUT_SYSTEM
        [SerializeField] private Vector2InputCast inputCast;
        #endif

        [SerializeField] private ScriptableVector3 scriptableVector3;

        private void OnEnable()
        {
            #if TAFRA_INPUT_SYSTEM
            inputCast.performed.AddListener(OnPerformed);
            #endif
        }
        private void OnDisable()
        {
            #if TAFRA_INPUT_SYSTEM
            inputCast.performed.RemoveListener(OnPerformed);
            #endif
        }
        private void OnPerformed(Vector2 input)
        {
            scriptableVector3.Set(new Vector3(input.x, 0, input.y));
        }
    }
}