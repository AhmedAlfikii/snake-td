using System;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Internal
{
    [Serializable]
    public class LevelEndModulesContainer
    {
        [SerializeReference] private List<LevelEndModule> modules = new List<LevelEndModule>();

        public List<LevelEndModule> Modules => modules;
    }
}