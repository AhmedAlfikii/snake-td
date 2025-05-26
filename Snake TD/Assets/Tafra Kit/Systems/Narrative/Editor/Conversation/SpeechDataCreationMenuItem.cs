using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TafraKit.Narrative;

public static class SpeechDataCreationMenuItem
{
    [MenuItem("Assets/Create/Tafra Kit/Conversation/Speech Data")]
    private static void CreateSpeechData(MenuCommand menuCommand)
    {
        bool createdSO = false;
        string[] guids = Selection.assetGUIDs;
        for(int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            AudioClip ac = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);

            if(ac == null)
                continue;

            string folderPath = Path.GetDirectoryName(assetPath);

            SpeechData sd = ScriptableObject.CreateInstance<SpeechData>();
            
            sd.EditorInitialize(ac);

            AssetDatabase.CreateAsset(sd, folderPath + $"/{ac.name}.asset");

            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = sd;

            createdSO = true;
        }

        if(!createdSO)
        {
            SpeechData sd = ScriptableObject.CreateInstance<SpeechData>();
            ProjectWindowUtil.CreateAsset(sd, "Speech Data.asset");
            //ProjectWindowUtil.CreateAssetWithContent("Speech Data.asset", string.Empty);
        }
    }
}
