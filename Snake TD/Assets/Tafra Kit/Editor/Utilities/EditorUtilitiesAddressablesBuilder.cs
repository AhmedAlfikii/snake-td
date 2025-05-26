#if TAFRA_ADDRESSABLES
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using TafraKit;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Build;

[InitializeOnLoad]
class UnityEditorStartup
{
    static UnityEditorStartup()
    {
        BuildPlayerWindow.RegisterBuildPlayerHandler(
               new System.Action<BuildPlayerOptions>(buildPlayerOptions => 
               {
                   EditorUtilitiesSettings editorUtilitiesSettings = Resources.Load<EditorUtilitiesSettings>("Tafra Settings/EditorUtilitiesSettings");

                   bool addressablesBuiltSuccessfully = false;
                   if (editorUtilitiesSettings.BuildAddressablesOnBuild)
                       addressablesBuiltSuccessfully = BuildAddressables();

                   if (editorUtilitiesSettings.BuildAddressablesOnBuild && !addressablesBuiltSuccessfully)
                   {
                       bool continueBuild = EditorUtility.DisplayDialog("Error - Tafra Kit", "Failed to build addressables, would you like to continue the build anyway?", "Contiune Build", "Cancel Build");
                       Debug.LogError("<color=#ff313f><b>TafraKit</b></color> - <color=white><b>Editor Utilities</b></color> - Failed to build addressables");

                       if (continueBuild)
                            BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(buildPlayerOptions);
                   }
                   else
                       BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(buildPlayerOptions);

                   if (editorUtilitiesSettings.BuildAddressablesOnBuild && addressablesBuiltSuccessfully)
                   {
                       Debug.Log("<color=#ff313f><b>TafraKit</b></color> - <color=white><b>Editor Utilities</b></color> - Successfully built addressables!");
                   }
               }));
    }

    public static bool BuildAddressables()
    {
        Debug.Log("<color=#ff313f><b>TafraKit</b></color> - <color=white><b>Editor Utilities</b></color> - Attempting to build addressables...");
        
        AddressableAssetSettings
            .BuildPlayerContent(out AddressablesPlayerBuildResult result);
        bool success = string.IsNullOrEmpty(result.Error);

        if (!success)
        {
            Debug.LogError("<color=#ff313f><b>TafraKit</b></color> - <color=white><b>Editor Utilities</b></color> - Failed to build addressables");
            return false;
        }
        else
        {
            Debug.Log("<color=#ff313f><b>TafraKit</b></color> - <color=white><b>Editor Utilities</b></color> - Successfully built addressables!");
            return true;
        }

    }
}
#endif