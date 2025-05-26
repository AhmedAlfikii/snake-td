using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.CharacterCustomization
{
    public abstract class GenericCustomizationGroup<T> : CustomizationGroup where T : Customization
    {
        [SerializeField] protected T[] customizations;

        public T[] Customizations => customizations;
    }
}