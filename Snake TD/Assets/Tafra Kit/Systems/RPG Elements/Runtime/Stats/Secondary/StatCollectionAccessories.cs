using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.ContentManagement;
using TafraKit.RPG;
using UnityEngine;

namespace TafraKit.Internal.RPGElements
{
    [Serializable]
    public class StatCollectionAccessories
    {
        [Tooltip("Serves no purpose but to nicefy the array element name.")]
        [SerializeField] private string name;
        [SerializeField] private TafraAsset<Stat> stat;
        [SerializeField] private TafraAsset<ScriptableFloat> output;
        [SerializeField] private List<ValueManipulator> manipulators;

        public TafraAsset<Stat> Stat => stat;
        public TafraAsset<ScriptableFloat> Output => output;
        public List<ValueManipulator> Manipulators => manipulators;
    }
}