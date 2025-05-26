using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TafraKit.UI
{
    [ExecuteAlways]
    public class CanvasPixelPerUnitScaler : MonoBehaviour
    {
        [SerializeField] private CanvasScaler canvasScaler;
        [SerializeField] private Vector2 referenceResolution;
        [SerializeField] private float referencePixelsPerUnit;

        private void Start()
        {
            Refresh();
        }

        #if UNITY_EDITOR
        private void Update()
        {
            Refresh();
        }
        #endif

        private void Refresh()
        {
            if(!canvasScaler)
                return;

            float referenceAspect = referenceResolution.x / referenceResolution.y;
            float curAspect = Screen.width / (float)Screen.height;

            float pixelsPerUnit = (referenceAspect / curAspect) * referencePixelsPerUnit;

            canvasScaler.referencePixelsPerUnit = pixelsPerUnit;
        }
    }
}