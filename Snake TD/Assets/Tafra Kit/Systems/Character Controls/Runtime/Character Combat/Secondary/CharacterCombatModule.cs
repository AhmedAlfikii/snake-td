using System.Collections;
using System.Collections.Generic;
using TafraKit.ModularSystem;
using UnityEngine;

namespace TafraKit.CharacterControls
{
    [System.Serializable]
    public abstract class CharacterCombatModule : InternalModule
    {
        protected CharacterCombat characterCombat;

        public void Initialize(CharacterCombat characterCombat)
        { 
            this.characterCombat = characterCombat;

            OnInitialize();
        }
    }
}