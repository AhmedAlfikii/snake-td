using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Text;

namespace TafraKitEditor
{
    public class MaterialsControllerWindow : EditorWindow
    {
        private static Toggle logIndividualMaterialsField;
        private static Toggle logIndividualShadersField;
        private static Toggle logGameObjectsField;

        [MenuItem("Tafra Games/Windows/Game Health/Materials Controller")]
        private static void Open()
        {
            GetWindow<MaterialsControllerWindow>("Materials Controller");
        }

        private void CreateGUI()
        {
            rootVisualElement.style.paddingTop = rootVisualElement.style.paddingBottom = rootVisualElement.style.paddingRight = rootVisualElement.style.paddingLeft = 2;

            #region Scene Analysis
            Label sceneAnalysisHeader = new Label("Scene Analysis");
            sceneAnalysisHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
            sceneAnalysisHeader.style.fontSize = 14;
            sceneAnalysisHeader.style.marginTop = 5;
            sceneAnalysisHeader.style.marginBottom = 5;

            logIndividualMaterialsField = new Toggle("Log Individual Materials");
            logIndividualMaterialsField.tooltip = "If true, a log will be printed for each unique material found, clicking that log will highlight the material in the project window";
            logIndividualMaterialsField.value = EditorPrefs.GetBool("MATERIALS_CONTRLLLER_WINDOW_LOG_INDIVIDUAL_MATERIALS", false);
            logIndividualMaterialsField.RegisterValueChangedCallback((ev) =>
            {
                EditorPrefs.SetBool("MATERIALS_CONTRLLLER_WINDOW_LOG_INDIVIDUAL_MATERIALS", ev.newValue);
            });

            logIndividualShadersField = new Toggle("Log Individual Shaders");
            logIndividualShadersField.tooltip = "If true, a log will be printed for each unique shader found";
            logIndividualShadersField.value = EditorPrefs.GetBool("MATERIALS_CONTRLLLER_WINDOW_LOG_INDIVIDUAL_SHADERS", false);
            logIndividualShadersField.RegisterValueChangedCallback((ev) =>
            {
                EditorPrefs.SetBool("MATERIALS_CONTRLLLER_WINDOW_LOG_INDIVIDUAL_SHADERS", ev.newValue);
            });

            Button analyzeMaterialsButton = new Button(AnalyzeSceneMaterials);
            analyzeMaterialsButton.text = "Analyze Scene Materials";

            TextField targetShaderField = new TextField("Target Shader Full Name");
            targetShaderField.value = EditorPrefs.GetString("MATERIALS_CONTRLLLER_WINDOW_TARGET_SHADER_FULL_NAME", "Universal Render Pipeline/Lit");
            targetShaderField.RegisterValueChangedCallback((ev) =>
            {
                EditorPrefs.SetString("MATERIALS_CONTRLLLER_WINDOW_TARGET_SHADER_FULL_NAME", ev.newValue);
            });

            logGameObjectsField = new Toggle("Log Game Objects");
            logGameObjectsField.tooltip = "If true, a log will be printed for each unique game object that has a renderer with a material using the target shader.";
            logGameObjectsField.value = EditorPrefs.GetBool("MATERIALS_CONTRLLLER_WINDOW_LOG_GAME_OBJECTS", false);
            logGameObjectsField.RegisterValueChangedCallback((ev) =>
            {
                EditorPrefs.SetBool("MATERIALS_CONTRLLLER_WINDOW_LOG_GAME_OBJECTS", ev.newValue);
            });

            Button findRenderersButton = new Button(() => { FindRenderersUsingShader(targetShaderField.value); });
            findRenderersButton.text = "Find Renderers Using Target Shader";

            rootVisualElement.Add(sceneAnalysisHeader);
            rootVisualElement.Add(logIndividualMaterialsField);
            rootVisualElement.Add(logIndividualShadersField);
            rootVisualElement.Add(analyzeMaterialsButton);

            rootVisualElement.Add(targetShaderField);
            rootVisualElement.Add(logGameObjectsField);
            rootVisualElement.Add(findRenderersButton);
            #endregion

            #region Materials Conversion
            Label conversionHeader = new Label("Materials Conversion");
            conversionHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
            conversionHeader.style.fontSize = 14;
            conversionHeader.style.marginTop = 5;
            conversionHeader.style.marginBottom = 5;

            TextField originalShaderFullNameField = new TextField("Original Shader Full Name");
            originalShaderFullNameField.value = EditorPrefs.GetString("MATERIALS_CONTRLLLER_WINDOW_ORIGINAL_SHADER_FULL_NAME", "Universal Render Pipeline/Lit");
            originalShaderFullNameField.RegisterValueChangedCallback((ev) =>
            {
                EditorPrefs.SetString("MATERIALS_CONTRLLLER_WINDOW_ORIGINAL_SHADER_FULL_NAME", ev.newValue);
            });

            TextField newShaderFullNameField = new TextField("New Shader Full Name");
            newShaderFullNameField.value = EditorPrefs.GetString("MATERIALS_CONTRLLLER_WINDOW_NEW_SHADER_FULL_NAME", "Universal Render Pipeline/Simple Lit");
            newShaderFullNameField.RegisterValueChangedCallback((ev) =>
            {
                EditorPrefs.SetString("MATERIALS_CONTRLLLER_WINDOW_NEW_SHADER_FULL_NAME", ev.newValue);
            });

            Toggle createNewMaterialsField = new Toggle("Create New Material");
            createNewMaterialsField.tooltip = "If true, new materials will be created instead of changing the shader of the original ones.";
            createNewMaterialsField.value = EditorPrefs.GetBool("MATERIALS_CONTRLLLER_WINDOW_CREATE_NEW_MATERIALS", false);
            createNewMaterialsField.RegisterValueChangedCallback((ev) =>
            {
                EditorPrefs.SetBool("MATERIALS_CONTRLLLER_WINDOW_CREATE_NEW_MATERIALS", ev.newValue);
            });

            Button convertSelectedObjectButton = new Button(() => { ConvertSelectedObjectMaterials(originalShaderFullNameField.text, newShaderFullNameField.text, createNewMaterialsField.value); });
            convertSelectedObjectButton.text = "Convert Selected Objects";

            Button convertSelectedHierarchyButton = new Button(() => { ConvertSelectedHierarchyMaterials(originalShaderFullNameField.text, newShaderFullNameField.text, createNewMaterialsField.value); });
            convertSelectedHierarchyButton.text = "Convert Selected Hierarchies";

            Button convertEntieSceneButton = new Button(() => { ConvertEntireSceneMaterials(originalShaderFullNameField.text, newShaderFullNameField.text, createNewMaterialsField.value); });
            convertEntieSceneButton.text = "Convert Entire Scene";

            rootVisualElement.Add(conversionHeader);
            rootVisualElement.Add(originalShaderFullNameField);
            rootVisualElement.Add(newShaderFullNameField);
            rootVisualElement.Add(createNewMaterialsField);
            rootVisualElement.Add(convertSelectedObjectButton);
            rootVisualElement.Add(convertSelectedHierarchyButton);
            rootVisualElement.Add(convertEntieSceneButton);
            #endregion
        }

