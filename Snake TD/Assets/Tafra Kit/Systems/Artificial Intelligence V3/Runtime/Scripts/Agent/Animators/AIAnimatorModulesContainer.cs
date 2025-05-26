using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.AI3
{
    [System.Serializable]
    public class AIAnimatorModulesContainer
    {
        [SerializeReference] private List<AIAnimatorModule> modules = new List<AIAnimatorModule>();

        public List<AIAnimatorModule> Modules => modules;
    }
}