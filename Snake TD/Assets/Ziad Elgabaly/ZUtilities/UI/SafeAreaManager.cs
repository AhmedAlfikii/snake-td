using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZUtilities
{
    [RequireComponent(typeof(Canvas))]
    public class SafeAreaManager : MonoBehaviour
    {
        [SerializeField] private RectTransform[] safeAreaTransforms;
        [SerializeField] private bool fakeNotchInEditor;
        [Range(0, 0.1f)]
        [SerializeField] private float fakeNotchSize = 0.05f;

        private Canvas canvas;

        void Start()
        {
            canvas = GetComponent<Canvas>();

            Rect safeArea = Screen.safeArea;

            #if UNITY_EDITOR
            if (fakeNotchInEditor)
            {
                float notchHeight = safeArea.height * fakeNotchSize;
                safeArea.height -= notchHeight;

                GameObject fakeNotch = new GameObject("FakeNotch", typeof(CanvasRenderer), typeof(Image));

                fakeNotch.transform.SetParent(canvas.transform);

                fakeNotch.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);

                RectTransform fakeNotchRT = fakeNotch.GetComponent<RectTransform>();
                fakeNotchRT.sizeDelta = new Vector2(safeArea.width, notchHeight);
                fakeNotchRT.position = new Vector3(safeArea.width / 2f , Screen.height - (notchHeight / 2f));
            }
            #endif

            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= canvas.pixelRect.width;
            anchorMin.y /= canvas.pixelRect.height;
            anchorMax.x /= canvas.pixelRect.width;
            anchorMax.y /= canvas.pixelRect.height;

            for (int i = 0; i < safeAreaTransforms.Length; i++)
            {
                safeAreaTransforms[i].anchorMin = anchorMin;
                safeAreaTransforms[i].anchorMax = anchorMax;
            }
        }
    }
}