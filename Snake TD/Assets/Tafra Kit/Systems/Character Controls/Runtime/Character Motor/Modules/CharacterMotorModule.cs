using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.ModularSystem;

namespace TafraKit.CharacterControls
{
    [System.Serializable]
    public abstract class CharacterMotorModule : InternalModule
    {
        protected CharacterMotor motor;

        public void Initialize(CharacterMotor motor)
        {
            this.motor = motor;

            OnInitialize();
        }
    }
}