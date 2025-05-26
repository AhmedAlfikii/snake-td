using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    [System.Serializable]
    public class TafraActorModulesContainer
    {
        [SerializeReference] private List<TafraActorModule> modules = new List<TafraActorModule>();

        public List<TafraActorModule> Modules => modules;
    }
}