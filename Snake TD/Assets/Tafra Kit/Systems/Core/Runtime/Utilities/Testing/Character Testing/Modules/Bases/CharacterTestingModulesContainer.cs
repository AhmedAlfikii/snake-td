using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    [System.Serializable]
    public class CharacterTestingModulesContainer
    {
        [SerializeReference] private List<CharacterTestingModule> modules = new List<CharacterTestingModule>();

        public List<CharacterTestingModule> Modules => modules;
    }
}