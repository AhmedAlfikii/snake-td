using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TafraKit
{
    public class TouchBlocker
    {
        private static GameObject blockingGO;
        private static ControlReceiver blockingReceiver;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            blockingReceiver = new ControlReceiver(OnFirstBlockerAdded, null, OnAllBlockersCleared);

            GameObject canvasGO = new GameObject("TouchBlocker_Canvas", typeof(Canvas), typeof(GraphicRaycaster));

            Canvas canvas = canvasGO.GetComponent<Canvas>();

            canvas.sortingOrder = 30000;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            GameObject blockingImageGO = new GameObject("Blocker", typeof(CanvasRenderer), typeof(Image));
            blockingImageGO.transform.SetParent(canvasGO.transform);
            
            blockingImageGO.GetComponent<Image>().color = new Color(0, 0, 0, 0);

            RectTransform blockingImageRT = blockingImageGO.GetComponent<RectTransform>();
            blockingImageRT.AnchorsAndRectFillParent();

            GameObject.DontDestroyOnLoad(canvasGO);
            
            canvasGO.SetActive(false);

            blockingGO = canvasGO;
        }

        private static void OnFirstBlockerAdded()
        {
            if(!blockingGO)
                return;

            blockingGO.gameObject.SetActive(true);
        }
        private static void OnAllBlockersCleared()
        {
            if(!blockingGO)
                return;

            blockingGO.gameObject.SetActive(false);
        }

        public static void Block(string blocker)
        {
            blockingReceiver.AddController(blocker);
        }
        public static void Unblock(string blocker)
        {
            blockingReceiver.RemoveController(blocker);
        }
    }
}