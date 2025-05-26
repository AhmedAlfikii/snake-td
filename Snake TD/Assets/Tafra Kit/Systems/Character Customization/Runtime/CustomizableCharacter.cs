using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

namespace TafraKit.CharacterCustomization
{
    public class CustomizableCharacter : MonoBehaviour
    {
        [Tooltip("The first bone in the skeleton hierarchy. Will be used to extract the full hierarchy of bones.")]
        [SerializeField] private Transform rootBone;
        [SerializeField] private MeshCustomization[] baseMeshCustomizattions;
        [Tooltip("The capsule colliders of the bones that should interact with the cloth in any mesh customization.")]
        [SerializeField] private CapsuleCollider[] clothBoneColliders;

        [Space()]

        [SerializeField] private bool applyBaseCustomizationsOnAwake = true;

        [Header("Events")]
        [SerializeField] private UnityEvent<SkinnedMeshRenderer> onCustomizationApplied;

        private Dictionary<string, Transform> bones = new Dictionary<string, Transform>();
        private Dictionary<string, Customization> baseCustomizationPerCategory = new Dictionary<string, Customization>();
        private Dictionary<string, Customization> customizationPerCategory = new Dictionary<string, Customization>();
        private Dictionary<string, List<GameObject>> meshCustomizationGOsPerCategory = new Dictionary<string, List<GameObject>>();

        public UnityEvent<SkinnedMeshRenderer> OnCustomizationApplied => onCustomizationApplied;

        private void Awake()
        {
            //Fill the bones dictionary with the entire bones hierarchy.
            ZHelper.FillDictionaryWithHierarchy(rootBone, bones);

            for(int i = 0; i < baseMeshCustomizattions.Length; i++)
            {
                if(!baseCustomizationPerCategory.TryAdd(baseMeshCustomizattions[i].Category, baseMeshCustomizattions[i]))
                {
                    TafraDebugger.Log("Customizable Character", 
                        $"Failed to add the base customization \"{baseMeshCustomizattions[i].name}\". Only one customization of the same category ({baseMeshCustomizattions[i].Category}) can be used as base.",
                        TafraDebugger.LogType.Error);

                    continue;
                }

                if (applyBaseCustomizationsOnAwake)
                    ApplyMeshCustomization(baseMeshCustomizattions[i]);
            }
        }

        public void ApplyBaseCustomizations()
        {
            for(int i = 0; i < baseMeshCustomizattions.Length; i++)
            {
                ApplyMeshCustomization(baseMeshCustomizattions[i]);
            }
        }

        public GameObject ApplyMeshCustomization(MeshCustomization customization)
        {
            string category = customization.Category;

            if(!customizationPerCategory.ContainsKey(category))
                customizationPerCategory.Add(category, customization);
            else
            {
                if(customizationPerCategory[category] != customization)
                    RemoveMeshCustomization(customization.Category, false);
                else
                {
                    TafraDebugger.Log("Customizable Character", $"This customization is already applied ({customization.name}), no need to apply it again.", TafraDebugger.LogType.Verbose);
                    return null;
                }
            }

            if(!meshCustomizationGOsPerCategory.ContainsKey(category))
                meshCustomizationGOsPerCategory.Add(category, new List<GameObject>());
            else if(meshCustomizationGOsPerCategory[category] == null)
                meshCustomizationGOsPerCategory[category] = new List<GameObject>();

            GameObject custPrefab = customization.CustomizationPrefab;
            
            if (custPrefab == null)
                return null;

            GameObject custGO = Instantiate(custPrefab, transform);

            meshCustomizationGOsPerCategory[category].Add(custGO);

            CustomizationExtraBonesData extraBonesData = customization.ExtraBonesData;
            Dictionary<string, Transform> extraBones = new Dictionary<string, Transform>();

            if(extraBonesData.Exists)
            {
                for (int i = 0; i < extraBonesData.BoneChains.Count; i++)
                {
                    CustomizationExtraBonesChain extraBonesChain = extraBonesData.BoneChains[i];

                    //This chain's root extra bone has the same sibiling index as the chain itself in the extra bones data.
                    //Since we remove them, then the next child chain will always be at element 0.
                    Transform rootExtraBone = custGO.transform.GetChild(0);

                    rootExtraBone.SetParent(bones[extraBonesChain.ParentBoneName]);
                    rootExtraBone.SetLocalPositionAndRotation(extraBonesChain.LocalPosition, extraBonesChain.LocalRotation);

                    meshCustomizationGOsPerCategory[category].Add(rootExtraBone.gameObject);

                    ZHelper.FillDictionaryWithHierarchy(rootExtraBone, extraBones);
                }
            }

            SkinnedMeshRenderer smr = custGO.GetComponent<SkinnedMeshRenderer>();

            if(smr != null)
                smr.rootBone = bones[customization.RootBoneName];

            List<Transform> boneTransforms = new List<Transform>();
            string[] boneNames = customization.BoneNames;

            for(int i = 0; i < boneNames.Length; i++)
            {
                string boneName = boneNames[i];

                if(bones.TryGetValue(boneName, out Transform bone))
                    boneTransforms.Add(bone);
                else if(extraBones.TryGetValue(boneName, out Transform extraBone))
                    boneTransforms.Add(extraBone);
                else
                    TafraDebugger.Log("Customizable Character", $"The bone \"{boneName}\" required by customization \"{customization.name}\" was not found in the character's main skeleton or the extra bones that came with the mesh customization.", TafraDebugger.LogType.Error);
            }

            if (smr != null)
                smr.bones = boneTransforms.ToArray();

            //Assign Bone Colliders to Cloth component if exists
            if(custGO.TryGetComponent(out Cloth cloth))
            {
                cloth.capsuleColliders = clothBoneColliders;
                cloth.enabled = true;
            }

            onCustomizationApplied?.Invoke(smr);
            
            return custGO;
        }

        public void RemoveMeshCustomization(string category, bool applyDefaultIfFound = true)
        {
            //Remove the customization scriptable object reference in the customizations dictionary.
            if(customizationPerCategory.ContainsKey(category))
                customizationPerCategory[category] = null;

            meshCustomizationGOsPerCategory.TryGetValue(category, out var customizationGOs);

            if(customizationGOs != null)
            {
                for(int i = 0; i < customizationGOs.Count; i++)
                {
                    Destroy(customizationGOs[i]);
                }
                customizationGOs.Clear();
            }

            if (applyDefaultIfFound && baseCustomizationPerCategory.TryGetValue(category, out var baseCustomization) && baseCustomization != null)
                ApplyMeshCustomization(baseCustomization as MeshCustomization);
        }
    }
}