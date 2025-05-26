using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Text;
using System;

namespace TafraKitEditor
{
    public class LogsReader : EditorWindow
    {
        private ObjectField logField;
        private static List<string> includedTags = new List<string>()
        {
            //"ActivityManager",
            "Unity"
        };

        private static List<string> excludedTags = new List<string>()
        {
            "SamsungAlarmManager",
            "SmartBondingService",
            "ConnectivityService",
            "CustomFrequencyManagerService",
            "SettingsProvider",
        };

        [MenuItem("Tafra Games/Windows/Logs Reader", priority = 2)]
        public static void OpenWindow()
        {
            EditorWindow.GetWindow<LogsReader>();
        }

        private void CreateGUI()
        {
            logField = new ObjectField("Log");
            logField.objectType = typeof(TextAsset);
            logField.style.marginTop = 5;

            rootVisualElement.Add(logField);

            Button refineButton = new Button(Refine);

            refineButton.text = "Refine";

            rootVisualElement.Add(refineButton);
        }

        private void Refine()
        {
            TextAsset log = logField.value as TextAsset;
            StringBuilder sb = new StringBuilder();

            string[] lines = log.text.Split(Environment.NewLine);

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                if(!ContainsTags(line, includedTags))
                    continue;

                if(!line.Contains(" E "))
                    continue;

                sb.AppendLine(line);
            }

            string filePath = AssetDatabase.GetAssetPath(log.GetInstanceID());
            filePath = filePath.Substring(0, filePath.LastIndexOf('/') + 1);
            string fileName = $"{log.name} - Refined.txt";

            System.IO.File.WriteAllText($"{filePath}{fileName}", sb.ToString());
            //System.Diagnostics.Process.Start(filePath.Replace(@"/", @"\"));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            string guid = AssetDatabase.AssetPathToGUID(filePath);

            TextAsset refinedLog = AssetDatabase.LoadAssetAtPath<TextAsset>($"{filePath}{fileName}");
            AssetDatabase.OpenAsset(refinedLog.GetInstanceID());
        }
        private bool ContainsTags(string text, List<string> tags)
        {
            for(int i = 0; i < tags.Count; i++)
            {
                var tag = tags[i];
                if(text.Contains($" {tag}"))
                    return true;
            }

            return false;
        }
    }
}