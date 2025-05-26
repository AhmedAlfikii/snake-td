using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TafraKitEditor;
using TafraKit.MobileNotifications;

namespace TafraKitEditor.MobileNotifications
{
    [CustomEditor(typeof(MobileNotificationsSettings))]
    public class MobileNotificationsSettingsEditor : SettingsModuleEditor
    {
        private SerializedProperty notificationsEnabled;
        private SerializedProperty mutedByDefault;
        private SerializedProperty autoAskForPermission;
        private SerializedProperty channels;
        private SerializedProperty closingNotifications;

        private bool mobileNotificationsFound;
        private string definingSymbol = "TAFRA_MOBILE_NOTIFICATIONS";

        private void OnEnable()
        {
            notificationsEnabled = serializedObject.FindProperty("Enabled");
            mutedByDefault = serializedObject.FindProperty("MutedByDefault");
            autoAskForPermission = serializedObject.FindProperty("AutoAskForPermission");
            channels = serializedObject.FindProperty("Channels");
            closingNotifications = serializedObject.FindProperty("ClosingNotifications");

            CheckIfMobileNotificationsPackageExist();
        }

        public override void OnFocus()
        {
            CheckIfMobileNotificationsPackageExist();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!mobileNotificationsFound)
            {
                #if UNITY_IOS || UNITY_ANDROID
                EditorGUILayout.HelpBox("You need to install Unity's Mobile Notifications package from the package manager in order to use this module.", MessageType.Error);
               
                if (GUILayout.Button("Install Mobile Notifications Package"))
                    UnityEditor.PackageManager.UI.Window.Open("com.unity.mobile.notifications");

                #else
                EditorGUILayout.HelpBox("Make sure that your build platform is either Android or iOS, and that Unity's Mobile Notifications package from the package manager is installed.", MessageType.Error);
                #endif

                if (notificationsEnabled.boolValue == true)
                    notificationsEnabled.boolValue = false;
            }

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(notificationsEnabled);

            if (EditorGUI.EndChangeCheck())
            {
                if (notificationsEnabled.boolValue == true)
                {
                    #if UNITY_IOS || UNITY_ANDROID
                    if (!mobileNotificationsFound)
                    {
                        if (EditorUtility.DisplayDialog("Package Not Found", "You need to install Unity's Mobile Notifications package from the package manager first.", "Install", "Later"))
                            UnityEditor.PackageManager.UI.Window.Open("com.unity.mobile.notifications");
                        notificationsEnabled.boolValue = false;
                    }
                    else
                    {
                        TafraEditorUtility.AddDefiningSymbols(definingSymbol, BuildTargetGroup.Android);
                        TafraEditorUtility.AddDefiningSymbols(definingSymbol, BuildTargetGroup.iOS);
                    }
                    #else
                    EditorUtility.DisplayDialog("Wrong Platform", "Make sure that your build platform is either Android or iOS, and that Unity's Mobile Notifications package from the package manager is installed.", "Ok");
                    notificationsEnabled.boolValue = false;
                    #endif
                }
                else
                {
                    TafraEditorUtility.RemoveDefiningSymbols(definingSymbol, BuildTargetGroup.Android);
                    TafraEditorUtility.RemoveDefiningSymbols(definingSymbol, BuildTargetGroup.iOS);
                }
            }

            EditorGUI.BeginDisabledGroup(!notificationsEnabled.boolValue);

            EditorGUILayout.PropertyField(mutedByDefault);
            EditorGUILayout.PropertyField(autoAskForPermission);

            #if TAFRA_MOBILE_NOTIFICATIONS
            EditorGUILayout.PropertyField(channels);
            EditorGUILayout.PropertyField(closingNotifications);
            #endif
            
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }

        void CheckIfMobileNotificationsPackageExist()
        {
            #if UNITY_ANDROID
            mobileNotificationsFound = TafraEditorUtility.CheckIfAClassExist("Unity.Notifications.Android", "AndroidNotificationCenter");
            #endif
            #if UNITY_IOS
            mobileNotificationsFound = TafraEditorUtility.CheckIfAClassExist("Unity.Notifications.iOS", "iOSNotificationCenter");
            #endif
        }
    }
}
