using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TafraKit;

namespace TafraKitEditor
{
    [CustomEditor(typeof(ProjectSetupSettings))]
    public class ProjectSetupSettingsEditor : SettingsModuleEditor
    {
        interface ISetting {
            void Fix();
            bool IsFixed();
            string GetName();
        };
        class Setting<T> : ISetting
        {
            private string name;
            private Func<T> Get;
            private Action<T> Set;
            private T wantedValue;

            public Setting(string _name, Func<T> _get, Action<T> _set, T _wantedValue)
            {
                name = _name;
                wantedValue = _wantedValue;
                Get = _get;
                Set = _set;
            }
            public T Value
            {
                get { return Get(); }
                set { Set(value); }
            }

            public void Fix()
            {
                Value = wantedValue;
            }
            public bool IsFixed()
            {
                return EqualityComparer<T>.Default.Equals(Value, wantedValue);
            }
            public string GetName()
            {
                return name;
            }
            public void SetWantedValue(T val)
            {
                wantedValue = val;
            }
        }

        List<ISetting> settings = new List<ISetting>() 
        {
            new Setting<string>(
            "Package Name",
            () => PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android),
            val => {
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, val);
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, val);
            },
            "com.tafra."),

            new Setting<bool>(
            "Render outside safe area",
            () => PlayerSettings.Android.renderOutsideSafeArea,
            val => { PlayerSettings.Android.renderOutsideSafeArea = val; },
            true),

            new Setting<UIOrientation>(
            "Portrait orientation",
            () => PlayerSettings.defaultInterfaceOrientation,
            val => { PlayerSettings.defaultInterfaceOrientation = val; },
            UIOrientation.Portrait),

            new Setting<AndroidSdkVersions>(
            "Minimum API Level",
            () => PlayerSettings.Android.minSdkVersion,
            val => { PlayerSettings.Android.minSdkVersion = val; },
            AndroidSdkVersions.AndroidApiLevel24),

            new Setting<ScriptingImplementation>(
            "Scripting Backend",
            () => PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android),
            val => { PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, val); PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, val); },
            ScriptingImplementation.IL2CPP),

            new Setting<ApiCompatibilityLevel>(
            "API Compatibility Level",
            () => PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.Android),
            val => { PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android, val); PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.iOS, val); },
            ApiCompatibilityLevel.NET_4_6),

            new Setting<AndroidArchitecture>(
            "Target Architectures",
            () => PlayerSettings.Android.targetArchitectures,
            val => { PlayerSettings.Android.targetArchitectures = val; },
            AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64)
        };

        private void OnEnable()
        {
            UpdatePackageNameWantedValue();
        }
        public override void OnFocus()
        {
            UpdatePackageNameWantedValue();
        }
        public override void OnInspectorGUI()
        {
            #region Company and Product Name
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();

            PlayerSettings.companyName = EditorGUILayout.TextField("Company Name", PlayerSettings.companyName);

            if (PlayerSettings.companyName != "Tafra Games" && GUILayout.Button("Fix", EditorStyles.miniButton))
                PlayerSettings.companyName = "Tafra Games";

            EditorGUILayout.EndHorizontal();

            PlayerSettings.productName = EditorGUILayout.TextField("Product Name", PlayerSettings.productName);

            bool companyProductChange = EditorGUI.EndChangeCheck();
            #endregion

            EditorGUILayout.Space(20);

            if (companyProductChange)
                UpdatePackageNameWantedValue();

            bool settingNeedsFixing = false;
            for (int i = 0; i < settings.Count; i++)
            {
                if (!settings[i].IsFixed())
                    settingNeedsFixing = true;
            }

            if (settingNeedsFixing && GUILayout.Button("Fix All"))
                FixAll();

            for (int i = 0; i < settings.Count; i++)
            {
                DrawSetting(settings[i]);
            }
        }

        void DrawSetting(ISetting setting)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(setting.GetName());
            GUILayout.FlexibleSpace();
            if (setting.IsFixed())
                EditorGUILayout.Toggle(true);
            else
            {
                if (GUILayout.Button("Fix", EditorStyles.miniButton))
                {
                    setting.Fix();
                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        void UpdatePackageNameWantedValue()
        {
            //Set the correct package name here since we can't get PlayerSettings.productName in the settings constructor.
            string correctPackageName = "com.tafra." + PlayerSettings.productName.ToLower().Replace(" ", "");
            ((Setting<string>)settings[0]).SetWantedValue(correctPackageName);
        }

        void FixAll()
        {
            for (int i = 0; i < settings.Count; i++)
            {
                if (!settings[i].IsFixed())
                {
                    settings[i].Fix();
                }
            }
        }
    }
}