#if TAFRA_CINEMACHINE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using UnityEditor;

[CanEditMultipleObjects()]
[CustomEditor(typeof(CinemachineViewFrame))]
public class CinemachineViewFrameEditor : Editor
{
    private CinemachineViewFrame cinemachineViewFrame;
    private int originalPriority;

    private void OnEnable()
    {
        if(Application.isPlaying)
            return;

        cinemachineViewFrame = (CinemachineViewFrame)target;
        CinemachineCamera cam = cinemachineViewFrame.GetCamera();

        if(cam != null && cam.gameObject.activeInHierarchy)
        {
            cam.gameObject.SetActive(false);
            cam.gameObject.SetActive(true);
        }
    }
    private void OnDisable()
    {
        if(cinemachineViewFrame == null) 
            return;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(!Application.isPlaying)
        {
            if(cinemachineViewFrame.myCam != null)
                Undo.RecordObject(cinemachineViewFrame.myCam, $"Ajusting Camera ({cinemachineViewFrame.myCam.name}) To View.");

            cinemachineViewFrame.DoUpdate();

            //if(cinemachineViewFrame.myCam != null)
            //    EditorUtility.SetDirty(cinemachineViewFrame.myCam);
        }

    }
}
#endif