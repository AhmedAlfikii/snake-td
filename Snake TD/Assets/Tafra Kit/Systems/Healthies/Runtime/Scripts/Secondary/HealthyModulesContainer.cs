using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Healthies
{
    [System.Serializable]
    public class HealthyModulesContainer
    {
        [SerializeReference] private List<HealthyModule> modules = new List<HealthyModule>();

        public List<HealthyModule> Modules => modules;
    }
}