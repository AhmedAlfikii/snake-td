using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ZUtilities
{
    [CustomEditor(typeof(ZCameraView))]
    public class ZCameraViewEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            if (GUILayout.Button("Aligh With Camera"))
            {
                (target as ZCameraView).AlignWithMainCamera();
            }
            if (GUILayout.Button("Aligh Camera With Me"))
            {
                (target as ZCameraView).AlignCameraWithView();
            }
        }
    }
}