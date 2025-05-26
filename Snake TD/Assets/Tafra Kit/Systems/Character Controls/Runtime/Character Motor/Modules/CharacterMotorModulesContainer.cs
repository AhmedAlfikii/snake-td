using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.CharacterControls
{
    [System.Serializable]
    public class CharacterMotorModulesContainer
    {
        [SerializeReference] private List<CharacterMotorModule> modules = new List<CharacterMotorModule>();

        public List<CharacterMotorModule> Modules => modules;
    }
}