using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Healthies
{
    [System.Serializable]
    public class HealthBarModulesContainer
    {
        [SerializeReference] private List<HealthBarModule> modules =new List<HealthBarModule>();

        public List<HealthBarModule> Modules => modules;
    }
}