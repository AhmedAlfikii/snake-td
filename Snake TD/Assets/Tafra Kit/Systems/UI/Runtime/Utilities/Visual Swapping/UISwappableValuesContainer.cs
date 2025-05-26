using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.UI
{
    [System.Serializable]
    public class UISwappableValuesContainer
    {
        [SerializeReference] private UISwappableValues[] swappableValues = new UISwappableValues[] { };

        public UISwappableValues[] SwappableValues => swappableValues;
    }
}