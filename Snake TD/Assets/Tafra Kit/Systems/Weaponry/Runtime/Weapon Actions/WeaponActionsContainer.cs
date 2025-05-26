using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Weaponry
{
    [Serializable]
    public class WeaponActionsContainer
    {
        [SerializeReference] private List<WeaponAction> actions = new List<WeaponAction>();

        public List<WeaponAction> Actions => actions;
    }
}