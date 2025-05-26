using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.CharacterCustomization
{
    [System.Serializable]
    public struct CustomizationExtraBonesData
    {
        public bool Exists;
        public List<CustomizationExtraBonesChain> BoneChains;

        public CustomizationExtraBonesData(bool exists, List<CustomizationExtraBonesChain> boneChains)
        {
            Exists = exists;
            BoneChains = boneChains;
        }
    }
}