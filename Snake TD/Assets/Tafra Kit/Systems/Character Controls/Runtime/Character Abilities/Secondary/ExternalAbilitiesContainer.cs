using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.CharacterControls
{
    [CreateAssetMenu(fileName = "External Abilities Container", menuName = "Tafra Kit/Character Controls/Abilities/External Abilities Container")]
    public class ExternalAbilitiesContainer : ScriptableObject
    {
        [SerializeField] private AbilitiesContainer container;

        public AbilitiesContainer Container => container;
    }
}