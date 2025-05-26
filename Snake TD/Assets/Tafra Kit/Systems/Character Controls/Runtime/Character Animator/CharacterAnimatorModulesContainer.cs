using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.CharacterControls
{
    [System.Serializable]
    public class CharacterAnimatorModulesContainer
    {
        [SerializeReference] private List<CharacterAnimatorModule> modules = new List<CharacterAnimatorModule>();

        public List<CharacterAnimatorModule> Modules => modules;
    }
}