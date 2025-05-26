using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    [System.Serializable]
    public class AbilitySystemObjectInitiailzersContainer
    {
        [SerializeReference] private List<AbilitySystemObjectInitiailzer> initializers = new List<AbilitySystemObjectInitiailzer>();

        public List<AbilitySystemObjectInitiailzer> Initializers => initializers;
    }
}