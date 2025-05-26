using TafraKit.ModularSystem;
using UnityEngine;

namespace TafraKit
{
    [System.Serializable]
    public abstract class CharacterTestingModule : InternalModule
    {
        protected CharacterTesting characterTesting;

        public void Initialize(CharacterTesting characterTesting)
        { 
            this.characterTesting = characterTesting;

            OnInitialize();
        }
    }
}