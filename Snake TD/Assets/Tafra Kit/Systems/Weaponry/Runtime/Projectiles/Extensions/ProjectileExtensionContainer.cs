using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Weaponry
{
    [System.Serializable]
    public class ProjectileExtensionContainer
    {
        [SerializeReference] private List<ProjectileExtension> extensions = new List<ProjectileExtension>();

        public List<ProjectileExtension> Extensions => extensions;
    }
}