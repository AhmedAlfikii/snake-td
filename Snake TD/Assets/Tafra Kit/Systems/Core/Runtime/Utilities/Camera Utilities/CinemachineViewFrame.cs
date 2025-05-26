#if TAFRA_CINEMACHINE
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CinemachineViewFrame : MonoBehaviour
{
    public enum UpdateMethod
    { 
        Awake,
        Start,
        Update,
        LateUpdate,
        FixedUpdate,
        OnEnable
    }
    public UpdateMethod UpdateOn = UpdateMethod.Start;
    public float Distance = 50f;
    public CinemachineCamera myCam;
    
    private Camera mainCamera;

    private void Awake()
    {
        if (UpdateOn == UpdateMethod.Awake)
            DoUpdate();
    }
    private void Start()
    {
        if (UpdateOn == UpdateMethod.Start)
            DoUpdate();
    }
    private void OnEnable()
    {
        if (UpdateOn == UpdateMethod.OnEnable)
            DoUpdate();
    }
    private void Update()
    {
        if (UpdateOn == UpdateMethod.Update)
            DoUpdate();
    }
    private void LateUpdate()
    {
        if (UpdateOn == UpdateMethod.LateUpdate)
            DoUpdate();
    }
    private void FixedUpdate()
    {
        if (UpdateOn == UpdateMethod.FixedUpdate)
            DoUpdate();
    }
    private void OnDrawGizmosSelected()
    {
        Vector3[] points = new Vector3[4];

        points[0] = transform.TransformPoint(new Vector3(0.5f, 0.5f, 0));
        points[1] = transform.TransformPoint(new Vector3(-0.5f, 0.5f, 0));
        points[2] = transform.TransformPoint(new Vector3(-0.5f, -0.5f, 0));
        points[3] = transform.TransformPoint(new Vector3(0.5f, -0.5f, 0));

#if UNITY_EDITOR
        Handles.DrawBezier(points[0], points[1], points[0], points[1], Color.cyan, null, 3);
        Handles.DrawBezier(points[1], points[2], points[1], points[2], Color.cyan, null, 3);
        Handles.DrawBezier(points[2], points[3], points[2], points[3], Color.cyan, null, 3);
        Handles.DrawBezier(points[3], points[0], points[3], points[0], Color.cyan, null, 3);

        Handles.color = Color.cyan;
        Handles.ArrowHandleCap(0, transform.position, transform.rotation * Quaternion.LookRotation(Vector3.forward), 1, EventType.Repaint);
#endif
    }

    public void DoUpdate()
    {
        if(!mainCamera)
            mainCamera = Camera.main;

        if (!mainCamera || !myCam)
            return;

        myCam.transform.SetPositionAndRotation(transform.position - transform.forward * Distance, transform.rotation);

        float orthoSizeBasedOnHeight = transform.localScale.y / 2f;
        float orthoSizeBasedOnWidth = (transform.localScale.x / mainCamera.aspect) / 2f;

        myCam.Lens.OrthographicSize = Mathf.Max(orthoSizeBasedOnHeight, orthoSizeBasedOnWidth);
    }

    public CinemachineCamera GetCamera()
    {
        return myCam;
    }
}
#endif