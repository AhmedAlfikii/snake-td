using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using System;
using TafraKit.CharacterCustomization;

namespace TafraKitEditor.CharacterCustomization
{
    public class MeshCustomizationCreationWindow : EditorWindow
    {
        private int selectionCount;
        private bool isSuitableSelection = true;
        private int suitableSelectionsCount;
        private ListView extraBoneChainsListView;
        private List<Transform> extraBoneChains = new List<Transform>();

        [MenuItem("Tafra Games/Windows/Mesh Customization Creator", priority = 20)]
        public static void OpenWindow()
        {
            //GetWindowWithRect<MeshCustomizationCreationWindow>(new Rect(0, 0, 350, 159), false, "Mesh Customization");
            GetWindow<MeshCustomizationCreationWindow>(false, "Mesh Customization");
        }

        private void OnEnable()
        {
            VisualTreeAsset uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Tafra Kit/UI Toolkit Assets/UXML/Character Customization/MeshCustomizationCreationWindow.uxml");
            uxml.CloneTree(rootVisualElement);

            VisualElement mainPanel = rootVisualElement.Q<VisualElement>("MainPanel");

            Button createButton = rootVisualElement.Q<Button>("CreateButton_Active");
            createButton.clicked += Create;
            
            Button dimmedCreateButton = rootVisualElement.Q<Button>("CreateButton_Dimmed");
            dimmedCreateButton.SetEnabled(false);

            ObjectField extraBonesDimmedField = rootVisualElement.Q<ObjectField>("ExtraBonesDimmedField");
            extraBonesDimmedField.SetEnabled(false);

            extraBoneChainsListView = DrawReorderableList(extraBoneChains, "Extra Bone Chains", rootVisualElement, true);
            mainPanel.Add(extraBoneChainsListView);

            extraBoneChainsListView.PlaceBehind(extraBonesDimmedField);

            EditorApplication.update += OnEditorUpdate;
        } 
        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
        }

        public static ListView DrawReorderableList<T>(List<T> sourceList, string title, VisualElement rootVisualElement, bool allowSceneObjects = true) where T : UnityEngine.Object
        {
            ListView list = new ListView(sourceList)
            {
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                showFoldoutHeader = true,
                headerTitle = title,
                showAddRemoveFooter = true,
                reorderMode = ListViewReorderMode.Animated,
                reorderable = true,
                makeItem = () => new ObjectField
                {
                    objectType = typeof(T),
                    allowSceneObjects = allowSceneObjects
                },
                bindItem = (element, i) =>
                {
                    ObjectField field = (ObjectField)element;

                    field.value = sourceList[i];
                    field.RegisterValueChangedCallback((value) =>
                    {
                        sourceList[i] = (T)value.newValue;
                    });
                }
            };
            return list;
        }

