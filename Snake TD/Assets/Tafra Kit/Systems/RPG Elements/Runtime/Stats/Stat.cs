using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.RPG
{
    [CreateAssetMenu(menuName = "Tafra Kit/RPG/Stats/Stat", fileName = "Stat")]
    public class Stat : IdentifiableScriptableObject
    {
        [SerializeField] private bool isPercentage;

        public bool IsPercentage => isPercentage;
    }
}