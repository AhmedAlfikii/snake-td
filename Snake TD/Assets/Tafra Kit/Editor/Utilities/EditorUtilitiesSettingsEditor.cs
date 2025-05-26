using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TafraKit;

namespace TafraKitEditor
{
    [CustomEditor(typeof(EditorUtilitiesSettings))]
    public class EditorUtilitiesSettingsEditor : SettingsModuleEditor
    {
        private SerializedProperty clearPrefsOnPlay;
        private SerializedProperty buildAddressablesOnBuild;
        private SerializedProperty skAdNetworkItems;

        private bool addressablesPackageFound;
        private string definingSymbol = "TAFRA_ADDRESSABLES";

        public int callbackOrder => 0;

        private void OnEnable()
        {
            clearPrefsOnPlay = serializedObject.FindProperty("ClearPrefsOnPlay");
            buildAddressablesOnBuild = serializedObject.FindProperty("BuildAddressablesOnBuild");
            skAdNetworkItems = serializedObject.FindProperty("SKAdNetworkItems");

            CheckIfAddressablesPackageExist();
        }

        public override void OnFocus()
        {
            CheckIfAddressablesPackageExist();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(clearPrefsOnPlay);

            if(!addressablesPackageFound)
                EditorGUILayout.HelpBox("Addressables package does not exit in the project.", MessageType.Info);

            EditorGUI.BeginDisabledGroup(!addressablesPackageFound);
            EditorGUILayout.PropertyField(buildAddressablesOnBuild);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(skAdNetworkItems);


            serializedObject.ApplyModifiedProperties();
        }

        void CheckIfAddressablesPackageExist()
        {
            addressablesPackageFound = TafraEditorUtility.CheckIfAClassExist("UnityEditor.AddressableAssets.Settings", "AddressableAssetSettings");

            if(addressablesPackageFound)
            {
                TafraEditorUtility.AddDefiningSymbols(definingSymbol, BuildTargetGroup.Android);
                TafraEditorUtility.AddDefiningSymbols(definingSymbol, BuildTargetGroup.iOS);
            }
            else
            {
                TafraEditorUtility.RemoveDefiningSymbols(definingSymbol, BuildTargetGroup.Android);
                TafraEditorUtility.RemoveDefiningSymbols(definingSymbol, BuildTargetGroup.iOS);
            }
        }
    }
}