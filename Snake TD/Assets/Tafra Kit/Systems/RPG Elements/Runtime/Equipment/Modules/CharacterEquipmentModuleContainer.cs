using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.RPG
{
    [System.Serializable]
    public class CharacterEquipmentModuleContainer
    {
        [SerializeReference] private List<CharacterEquipmentModule> modules = new List<CharacterEquipmentModule>();

        public List<CharacterEquipmentModule> Modules => modules;
    }
}