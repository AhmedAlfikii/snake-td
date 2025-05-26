using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.CharacterCustomization
{
    [CreateAssetMenu(fileName = "MeshCustomization", menuName = "Tafra Kit/Character Customization/Mesh Customization")]
    public class MeshCustomization : Customization
    {
        [Tooltip("The prefab that contains the mesh.")]
        [SerializeField] private GameObject customizationPrefab;
        [SerializeField] private string rootBoneName;
        [SerializeField] private string[] boneNames;                    //An array of bone names that the skinned mesh of this customization requires.
        [SerializeField] private CustomizationExtraBonesData extraBonesData;

        public GameObject CustomizationPrefab => customizationPrefab;
        public string RootBoneName => rootBoneName;
        public string[] BoneNames => boneNames;
        public CustomizationExtraBonesData ExtraBonesData => extraBonesData;

        #region Editor Related Functions
        #if UNITY_EDITOR
        public void EditorInitialize(GameObject customizationPrefab, string category, string rootBoneName, string[] boneNames, CustomizationExtraBonesData extraBonesData)
        {
            this.customizationPrefab = customizationPrefab;
            this.category = new TafraString(category, null);
            this.rootBoneName = rootBoneName;
            this.boneNames = boneNames;
            this.extraBonesData = extraBonesData;
        }
        #endif
        #endregion
    }
}