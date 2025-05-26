using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ZUtilities
{
    public class ZCameraView : MonoBehaviour
    {
        [SerializeField] private ZCamera.CoveringType coveringType = ZCamera.CoveringType.Envelope;
        [SerializeField] private ZCamera.Pivot pivot;

        [Header("Edit Only Properties")]
        [SerializeField] private float distanceFromCam = 5; 

        public ZCamera.CoveringType CoveringType()
        {
            return coveringType;
        }
        public ZCamera.Pivot Pivot()
        {
            return pivot;
        }

        void OnDrawGizmosSelected()
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
            Handles.ArrowHandleCap(0, transform.position, transform.rotation * Quaternion.LookRotation(-Vector3.forward), 1, EventType.Repaint);
            #endif
        }

        [ContextMenu("Align With Main Cam")]
        public void AlignWithMainCamera()
        {
            Camera cam = Camera.main;

            transform.position = cam.transform.position + cam.transform.forward * distanceFromCam;
            transform.forward = -cam.transform.forward;

            float frustumHeight = 2.0f * distanceFromCam * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float frustumWidth = frustumHeight * cam.aspect;

            transform.localScale = new Vector3(frustumWidth, frustumHeight, 1);
        }

        [ContextMenu("Align Cam With View")]
        public void AlignCameraWithView()
        {
            Camera cam = Camera.main;
            cam.GetComponent<ZCamera>().CoverViewEditor(this);
        }
    }
}
