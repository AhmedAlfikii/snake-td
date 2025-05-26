using System.Collections;
using System.Collections.Generic;
using TafraKit.Internal.UI;
using TafraKit.MotionFactory;
using UnityEngine;
using UnityEngine.UI;

namespace TafraKit.UI
{
    /// <summary>
    /// Highlights UI objects by increasing their sorting order to a high number and fading everything behind them by displaying a black transparent background.
    /// </summary>
    public static class UIHighlighter
    {
        public struct HighlightedObjectData
        {
            public Canvas canvas;
            public GraphicRaycaster graphicRaycaster;
            public bool componentsAreAdded;
            public int originalSortingOrder;
        }

        private static UIHighlighterSettings settings;
        private static VisibilityMotionController bgMotionController;
        private static int highlightSortingOrder;
        private static RectTransform pointerRT;
        private static VisibilityMotionController pointerMotionController;
        private static float pointerWidth;
        private static float pointerHeight;

        /// <summary>
        /// Contains all the currently highlighted objects and their data.
        /// </summary>
        private static Dictionary<GameObject, HighlightedObjectData> highlightedObjectsData = new Dictionary<GameObject, HighlightedObjectData>();
        private static List<GameObject> tempHighlightedObjects = new List<GameObject>();
        private static RectTransform pointerTarget;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            settings = TafraSettings.GetSettings<UIHighlighterSettings>();

            if(settings == null || !settings.Enabled)
                return;

            highlightSortingOrder = settings.SortingOrder;

            GameObject highlighterCanvasGO = Object.Instantiate(settings.HighlightBG);

            Object.DontDestroyOnLoad(highlighterCanvasGO);

            Transform bgTrans = highlighterCanvasGO.transform.Find("HighlighterBG");

            if(bgTrans != null)
            {
                bgMotionController = bgTrans.GetComponent<VisibilityMotionController>();

                Canvas bgCanvas = bgTrans.GetComponent<Canvas>();
                bgCanvas.overrideSorting = true;
                bgCanvas.sortingOrder = highlightSortingOrder - 1;
            }
            else
            {
                TafraDebugger.Log("UIHighlighter", "The highlighter canvas you assigned doesn't have a child called \"HighlighterBG\". Make sure to add one. " +
                    "It should have the following components: VisibilityMotionController, Canvas and GraphicRayCaster.", TafraDebugger.LogType.Error);
            }

            Transform pointerTrans = highlighterCanvasGO.transform.Find("Pointer");

            if(pointerTrans != null)
            {
                pointerRT = pointerTrans.GetComponent<RectTransform>();
                pointerMotionController = pointerTrans.GetChild(0).GetComponent<VisibilityMotionController>();

                pointerWidth = pointerRT.rect.width;
                pointerHeight = pointerRT.rect.height;

                Canvas pointerCanvas = pointerTrans.GetComponent<Canvas>();
                pointerCanvas.overrideSorting = true;
                pointerCanvas.sortingOrder = highlightSortingOrder + 1;
            }
        }

