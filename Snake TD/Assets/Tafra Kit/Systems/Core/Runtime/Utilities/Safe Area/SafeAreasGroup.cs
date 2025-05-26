using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TafraKit
{
    [RequireComponent(typeof(Canvas))]
    public class SafeAreasGroup : MonoBehaviour
    {
        [SerializeField] private RectTransform[] safeAreaTransforms;

        public UnityEvent OnUpdated;

        private Canvas canvas;

        void Awake()
        {
            canvas = GetComponent<Canvas>();

            UpdateArea(true);

            TafraSafeAreas.OnAreaUpdated.AddListener(OnPropertiesUpdated);
        }

        private void OnPropertiesUpdated()
        {
            UpdateArea();
        }

        public void UpdateArea(bool trivial = false)
        {
            Rect safeArea = TafraSafeAreas.SafeArea;

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

            if (!trivial)
                OnUpdated?.Invoke();
        }
    }
}