using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.CharacterCustomization
{
    public abstract class Customization : ScriptableObject
    {
        [Tooltip("The name of the category this customization is part of. Only one customization can be applied per category.")]
        [SerializeField] protected TafraString category;

        public string Category => category.Value;
    }
}