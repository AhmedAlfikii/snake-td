using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TafraKit;
using System.Reflection;
using System.Collections;
using System.IO;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace TafraKitEditor
{
    [InitializeOnLoad]
    public static class TafraEditorUtility
    {
        private static bool verboseLogCloning = false;

        private static object clipboardObject;
        /// <summary>
        /// Used to avoid infinite recurrsing when cloning objects that reference objects referencing them (mostly in SerializeReference objects).
        /// Contains the original object as key and the its cloned version as value.
        /// </summary>
        private static Dictionary<object, object> tempAlreadyClonedObjects;

        static TafraEditorUtility()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            EditorUtilitiesSettings settings = TafraSettings.GetSettings_Editor<EditorUtilitiesSettings>();

            switch (change)
            {
                case PlayModeStateChange.EnteredEditMode:
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    if (settings.ClearPrefsOnPlay)
                        PlayerPrefs.DeleteAll();
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    //The log is moved here while the action is in ExitingEditMode because logs can get clearet after entering play mode.
                    if (settings.ClearPrefsOnPlay)
                        Debug.Log("<b><color=red>Cleared all PlayerPrefs (delyed log).</color></b>");
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
            }
        }
        public static void AddDefiningSymbols(string symbol, BuildTargetGroup platform)
        {
            string[] curDefiningSymbols = new string[] { };

            PlayerSettings.GetScriptingDefineSymbolsForGroup(platform, out curDefiningSymbols);

            List<string> symbolsList = new List<string>(curDefiningSymbols);

            if (!symbolsList.Contains(symbol))
            {
                symbolsList.Add(symbol);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, symbolsList.ToArray());
            }
        }
        public static void RemoveDefiningSymbols(string symbol, BuildTargetGroup platform)
        {
            string[] curDefiningSymbols = new string[] { };

            PlayerSettings.GetScriptingDefineSymbolsForGroup(platform, out curDefiningSymbols);

            List<string> symbolsList = new List<string>(curDefiningSymbols);

            if (symbolsList.Contains(symbol))
            {
                symbolsList.Remove(symbol);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, symbolsList.ToArray());
            }
        }
        public static bool CheckIfAClassExist(string namespaceName, string className)
        {
            Type type = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                         from typeX in assembly.GetTypes()
                         where typeX.Namespace == namespaceName && typeX.Name == className
                         select typeX).FirstOrDefault();

            bool found = type != null;

            return found;
        }
        public static bool OpenClassScript(Type classType)
        {
            string className = classType.Name;

            List<string> scriptPathes = new List<string>();
          
            GetAllScriptPaths(Application.dataPath, scriptPathes);

            for (int i = 0; i < scriptPathes.Count; i++)
            {
                var scriptPath = scriptPathes[i];
                if(scriptPath.EndsWith(className + ".cs"))
                {
                    TextAsset scriptFile = AssetDatabase.LoadAssetAtPath<TextAsset>(scriptPath.Substring(scriptPath.IndexOf("Assets")));
              
                    AssetDatabase.OpenAsset(scriptFile.GetInstanceID());
                    return true;
                }
            }

            return false;
        }
        public static bool SelectClassScript(Type classType)
        {
            string className = classType.Name;

            List<string> scriptPathes = new List<string>();
          
            GetAllScriptPaths(Application.dataPath, scriptPathes);

            for (int i = 0; i < scriptPathes.Count; i++)
            {
                var scriptPath = scriptPathes[i];
                if(scriptPath.EndsWith(className + ".cs"))
                {
                    TextAsset scriptFile = AssetDatabase.LoadAssetAtPath<TextAsset>(scriptPath.Substring(scriptPath.IndexOf("Assets")));
              
                    EditorGUIUtility.PingObject(scriptFile);
                    return true;
                }
            }

            return false;
        }

        private static void GetAllScriptPaths(string rootDirectory, List<string> listToPopulate)
        {
            foreach(string file in Directory.GetFiles(rootDirectory))
            {
                if(file.EndsWith(".cs"))
                    listToPopulate.Add(file);
            }

            foreach(string dir in Directory.GetDirectories(rootDirectory))
            {
                GetAllScriptPaths(dir, listToPopulate);
            }
        }

        #region Prefabs
        /// <summary>
        /// Fills a list with the sent prefab instance's prefab asset and all of that asset's parents.
        /// </summary>
        /// <param name="childPrefabInstance"></param>
        /// <param name="parentsListToFill"></param>
        public static void GetAllPrefabInstanceParents(GameObject childPrefabInstance, List<GameObject> parentsListToFill)
        {
            parentsListToFill.Clear();

            GameObject nextGO = childPrefabInstance;
            do
            {
                GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(nextGO);

                nextGO = prefab;

                if (prefab != null)
                    parentsListToFill.Add(prefab);

            } while (nextGO != null);
        }
        #endregion

        #region Copying Objects
        public static void AddObjectToClipboard(object obj)
        {
            clipboardObject = obj;
        }
        public static object GetClipboardObject()
        {
            return clipboardObject;
        }
        public static bool CanPasteClipboardObjectOnObject(object pasteOn)
        {
            if(clipboardObject == null || pasteOn == null)
                return false;

            if (clipboardObject.GetType() != pasteOn.GetType())
                return false;

            return true;
        }
        public static bool PasteClipboardObjectOnObject(object pasteOn)
        {
            if(clipboardObject == null)
            {
                Debug.Log("Clipboard is empty!");
                return false;
            }

            tempAlreadyClonedObjects = new Dictionary<object, object>();

            bool cloned = CloneObject(clipboardObject, pasteOn);

            tempAlreadyClonedObjects = null;
            
            return cloned;
        }
        public static object GetClipboardObjectCopy()
        {
            if (clipboardObject == null)
            {
                Debug.Log("Clipboard is empty!");
                return null;
            }

            object copy = Activator.CreateInstance(clipboardObject.GetType());

            tempAlreadyClonedObjects = new Dictionary<object, object>();

            bool copied = CloneObject(clipboardObject, copy);

            tempAlreadyClonedObjects = null;

            if(!copied)
            {
                Debug.LogError("Failed to clone the clipboard object.");
                return null;
            }

            return copy;
        }
        public static bool CanPasteOnObject(object obj)
        {
            if(clipboardObject == null)
                return false;

            if(obj.GetType() != clipboardObject.GetType())
                return false;

            return true;
        }
        public static bool Clone(object source, object destination)
        {
            tempAlreadyClonedObjects = new Dictionary<object, object>();

            bool cloned = CloneObject(source, destination);

            tempAlreadyClonedObjects = null;

            return cloned;
        }
        private static bool CloneObject(object source, object destination)
        {
            if (source == null || destination == null)
            {
                Debug.Log($"Clone Object - Object: Can't clone object while the source or the destintaiton are null.");
                return false;
            }
            else if (verboseLogCloning)
                Debug.Log($"Clone Object - Object: Non of the objects are null, proceeding to the next step.");

            Type sourceType = source.GetType();
            Type destinationType = destination.GetType();

            if (verboseLogCloning)
                Debug.Log($"<b>Clone Object - Object: Source object is of type ({sourceType}). Destination object is of type ({destinationType}).</b>");

            if (sourceType != destinationType && !destinationType.IsAssignableFrom(sourceType))
            {
                Debug.Log($"Clone Object - Object: Can't clone object to an object with a different type and the source type isn't a subclass of the destination type. ({sourceType} -> {destinationType})");
                return false;
            }
            else if (verboseLogCloning)
                Debug.Log($"Clone Object - Object: Both objects are of the same type, proceeding to the next step.");

            if (sourceType.IsList())
            {
                if (verboseLogCloning)
                    Debug.Log($"Clone Object - Object: Source object is a list, will use the list cloner instead.");

                return CloneListObject(source, destination);
            }
            else if (verboseLogCloning)
                Debug.Log($"Clone Object - Object: Source object is not a list, will continue using the object cloner.");

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

            List<FieldInfo> fieldsInfo = sourceType.GetFieldsUnambiguous(flags);
            foreach(var field in fieldsInfo)
            {
                if (verboseLogCloning)
                    Debug.Log($"<b>Clone Object - Object: Checking the field named \"{field.Name}\".</b>");

                //Skip fields marked as non-serialized.
                if (field.GetCustomAttribute<NonSerializedAttribute>() != null)
                {
                    if (verboseLogCloning)
                        Debug.Log($"Clone Object - Object: Field \"{field.Name}\" has a \"NonSerialized\" attribute. Skipping it.");

                    continue;
                }

                //Skip non-public fields that don't have a serialize field or reference attributes.
                if (!field.IsPublic && field.GetCustomAttribute<SerializeField>() == null && field.GetCustomAttribute<SerializeReference>() == null)
                {
                    if (verboseLogCloning)
                        Debug.Log($"Clone Object - Object: Field \"{field.Name}\" is not public and does not have a \"SerializeField\" or \"SerializeReference\" attributes. Skipping it.");

                    continue;
                }

                if (verboseLogCloning)
                    Debug.Log($"Clone Object - Object: Field \"{field.Name}\" passed the checks, attempting to clone it.");

                Type sourceFieldType = field.FieldType;
                object sourceFieldValue = field.GetValue(source);

                if (verboseLogCloning)
                    Debug.Log($"Clone Object - Object: Field \"{field.Name}\" type is ({sourceFieldType}), and its value is ({sourceFieldValue}). Attempting to copy it.");

                if (sourceFieldValue != null && tempAlreadyClonedObjects.TryGetValue(sourceFieldValue, out object readyClone))
                {
                    field.SetValue(destination, readyClone);
                }
                else
                {
                    CloneBasedOnType((sourceFieldValue != null ? sourceFieldValue.GetType() : sourceFieldType), sourceFieldValue,
                        onClone: (result) =>
                        {
                            if (verboseLogCloning)
                                Debug.Log($"Clone Object - Object: Field \"{field.Name}\" copy operation's resulting value is ({result}), will assign it to the destination object.");

                            field.SetValue(destination, result);

                            if (sourceFieldValue != null && sourceFieldValue is not ValueType && sourceFieldValue is not String)
                            {
                                tempAlreadyClonedObjects.Add(sourceFieldValue, result);
                            }
                        });

                }
            }

            if (verboseLogCloning)
                Debug.Log($"Clone Object - Object: Object cloning completed successfully.");

            return true;
        }
        private static bool CloneListObject(object sourceListObject, object destinationListObject)
        {
            if(sourceListObject == null || destinationListObject == null)
            {
                Debug.Log("Can't clone list object while the source or the destintaiton are null.");
                return false;
            }
            else if (verboseLogCloning)
                Debug.Log($"Clone Object - List: Non of the lists are null, proceeding to the next step.");

            Type sourceType = sourceListObject.GetType();
            Type destinationType = destinationListObject.GetType();

            if (verboseLogCloning)
                Debug.Log($"Clone Object - List: Source list is of type ({sourceType}). Destination list is of type ({destinationType}).");

            if (sourceType != destinationType)
            {
                Debug.Log($"Can't clone list object to an list object with a different type. Source: {sourceType} Destination: {destinationType}");
                return false;
            }
            else if (verboseLogCloning)
                Debug.Log($"Clone List - List: Both lists are of the same type, proceeding to the next step.");

            IList sourceList = sourceListObject as IList;
            IList destinationList = destinationListObject as IList;

            if (verboseLogCloning)
                Debug.Log($"Clone List - List: Source list is ({sourceList}), destination list is ({destinationList}).");

            if (sourceList == null || destinationList == null)
            {
                Debug.Log("Can't clone object, it's not a list.");
                return false;
            }
            else if (verboseLogCloning)
                Debug.Log($"Clone List - List: Both lists are indeed lists, will proceed normally.");

            if(destinationList is Array destinationArray)
            {
                if(verboseLogCloning)
                    Debug.Log($"Clone List - List: Destination list is actually an array with the size {destinationList.Count}.");

                if(destinationArray.Length != sourceList.Count)
                {
                    destinationList = Array.CreateInstance(destinationList.GetType().GetElementType(), sourceList.Count);

                    if(verboseLogCloning)
                        Debug.Log($"Clone List - List: Destination array doesn't have the same size as the source one, created a new instance, now it has the size of {destinationList.Count}.");
                }
                else if(verboseLogCloning)
                    Debug.Log($"Clone List - List: Destination array already has the same size as the source array, no need to create a new instance.");
            }
            else
            {
                if(verboseLogCloning)
                    Debug.Log($"Clone List - List: Destination list is actually a list.");

                //If this is an actual list, then populate it by adding an element for each corresponding element in source list.
                for(int i = 0; i < sourceList.Count; i++)
                {
                    destinationList.Add(null);
                }
            }

            for (int i = 0; i < sourceList.Count; i++)
            {
                var element = sourceList[i];

                if(verboseLogCloning)
                    Debug.Log($"Clone List - List: Found element ({element}) in source list, will clone it based on its type and add it to the destination list.");

                Type elementType = element.GetType();

                if(verboseLogCloning)
                    Debug.Log($"Clone List - List: Element ({element})'s type is ({elementType}).");

                CloneBasedOnType(elementType, element,
                    onClone: (result) =>
                    {
                        if(verboseLogCloning)
                            Debug.Log($"Clone Object - List: Element {element} copy operation's resulting value is ({result}), will add it to the destination list.");

                        destinationList[i] = result;
                    });
            }
   

            if (verboseLogCloning)
                Debug.Log($"Clone Object - List: List cloning completed successfully.");

            return true;
        }

        private static void CloneBasedOnType(Type sourceType, object sourceValue, Action<object> onClone)
        {
            bool cloneAsIs = sourceValue is ValueType || sourceValue is string || (typeof(UnityEngine.Object).IsAssignableFrom(sourceType));

            if (verboseLogCloning)
            {
                Debug.Log($"Clone Object - Clone Based On Type: Source type is ({sourceType}), source value is ({sourceValue}), source value type is ({(sourceValue != null ? sourceValue.GetType() : null)}).");
                Debug.Log($"Clone Object - Clone Based On Type: Should be cloned as is? {cloneAsIs}.");
            }


            if (cloneAsIs)
                onClone?.Invoke(sourceValue);
            else if(sourceValue is AnimationCurve sourceAnimCurve)
            {
                if (verboseLogCloning)
                    Debug.Log($"Clone Object - Clone Based On Type: Source object is an animation curve, cloning it accordingly.");

                AnimationCurve freshAnimCurve = new AnimationCurve(sourceAnimCurve.keys);
                freshAnimCurve.preWrapMode = sourceAnimCurve.preWrapMode;
                freshAnimCurve.postWrapMode = sourceAnimCurve.postWrapMode;

                onClone?.Invoke(freshAnimCurve);
            }
            else
            {
                if (verboseLogCloning)
                    Debug.Log($"Clone Object - Clone Based On Type: Source object is a boxed value, will create a new object for the fresh value, then clone the source value into it.");

                bool isList = sourceValue is IList;

                if(verboseLogCloning)
                    Debug.Log($"Clone Object - Clone Based On Type: Is the source value a list? {isList}.");

                object freshValue;

                //If this is an array, then create an instance accordingly.
                if (isList && sourceValue is Array)
                {
                    freshValue = Array.CreateInstance(sourceType.GetElementType(), ((IList)sourceValue).Count);

                    if (verboseLogCloning)
                        Debug.Log($"Clone Object - Objects are an array created a fresh array with the element type {freshValue.GetType().GetElementType()}, and the size of {((Array)freshValue).Length}.");
                }
                else
                {
                    try
                    {
                        freshValue = Activator.CreateInstance(sourceType);
                        if (verboseLogCloning)
                            Debug.Log($"Clone Object - Fresh value is {freshValue} and is of type {freshValue.GetType()}.");

                    }
                    catch (Exception e)
                    {
                        //if (verboseLogCloning)
                            Debug.LogError($"Clone Object - Couldn't clone the field of type {sourceType}: {e.Message}");
                        freshValue = null;
                    }
                }

                onClone?.Invoke(freshValue);

                //bool isList = typeof(IList).IsAssignableFrom(freshValue.GetType());

                if (!isList)
                {
                    if (verboseLogCloning)
                        Debug.Log($"Clone Object - Clone Based On Type: Fresh value is not a list, will use the object cloner.");

                    CloneObject(sourceValue, freshValue);
                }
                else
                {
                    if (verboseLogCloning)
                        Debug.Log($"Clone Object - Clone Based On Type: Fresh value is a list, will use the list cloner.");

                    CloneListObject(sourceValue, freshValue);
                }
            }
        }
        #endregion

        public static void BuildPropertyContextualMenu(SerializedProperty property, ContextualMenuPopulateEvent ev)
        {
            #region Misc. Items
            ev.menu.AppendAction("Copy Property Path", (a) =>
            {
                property.propertyPath.CopyToClipboard();
            });
            #endregion

            #region Prefab Items
            GameObject targetGameObject = property.serializedObject.targetObject.GameObject();
            bool propertyIsModified = false;

            if(targetGameObject != null)
            {
                var modifiedProperties = PrefabUtility.GetPropertyModifications(targetGameObject);

                if(modifiedProperties != null)
                {
                    for (int i = 0; i < modifiedProperties.Length; i++)
                    {
                        var modifiedProp = modifiedProperties[i];

                        if (modifiedProp.propertyPath.Contains(property.propertyPath) || (property.propertyType == SerializedPropertyType.ManagedReference && property.managedReferenceValue != null && modifiedProp.propertyPath.Contains(property.managedReferenceId.ToString())))
                        {
                            propertyIsModified = true;
                            break;
                        }
                    }
                }

                if (propertyIsModified)
                {
                    List<GameObject> prefabsList = new List<GameObject>();

                    GetAllPrefabInstanceParents(targetGameObject, prefabsList);

                    for (int i = 0; i < prefabsList.Count; i++)
                    {
                        GameObject prefab = prefabsList[i];

                        string title = "";

                        if (i < prefabsList.Count - 1)  //If this isn't the root parent.
                            title = $"Apply as Override in Prefab '{prefab.name}'";
                        else //If this is the root parent.
                            title = $"Apply to Prefab '{prefab.name}'";

                        ev.menu.AppendAction(title, (a) =>
                        {
                            PrefabUtility.ApplyPropertyOverride(property, AssetDatabase.GetAssetPath(prefab), InteractionMode.UserAction);
                            property.serializedObject.ApplyModifiedProperties();
                        });
                    }
                    ev.menu.AppendAction("Revert", (a) =>
                    {
                        PrefabUtility.RevertPropertyOverride(property, InteractionMode.UserAction);
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }
            }
            #endregion

            #region Copy & Paste
            ev.menu.AppendSeparator();

            bool canPaste = CanPasteClipboardObjectOnObject(property.boxedValue);

            ev.menu.AppendAction("Copy", (a) =>
            {
                property.serializedObject.Update();
                AddObjectToClipboard(property.boxedValue);
            });
            ev.menu.AppendAction("Paste", (a) =>
            {
                object clipboardCopy = GetClipboardObjectCopy();
                if (clipboardCopy != null)
                {
                    property.boxedValue = clipboardCopy;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }, canPaste ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            #endregion
        }
    }
}