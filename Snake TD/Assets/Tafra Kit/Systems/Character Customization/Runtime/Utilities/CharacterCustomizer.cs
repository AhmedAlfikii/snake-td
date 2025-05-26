using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.CharacterCustomization
{
    public class CharacterCustomizer : MonoBehaviour
    {
        #region Classes, Structs & Enums
        [Serializable]
        public class CustomizationGroupData
        {
            public int defaultCustomizationIndex = -1;
            [HideInInspector] public int curCustomizationIndex = -1;
        }
        [Serializable]
        public class GenericCustomizationGroupData<T> : CustomizationGroupData where T : CustomizationGroup
        {
            public T group;
        }

        [Serializable]
        public class MeshCustomizationGroupData : GenericCustomizationGroupData<MeshCustomizationGroup>
        {
        }
        #endregion

        [SerializeField] private CustomizableCharacter character;
        [SerializeField] private MeshCustomizationGroupData[] meshGroups;

        private void Start()
        {
            for(int i = 0; i < meshGroups.Length; i++)
            {
                if(meshGroups[i].defaultCustomizationIndex > -1)
                    ApplyMeshCustomization(i, meshGroups[i].defaultCustomizationIndex);
                else
                    meshGroups[i].curCustomizationIndex = -1;
            }
        }
        public void ApplyMeshCustomization(int groupIndex, int customizationIndex)
        {
            MeshCustomizationGroupData groupData = meshGroups[groupIndex];

            character.ApplyMeshCustomization(groupData.group.Customizations[customizationIndex]);
            groupData.curCustomizationIndex = customizationIndex;
        }
        public void ApplyNextMeshCustomizationInGroup(int groupIndex)
        {
            MeshCustomizationGroupData groupData = meshGroups[groupIndex];

            int nextCustomizationIndex = (groupData.curCustomizationIndex + 1) % groupData.group.Customizations.Length;

            ApplyMeshCustomization(groupIndex, nextCustomizationIndex);
        }
        public void ApplyPreviousMeshCustomizationInGroup(int groupIndex)
        {
            MeshCustomizationGroupData groupData = meshGroups[groupIndex];

            int nextCustomizationIndex = groupData.curCustomizationIndex - 1;

            if (nextCustomizationIndex < 0)
                nextCustomizationIndex = groupData.group.Customizations.Length - 1;

            ApplyMeshCustomization(groupIndex, nextCustomizationIndex);
        }
    }
}