        public static void Highlight(GameObject target)
        {
            //If the target is already highlighted, then no need to highlight it again.
            if(highlightedObjectsData.ContainsKey(target))
                return;
            
            HighlightedObjectData data = new HighlightedObjectData();

            Canvas canvas = target.GetComponent<Canvas>();
            if(canvas != null)
            {
                data.canvas = canvas;

                data.originalSortingOrder = canvas.sortingOrder;
                canvas.sortingOrder = highlightSortingOrder;
            }
            else
            {
                Canvas newCanvas = target.AddComponent<Canvas>();
                newCanvas.overrideSorting = true;
                newCanvas.sortingOrder = highlightSortingOrder;
                data.canvas = newCanvas;

                data.graphicRaycaster = target.AddComponent<GraphicRaycaster>();

                data.componentsAreAdded = true;
            }

            highlightedObjectsData.Add(target, data);

            //If this is the first object to be highlighted, then we need to show the background.
            if(highlightedObjectsData.Count == 1)
                bgMotionController.Show();
        }
        public static void RemoveHighlight(GameObject target)
        {
            if(!highlightedObjectsData.TryGetValue(target, out HighlightedObjectData highlightedObjectData))
                return;

            if(highlightedObjectData.componentsAreAdded)
            {
                Object.Destroy(highlightedObjectData.graphicRaycaster);
                Object.Destroy(highlightedObjectData.canvas);
            }
            else
                highlightedObjectData.canvas.sortingOrder = highlightedObjectData.originalSortingOrder;

            highlightedObjectsData.Remove(target);

            //If this is the last highlighted object, then we need to hide the background.
            if(highlightedObjectsData.Count == 0)
                bgMotionController.Hide();
        }
        public static void RemoveAllHighlightedObjects()
        {
            foreach (var item in highlightedObjectsData)
            {
                tempHighlightedObjects.Add(item.Key);
            }

            for (int i = 0; i < tempHighlightedObjects.Count; i++)
            {
                RemoveHighlight(tempHighlightedObjects[i]);
            }

            tempHighlightedObjects.Clear();
        }

        public static void PointAt(RectTransform target)
        {
            float lossyScale = target.lossyScale.x;

            float screenLossyWidth = Screen.width / lossyScale;
            float screenLossyHeight = Screen.height / lossyScale;

            pointerTarget = target;

            bool freeRightSpace = true;
            bool freeLeftSpace = true;
            bool freeTopSpace = true;
            bool freeBottomSpace = true;
            
            Vector3 targetPosition = target.position / lossyScale;

            float targetWidth = target.rect.width;
            float targetHeight = target.rect.height;

            Vector3 halfSize = new Vector3((targetWidth / 2f), (targetHeight / 2f), 0);

            Vector3 targetMinPoint = targetPosition - halfSize;
            Vector3 targetMaxPoint = targetPosition + halfSize;

            freeRightSpace = screenLossyWidth - targetMaxPoint.x > pointerWidth;
            freeLeftSpace = targetMinPoint.x > pointerWidth;
            freeTopSpace = screenLossyHeight - targetMaxPoint.y > pointerHeight;
            freeBottomSpace = targetMinPoint.y > pointerHeight;

            if (freeBottomSpace && freeRightSpace)
            {
                pointerRT.transform.position = target.transform.position + new Vector3((targetWidth / 4f) * lossyScale, -(targetHeight / 2f) * lossyScale, 0);
                pointerRT.transform.localScale = new Vector3(1, 1, 1);
            }
            else if (freeBottomSpace && freeLeftSpace)
            {
                pointerRT.transform.position = target.transform.position + new Vector3(-(targetWidth / 4f) * lossyScale, -(targetHeight / 2f) * lossyScale, 0);
                pointerRT.transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (freeTopSpace && freeLeftSpace)
            {
                pointerRT.transform.position = target.transform.position + new Vector3(-(targetWidth / 4f) * lossyScale, (targetHeight / 2f) * lossyScale, 0);
                pointerRT.transform.localScale = new Vector3(-1, -1, 1);
            }
            else if (freeTopSpace && freeRightSpace)
            {
                pointerRT.transform.position = target.transform.position + new Vector3((targetWidth / 4f) * lossyScale, (targetHeight / 2f) * lossyScale, 0);
                pointerRT.transform.localScale = new Vector3(1, -1, 1);
            }
            else
            {
                pointerRT.transform.position = target.transform.position;
                pointerRT.transform.localScale = new Vector3(1, 1, 1);
            }

            if (!pointerMotionController.IsVisible)
                pointerMotionController.Show();
        }
        public static void RemovePointer()
        {
            pointerTarget = null;

            if (pointerMotionController.IsVisible)
                pointerMotionController.Hide();
        }
    }
}