using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEditor;
using Unity.AI.Navigation.Editor;

[CustomEditor(typeof(NavMeshBaker))]
public class NavMeshBakerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        NavMeshBaker baker = target as NavMeshBaker;
        
        if(GUILayout.Button("Bake Children"))
        {
            baker.Bake();
        }
    }
}
