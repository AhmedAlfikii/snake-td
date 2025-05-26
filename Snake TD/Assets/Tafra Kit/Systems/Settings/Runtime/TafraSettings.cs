using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit
{
    public static class TafraSettings
    {
        private static List<SettingsModule> settings = new List<SettingsModule>();
        private static bool isInitialized;

        public static bool IsInitialized => isInitialized;

        public static UnityEvent OnInitialize = new UnityEvent();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            ScriptableObject[] scriptableObjects = Resources.LoadAll<ScriptableObject>("Tafra Settings");

            for (int i = 0; i < scriptableObjects.Length; i++)
            {
                if (scriptableObjects[i] is SettingsModule)
                    settings.Add(scriptableObjects[i] as SettingsModule);
            }

            isInitialized = true;
            OnInitialize?.Invoke();
        }

        //TODO: Optimize this. Maybe store all the settings in a dictionary by type name.
        public static T GetSettings<T>() where T : SettingsModule
        {
            for (int i = 0; i < settings.Count; i++)
            {
                if (settings[i] is T)
                    return settings[i] as T;
            }

            TafraDebugger.Log("Settings", $"Couldn't find the settings object you're looking for ({typeof(T)})", TafraDebugger.LogType.Error);

            return null;
        }


        #if UNITY_EDITOR
        /// <summary>
        /// Used particularly for editor scripts (in case another editor script requires to access a certain settings module).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetSettings_Editor<T>() where T : SettingsModule
        {
            List<SettingsModule> settingModules = new List<SettingsModule>();

            ScriptableObject[] scriptableObjects = Resources.LoadAll<ScriptableObject>("Tafra Settings");

            for (int i = 0; i < scriptableObjects.Length; i++)
            {
                if (scriptableObjects[i] is SettingsModule)
                    settingModules.Add(scriptableObjects[i] as SettingsModule);
            }

            for (int i = 0; i < settingModules.Count; i++)
            {
                if (settingModules[i] is T)
                    return settingModules[i] as T;
            }

            TafraDebugger.Log("Settings", $"Couldn't find the settings object you're looking for ({typeof(T)})", TafraDebugger.LogType.Error);

            return null;
        }
        #endif
    }
}