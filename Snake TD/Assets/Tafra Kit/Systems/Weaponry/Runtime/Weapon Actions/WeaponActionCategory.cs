using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Weaponry
{
    [Serializable]
    public class WeaponActionCategory
    {
        [SerializeField] private TafraString categoryName;
        [SerializeReferenceListContainer("actions", true, "Action", "Actions", 1)]
        [SerializeField] private WeaponActionsContainer actionContainer;

        public WeaponAction Action
        {
            get 
            { 
                if (actionContainer.Actions.Count > 0)
                    return actionContainer.Actions[0];

                return null;
            }
        }
    }
}