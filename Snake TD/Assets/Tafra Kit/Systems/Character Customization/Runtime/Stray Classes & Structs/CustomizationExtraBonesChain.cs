using UnityEngine;

namespace TafraKit.CharacterCustomization
{
    [System.Serializable]
    public struct CustomizationExtraBonesChain
    {
        public string ParentBoneName;
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;

        public CustomizationExtraBonesChain(string parentBoneName, Vector3 localPosition, Quaternion localRotation)
        {
            ParentBoneName = parentBoneName;
            LocalPosition = localPosition;
            LocalRotation = localRotation;
        }
    }
}