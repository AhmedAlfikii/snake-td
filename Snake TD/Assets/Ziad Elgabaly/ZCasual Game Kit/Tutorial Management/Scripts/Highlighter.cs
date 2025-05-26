using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TafraKit;
using ZUI;
using System;

namespace ZCasualGameKit
{
    [Obsolete("Use TafraKit.UI.UIHighlighter instead.")]
    public class Highlighter : MonoBehaviour
    {
        public static Highlighter Instance;

        [Header("Settings")]
        [SerializeField] private int highlightLayerOrder;

        [Header("BG")]
        [SerializeField] private UIElement bgUIE;
        [SerializeField] private Canvas bgCanvas;

        [Header("Hand")]
        [SerializeField] private UIElement pointingHandUIE;
        [SerializeField] private Canvas pointingCanvas;

        private List<GameObject> highlightedUIObjects = new List<GameObject>();
        private float bgHidingDuration;

        void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        void Start()
        {
            bgCanvas.overrideSorting = true;
            bgCanvas.sortingOrder = highlightLayerOrder;
            pointingCanvas.overrideSorting = true;
            pointingCanvas.sortingOrder = highlightLayerOrder + 2;

            bgHidingDuration = bgUIE.GetAllHidingTime();
        }

        void ClearHighlightedList()
        {
            for (int i = 0; i < highlightedUIObjects.Count; i++)
            {
                Destroy(highlightedUIObjects[i].GetComponent<GraphicRaycaster>());
                Destroy(highlightedUIObjects[i].GetComponent<Canvas>());
            }

            highlightedUIObjects.Clear();
        }

        IEnumerator Highlighting(GameObject[] gos)
        {
            yield return Yielders.EndOfFrame;

            for (int i = 0; i < gos.Length; i++)
            {
                Canvas c = gos[i].AddComponent<Canvas>();
                c.overrideSorting = true;
                c.sortingOrder = highlightLayerOrder + 1;

                gos[i].AddComponent<GraphicRaycaster>();
            }
            highlightedUIObjects.AddRange(gos);

            bgUIE.ChangeVisibility(true);
        }

        IEnumerator Dehighlighting(List<GameObject> gosToDehighlight)
        {
            yield return Yielders.GetWaitForSeconds(bgHidingDuration);

            for (int i = 0; i < gosToDehighlight.Count; i++)
            {
                Destroy(gosToDehighlight[i].GetComponent<GraphicRaycaster>());
                Destroy(gosToDehighlight[i].GetComponent<Canvas>());
            }
        }

        public void HighlightUIObjects(GameObject[] gos)
        {
            //if (highlightedUIObjects.Count > 0)
            //    ClearHighlightedList();

            StartCoroutine(Highlighting(gos));
        }

        public void Dehighlight()
        {
            bgUIE.ChangeVisibility(false);

            //StartCoroutine(Dehighlighting(new List<GameObject>(highlightedUIObjects)));

            for (int i = 0; i < highlightedUIObjects.Count; i++)
            {
                Destroy(highlightedUIObjects[i].GetComponent<GraphicRaycaster>());
                Destroy(highlightedUIObjects[i].GetComponent<Canvas>());
            }

            highlightedUIObjects.Clear();
        }

        public void ShowHand(RectTransform targetObject)
        {
            ShowHand(targetObject.position);
        }

        public void ShowHand(Vector3 pos)
        {
            pointingHandUIE.transform.position = pos;
            pointingHandUIE.ChangeVisibility(true);
        }

        public void HideHand()
        {
            pointingHandUIE.ChangeVisibility(false);
        }
    }
}