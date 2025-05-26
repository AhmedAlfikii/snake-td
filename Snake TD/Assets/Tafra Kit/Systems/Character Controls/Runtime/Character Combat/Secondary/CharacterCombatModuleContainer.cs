using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.CharacterControls
{
    [System.Serializable]
    public class CharacterCombatModuleContainer
    {
        [SerializeReference] private List<CharacterCombatModule> modules = new List<CharacterCombatModule>();

        public List<CharacterCombatModule> Modules => modules;
    }
}