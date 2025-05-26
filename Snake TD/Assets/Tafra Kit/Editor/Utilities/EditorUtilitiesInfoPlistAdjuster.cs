#if UNITY_IOS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using TafraKit;
using UnityEditor.Callbacks;
using System.IO;

public class EditorUtilitiesInfoPlistAdjuster
{
    public int callbackOrder => int.MaxValue;

    [PostProcessBuild(int.MaxValue)]
    public static void PostProcessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget != BuildTarget.iOS)
            return;

        EditorUtilitiesSettings editorUtilitiesSettings = TafraSettings.GetSettings_Editor<EditorUtilitiesSettings>();

        if (editorUtilitiesSettings.SKAdNetworkItems.Length == 0)
            return;

        TafraDebugger.Log("Editor Utilities", $"Attempting to add {editorUtilitiesSettings.SKAdNetworkItems.Length} SKAdNetworkItems to info plist.", TafraDebugger.LogType.Info);

        string plistPath = Path.Combine(path, "Info.plist");
        PlistDocument plist = new PlistDocument();

        plist.ReadFromFile(plistPath);

        PlistElementDict root = plist.root;

        PlistElementArray skAdNetworkArray = root["SKAdNetworkItems"].AsArray();

        for (int i = 0; i < editorUtilitiesSettings.SKAdNetworkItems.Length; i++)
        {
            TafraDebugger.Log("Editor Utilities", $"Added SKAdNetworkIdentifier ({editorUtilitiesSettings.SKAdNetworkItems[i]}) to info plist.", TafraDebugger.LogType.Info);
            skAdNetworkArray.AddDict().SetString("SKAdNetworkIdentifier", editorUtilitiesSettings.SKAdNetworkItems[i]);
        }

        plist.WriteToFile(plistPath);
    }
}
#endif