        private void AnalyzeSceneMaterials()
        { 
            MeshRenderer[] meshRenderers = FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            SkinnedMeshRenderer[] skinnedMeshRenderers = FindObjectsByType<SkinnedMeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            List<Renderer> renderers = new List<Renderer>();
            renderers.AddRange(meshRenderers);
            renderers.AddRange(skinnedMeshRenderers);

            int totalMeshRenderers = renderers.Count;
            List<Material> uniqueMaterials = new List<Material>();
            List<Shader> uniqueShaders = new List<Shader>();
            Dictionary<Shader, int> shaderUsages = new Dictionary<Shader, int>();
            List<Material> uniqueMeshes = new List<Material>();

            for (int i = 0; i < renderers.Count; i++)
            {
                var renderer = renderers[i];

                //if(!renderer.enabled)
                //    continue;

                for (int j = 0; j < renderer.sharedMaterials.Length; j++)
                {
                    var material = renderer.sharedMaterials[j];

                    if (!uniqueMaterials.Contains(material))
                        uniqueMaterials.Add(material);
                }
            }

            for (int i = 0; i < uniqueMaterials.Count; i++)
            {
                var material = uniqueMaterials[i];

                if (!uniqueShaders.Contains(material.shader))
                {
                    uniqueShaders.Add(material.shader);
                    shaderUsages.Add(material.shader, 1);
                }
                else
                    shaderUsages[material.shader]++;
            }

            Debug.Log($"<b>Materials Controller - Materials Analysis - Start</b>");
            Debug.Log($"Materials Controller - Materials Analysis - Total Renderers {totalMeshRenderers}");
            Debug.Log($"Materials Controller - Materials Analysis - Unique Materials {uniqueMaterials.Count}");
            Debug.Log($"Materials Controller - Materials Analysis - Unique Shaders {uniqueShaders.Count}");

            if (logIndividualMaterialsField.value == true)
            {
                for (int i = 0; i < uniqueMaterials.Count; i++)
                {
                    var material = uniqueMaterials[i];

                    Debug.Log($"Materials Controller - Materials Analysis - Material: {material.name}, shader: {material.shader.name}", material);
                }
            }

            if (logIndividualShadersField.value == true)
            {
                for (int i = 0; i < uniqueShaders.Count; i++)
                {
                    var shader = uniqueShaders[i];

                    Debug.Log($"Materials Controller - Materials Analysis - Shader: {shader.name} ({shaderUsages[shader]} materials)");
                }
            }

            Debug.Log($"<b>Materials Controller - Materials Analysis - End</b>");
        }
        private void FindRenderersUsingShader(string shaderName)
        {
            if (string.IsNullOrEmpty(shaderName))
                return;

            MeshRenderer[] meshRenderers = FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            SkinnedMeshRenderer[] skinnedMeshRenderers = FindObjectsByType<SkinnedMeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            List<Renderer> renderers = new List<Renderer>();
            renderers.AddRange(meshRenderers);
            renderers.AddRange(skinnedMeshRenderers);

            List<GameObject> uniqueGameObjects = new List<GameObject>();
            List<Material> uniqueMaterials = new List<Material>();

            for (int i = 0; i < renderers.Count; i++)
            {
                var renderer = renderers[i];

                //if (!renderer.enabled)
                //    continue;

                for (int j = 0; j < renderer.sharedMaterials.Length; j++)
                {
                    var material = renderer.sharedMaterials[j];
                    var shader = material.shader;

                    if (shader.name == shaderName)
                    {
                        if (!uniqueGameObjects.Contains(renderer.gameObject))
                            uniqueGameObjects.Add(renderer.gameObject);

                        if (!uniqueMaterials.Contains(material))
                            uniqueMaterials.Add(material);
                    }
                }
            }

            Debug.Log($"<b>Materials Controller - Find Objects Using Target Shader - Start</b>");
            Debug.Log($"Materials Controller - Find Objects Using Target Shader - Found <b>{uniqueGameObjects.Count}</b> game objects using the shader \"{shaderName}\" through <b>{uniqueMaterials.Count}</b> unique materials.");

            if (logGameObjectsField.value == true)
            {
                for (int i = 0; i < uniqueGameObjects.Count; i++)
                {
                    var go = uniqueGameObjects[i];

                    Debug.Log($"Materials Controller - Find Objects Using Target Shader - {go.name}", go);
                }
            }

            for (int i = 0; i < uniqueMaterials.Count; i++)
            {
                var material = uniqueMaterials[i];

                Debug.Log($"Materials Controller - Find Objects Using Target Shader - Material: {material.name}", material);
            }

            Debug.Log($"<b>Materials Controller - Find Objects Using Target Shader - End</b>");
        }
        private void ConvertSelectedObjectMaterials(string originalShaderName, string newShaderName, bool createNewMaterials)
        {
            if(Selection.gameObjects.Length == 0)
            {
                Debug.LogError($"Materials Controller - No objects selected.");
                return;
            }

            if(!EditorUtility.DisplayDialog("Convert Selected Objects", "You're about to convert the materials of the selected objects, this action can not be undone. Are you sure you want to proceed?", "Yes", "Cancel"))
                return;
           
            List<Renderer> renderers = new List<Renderer>();

            for(int i = 0; i < Selection.gameObjects.Length; i++)
            {
                GameObject selectedGO = Selection.gameObjects[i];

                MeshRenderer renderer = selectedGO.GetComponent<MeshRenderer>();
                SkinnedMeshRenderer skinnedRenderer = selectedGO.GetComponent<SkinnedMeshRenderer>();

                if (renderer != null)
                    renderers.Add(renderer);
                if (skinnedRenderer != null)
                    renderers.Add(skinnedRenderer);
            }

            ConvertRenderersMaterials(renderers, originalShaderName, newShaderName, createNewMaterials);
        }
        private void ConvertSelectedHierarchyMaterials(string originalShaderName, string newShaderName, bool createNewMaterials)
        {
            if(Selection.gameObjects.Length == 0)
            {
                Debug.LogError($"Materials Controller - No objects selected.");
                return;
            }

            if(!EditorUtility.DisplayDialog("Convert Selected Hierarchies", "You're about to convert the materials of the selected hierarchies, this action can not be undone. Are you sure you want to proceed?", "Yes", "Cancel"))
                return;

            List<Renderer> renderers = new List<Renderer>();

            for(int i = 0; i < Selection.gameObjects.Length; i++)
            {
                GameObject selectedGO = Selection.gameObjects[i];

                MeshRenderer[] nestedRenderers = selectedGO.GetComponentsInChildren<MeshRenderer>();
                SkinnedMeshRenderer[] nestedSkinnedRenderers = selectedGO.GetComponentsInChildren<SkinnedMeshRenderer>();

                renderers.AddRange(nestedRenderers);
                renderers.AddRange(nestedSkinnedRenderers);
            }

            ConvertRenderersMaterials(renderers, originalShaderName, newShaderName, createNewMaterials);
        }
        private void ConvertEntireSceneMaterials(string originalShaderName, string newShaderName, bool createNewMaterials)
        {
            if(!EditorUtility.DisplayDialog("Convert Entire Scene", "You're about to convert the materials of the entire scene, this action can not be undone. Are you sure you want to proceed?", "Yes", "Cancel"))
                return;

            MeshRenderer[] meshRenderers = FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            SkinnedMeshRenderer[] skinnedMeshRenderers = FindObjectsByType<SkinnedMeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            List<Renderer> allRenderers = new List<Renderer>();
            allRenderers.AddRange(meshRenderers);
            allRenderers.AddRange(skinnedMeshRenderers);

            ConvertRenderersMaterials(allRenderers, originalShaderName, newShaderName, createNewMaterials);
        }
        private void ConvertRenderersMaterials(List<Renderer> renderers, string originalShaderName, string newShaderName, bool createNewMaterials)
        {
            Shader newShader = Shader.Find(newShaderName);

            Dictionary<Material, Material> copiesByOriginalMaterial = new Dictionary<Material, Material>();

            if(newShader == null)
            {
                Debug.LogError($"Materials Controller - Couldn't find a shader with the name \"{newShaderName}\".");
                return;
            }

            List<Material> convertedMaterials = new List<Material>();

            for(int i = 0; i < renderers.Count; i++)
            {
                var renderer = renderers[i];

                if(renderer == null /*|| !renderer.enabled*/)
                    continue;

                Material[] sharedMaterials = renderer.sharedMaterials;

                for(int j = 0; j < sharedMaterials.Length; j++)
                {
                    var material = sharedMaterials[j];

                    if(material == null)
                        continue;

                    if(material.shader.name == originalShaderName)
                    {
                        if(!createNewMaterials)
                        {
                            material.shader = newShader;
                            AssetDatabase.SaveAssetIfDirty(material);
                        }
                        else
                        {
                            if(copiesByOriginalMaterial.TryGetValue(material, out var copiedMaterial))
                            {
                                sharedMaterials[j] = copiedMaterial;
                            }
                            else
                            {
                                string originalMatPath = AssetDatabase.GetAssetPath(material);
                                string newMatPath = originalMatPath.Substring(0, originalMatPath.Length - 4) + "_Converted.mat";

                                if(AssetDatabase.CopyAsset(originalMatPath, newMatPath))
                                {
                                    Material copy = AssetDatabase.LoadAssetAtPath<Material>(newMatPath);
                                    copy.shader = newShader;

                                    sharedMaterials[j] = copy;

                                    convertedMaterials.Add(material);
                                    copiesByOriginalMaterial.Add(material, copy);
                                }
                            }
                        }
                    }
                }

                renderer.sharedMaterials = sharedMaterials;

                EditorUtility.SetDirty(renderer);
            }

            StringBuilder sb = new StringBuilder();

            sb.Append("Converted the following materials(").Append(convertedMaterials.Count).Append("):\n");

            for (int i = 0; i < convertedMaterials.Count; i++)
            {
                sb.Append(convertedMaterials[i].name);

                if(i < convertedMaterials.Count - 1)
                    sb.Append("\n");
            }

            EditorUtility.DisplayDialog("Conversion Results", sb.ToString(), "OK");
        }
    }
}