        private void OnEditorUpdate()
        {
            selectionCount = Selection.gameObjects.Length;

            suitableSelectionsCount = 0;

            for(int i = 0; i < selectionCount; i++)
            {
                if(Selection.gameObjects[i].GetComponent<SkinnedMeshRenderer>())
                    suitableSelectionsCount++;
            }

            Label selectedCountLabel = rootVisualElement.Query<Label>("SelectedCount");
            selectedCountLabel.text = $"Selected Skinned Renderers: <b>{suitableSelectionsCount}</b>";

            VisualElement noSelectionElement = rootVisualElement.Query<VisualElement>("NoSelectionElements");
            VisualElement suitableSelectionElements = rootVisualElement.Query<VisualElement>("SuitableSelectionElements");

            if(suitableSelectionsCount > 0 && !isSuitableSelection)
            {
                noSelectionElement.style.display = DisplayStyle.None;
                suitableSelectionElements.style.display = DisplayStyle.Flex;

                isSuitableSelection = true;
            }
            else if(suitableSelectionsCount == 0 && isSuitableSelection)
            {
                noSelectionElement.style.display = DisplayStyle.Flex;
                suitableSelectionElements.style.display = DisplayStyle.None;

                isSuitableSelection = false;
            }

            ObjectField extraBonesField = rootVisualElement.Query<ObjectField>("ExtraBonesField");
            ObjectField extraBonesDimmedField = rootVisualElement.Query<ObjectField>("ExtraBonesDimmedField");

            if(suitableSelectionsCount > 1)
            {
                extraBonesField.style.display = DisplayStyle.None;
                extraBonesDimmedField.style.display = DisplayStyle.Flex;
            }
            else
            {
                extraBonesField.style.display = DisplayStyle.Flex;
                extraBonesDimmedField.style.display = DisplayStyle.None;
            }
        }
        private void Create()
        {
            bool isSingleSelection = suitableSelectionsCount == 1;

            TextField categoryField = rootVisualElement.Q<TextField>("CategoryField");
            string category = categoryField.text;
            
            //Get the path folder to save the assets in.
            string folderPath = $"{rootVisualElement.Q<TextField>("PathField").text}";
            int createdCustomizations = 0;

            for(int i = 0; i < Selection.gameObjects.Length; i++)
            {
                GameObject selectedGO = Selection.gameObjects[i];
                SkinnedMeshRenderer smr = selectedGO.GetComponent<SkinnedMeshRenderer>();
                if(smr)
                {
                    createdCustomizations++;

                    //Get the Skinned Mesh's bone names.
                    Transform rootBone = smr.rootBone;
                    if(rootBone == null)
                    {
                        Debug.LogError($"The Skinned Mesh Renderer ({selectedGO.name}) you're trying to create a customization of doesn't have a root bone. A customization asset was not created for it.");
                        continue;
                    }
                    string rootBoneName = smr.rootBone.name;
                    List<string> boneNames = new List<string>();

                    for(int j = 0; j < smr.bones.Length; j++)
                    {
                        boneNames.Add(smr.bones[j].name);
                    }

                    //Create a prefab
                    GameObject meshGO = Instantiate(selectedGO);
                    meshGO.name = selectedGO.name;

                    //Instantiate a copy of each extra bone chain.
                    if(isSingleSelection && extraBoneChains.Count > 0)
                    {
                        for (int j = 0; j < extraBoneChains.Count; j++)
                        {
                            GameObject eBonesGO = Instantiate(extraBoneChains[j].gameObject, meshGO.transform);
                            eBonesGO.name = extraBoneChains[j].name;
                        }
                    }

                    //The asset path without the extention.
                    string assetPath = $"{folderPath}/{selectedGO.name}";

                    GameObject meshPrefab = PrefabUtility.SaveAsPrefabAsset(meshGO, $"{assetPath}.prefab");

                    DestroyImmediate(meshGO);

                    //Create the Scriptable Object that will be used for customization.
                    MeshCustomization meshCustomizationSO = CreateInstance<MeshCustomization>();

                    CustomizationExtraBonesData extraBonesData = new CustomizationExtraBonesData();

                    if (isSingleSelection && extraBoneChains.Count > 0)
                    {
                        extraBonesData.Exists = true;

                        extraBonesData.BoneChains = new List<CustomizationExtraBonesChain>();

                        for (int j = 0; j < extraBoneChains.Count; j++)
                        {
                            Transform extraBonesRoot = extraBoneChains[j];
                            CustomizationExtraBonesChain chain = new CustomizationExtraBonesChain()
                            {
                                ParentBoneName = extraBonesRoot.parent.name,
                                LocalPosition = extraBonesRoot.localPosition,
                                LocalRotation = extraBonesRoot.localRotation
                            };
                            extraBonesData.BoneChains.Add(chain);
                        }
                    }

                    meshCustomizationSO.EditorInitialize(meshPrefab, category, rootBoneName, boneNames.ToArray(), extraBonesData);

                    AssetDatabase.CreateAsset(meshCustomizationSO, $"{assetPath}.asset");

                    if(createdCustomizations == suitableSelectionsCount)
                    {
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath<MeshCustomization>($"{assetPath}.asset");
                        EditorGUIUtility.PingObject(meshCustomizationSO);
                    }
                }
            }

            extraBoneChainsListView.Clear();
            categoryField.value = "";
        }
        private void CreateGroup()
        {
            bool isSingleSelection = suitableSelectionsCount == 1;

            TextField categoryField = rootVisualElement.Q<TextField>("CategoryField");
            string category = categoryField.text;
            
            //Get the path folder to save the assets in.
            string folderPath = $"{rootVisualElement.Q<TextField>("PathField").text}";
            int createdCustomizations = 0;

            for(int i = 0; i < Selection.gameObjects.Length; i++)
            {
                GameObject selectedGO = Selection.gameObjects[i];
                SkinnedMeshRenderer smr = selectedGO.GetComponent<SkinnedMeshRenderer>();
                if(smr)
                {
                    createdCustomizations++;

                    //Get the Skinned Mesh's bone names.
                    Transform rootBone = smr.rootBone;
                    if(rootBone == null)
                    {
                        Debug.LogError($"The Skinned Mesh Renderer ({selectedGO.name}) you're trying to create a customization of doesn't have a root bone. A customization asset was not created for it.");
                        continue;
                    }
                    string rootBoneName = smr.rootBone.name;
                    List<string> boneNames = new List<string>();

                    for(int j = 0; j < smr.bones.Length; j++)
                    {
                        boneNames.Add(smr.bones[j].name);
                    }

                    //Create a prefab
                    GameObject meshGO = Instantiate(selectedGO);
                    meshGO.name = selectedGO.name;

                    //Instantiate a copy of each extra bone chain.
                    if(isSingleSelection && extraBoneChains.Count > 0)
                    {
                        for (int j = 0; j < extraBoneChains.Count; j++)
                        {
                            GameObject eBonesGO = Instantiate(extraBoneChains[j].gameObject, meshGO.transform);
                            eBonesGO.name = extraBoneChains[j].name;
                        }
                    }

                    //The asset path without the extention.
                    string assetPath = $"{folderPath}/{selectedGO.name}";

                    GameObject meshPrefab = PrefabUtility.SaveAsPrefabAsset(meshGO, $"{assetPath}.prefab");

                    DestroyImmediate(meshGO);

                    //Create the Scriptable Object that will be used for customization.
                    MeshCustomization meshCustomizationSO = CreateInstance<MeshCustomization>();

                    CustomizationExtraBonesData extraBonesData = new CustomizationExtraBonesData();

                    if (isSingleSelection && extraBoneChains.Count > 0)
                    {
                        extraBonesData.Exists = true;

                        extraBonesData.BoneChains = new List<CustomizationExtraBonesChain>();

                        for (int j = 0; j < extraBoneChains.Count; j++)
                        {
                            Transform extraBonesRoot = extraBoneChains[j];
                            CustomizationExtraBonesChain chain = new CustomizationExtraBonesChain()
                            {
                                ParentBoneName = extraBonesRoot.parent.name,
                                LocalPosition = extraBonesRoot.localPosition,
                                LocalRotation = extraBonesRoot.localRotation
                            };
                            extraBonesData.BoneChains.Add(chain);
                        }
                    }

                    meshCustomizationSO.EditorInitialize(meshPrefab, category, rootBoneName, boneNames.ToArray(), extraBonesData);

                    AssetDatabase.CreateAsset(meshCustomizationSO, $"{assetPath}.asset");

                    if(createdCustomizations == suitableSelectionsCount)
                    {
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath<MeshCustomization>($"{assetPath}.asset");
                        EditorGUIUtility.PingObject(meshCustomizationSO);
                    }
                }
            }

            extraBoneChainsListView.Clear();
            categoryField.value = "";
        }
    }
}