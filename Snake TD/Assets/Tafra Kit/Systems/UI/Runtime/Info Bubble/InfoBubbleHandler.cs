using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.Internal.UI;
using System.Threading.Tasks;
using System;
using JetBrains.Annotations;
using TafraKit.Internal;

namespace TafraKit.UI
{
    //Make sure the following is implemented:
    //Any bubble has it's pivot centered.
    //Any bubble arrow has it's pivot at (0.5, 0).
    //TODO: Support multiple bubbles visible at the same time. And pool bubbles instead of instantly hiding the bubble to show it for another rect.
    public static class InfoBubbleHandler
    {
        private static InfoBubbleSettings settings;
        private static bool enabled;
        private static Canvas bubblesCanvas;
        private static List<InfoBubble> bubbles = new List<InfoBubble>();
        private static InfoBubble visibleBubble;
        private static RectTransform visibleBubbleRect;
        private static float paddingLeft = 20;
        private static float paddingRight = 20;
        private static float paddingTop = 20;
        private static float paddingBottom = 20;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            settings = TafraSettings.GetSettings<InfoBubbleSettings>();
           
            enabled = settings.Enabled;

            if (settings == null || !enabled)
                return;
            
            bubblesCanvas = GameObject.Instantiate(settings.BubblesCanvas);

            GameObject.DontDestroyOnLoad(bubblesCanvas.gameObject);

            for (int i = 0; i < settings.Bubbles.Length; i++)
            {
                GameObject bubbleGO = GameObject.Instantiate(settings.Bubbles[i].gameObject, bubblesCanvas.transform);

                bubbleGO.gameObject.SetActive(false);

                InfoBubble infoBubble = bubbleGO.GetComponent<InfoBubble>();
                
                infoBubble.Hide(true);

                bubbles.Add(infoBubble);
            }

            paddingLeft = settings.PaddingLeft;
            paddingRight = settings.PaddingRight;
            paddingTop = settings.PaddingTop;
            paddingBottom = settings.PaddingBottom;
        }

        /// <summary>
        /// Display an info bubble.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="side"></param>
        /// <param name="info"></param>
        /// <param name="title"></param>
        public static void Show(RectTransform rect, Side side, string info, string title = null)
        {
            if (!enabled)
            {
                TafraDebugger.Log("Info Bubble Handler", "Info Bubble Handler is not enabled, enable it in Tafra Kit in order to use it.", TafraDebugger.LogType.Error);
                return;
            }
            
            if (visibleBubble)
            {
                bool isVisibleBubbleRect = visibleBubbleRect == rect;
                
                visibleBubble.Hide(!isVisibleBubbleRect);   //Hide instantly if it's not the visible bubble rect.

                visibleBubble = null;
                visibleBubbleRect = null;

                if (isVisibleBubbleRect)
                    return;
            }

            InfoBubble bubble = bubbles[0];

            RectTransform bubbleRT = bubble.MainRT;
            RectTransform arrowRT = bubble.GetArrow(side);

            bubble.SetInfo(info, title);
            
            bubble.gameObject.SetActive(true);
            
            //To give unity's UI system a chance to adapt to the change in the layout group and the content size fitter of the bubble.
            GeneralCoroutinePlayer.StartCoroutine(PositionBubbleDelayed(rect, bubble, bubbleRT, arrowRT, side));
        }
        private static IEnumerator PositionBubbleDelayed(RectTransform rect, InfoBubble bubble, RectTransform bubbleRT, RectTransform arrowRT, Side side)
        {
            yield return null;

            bubble.RefreshLayoutGroup();

            float bubbleLossyScale = bubblesCanvas.transform.lossyScale.x;
            Vector2 bubbleSize = new Vector2(bubbleRT.rect.width * bubbleLossyScale, bubbleRT.rect.height * bubbleLossyScale);
            Vector2 halfBubbleSize = new Vector2(bubbleSize.x / 2f, bubbleSize.y / 2f);
            float arrowLength = 0;

            if(side == Side.Top || side == Side.Bottom)
                arrowLength = arrowRT.rect.height * bubbleLossyScale;
            else if(side == Side.Left || side == Side.Right)
                arrowLength = arrowRT.rect.width * bubbleLossyScale;

            Vector2 bubblePosition;
            Vector2 arrowPosition;

            GetBubbleUnclampedPosition(rect, bubbleRT, arrowRT, side, bubbleSize, halfBubbleSize, arrowLength, out bubblePosition, out arrowPosition);
            bubblePosition = GetBubbleClampedPosition(bubblePosition, halfBubbleSize, side);

            bubbleRT.position = bubblePosition;
            arrowRT.position = arrowPosition;

            bubble.Show(false);

            visibleBubble = bubble;
            visibleBubbleRect = rect;

            ListenToMouseDown();

        }
        /// <summary>
        /// Hide the info bubble that was visible for a specific rect.
        /// </summary>
        /// <param name="rect">The rect that an info bubble was displayed for.</param>
        public static void Hide()
        {
            if (visibleBubble)
            {
                visibleBubble.Hide(false);

                visibleBubble = null;
                visibleBubbleRect = null;
            }
        }

        private static async Task ListenToMouseDown()
        {
            try
            {
                await Task.Yield();

                while(true)
                {
                    if(Input.GetMouseButtonDown(0))
                    {
                        Hide();
                        break;
                    }

                    await Task.Yield();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
        private static void GetBubbleUnclampedPosition(RectTransform rect, RectTransform bubbleRT, RectTransform arrowRT, Side side, Vector2 bubbleSize, Vector2 halfBubbleSize, float arrowLength , out Vector2 bubblePosition, out Vector2 arrowPosition)
        {
            float rectLossyScale = rect.lossyScale.x;

            Vector2 rectSize = new Vector2(rect.rect.width * rectLossyScale, rect.rect.height * rectLossyScale);

            Vector2 centerPosition = new Vector2(rect.position.x - ((rect.pivot.x - 0.5f) * rectSize.x), rect.position.y - ((rect.pivot.y - 0.5f) * rectSize.y));

            bubblePosition = centerPosition;

            arrowPosition = Vector3.zero;

            switch (side)
            {
                case Side.Left:
                    bubblePosition.x -= rectSize.x / 2f; //Rect body.
                    bubblePosition.x -= (1 - bubbleRT.pivot.x) * bubbleSize.x;   //Bubble body.
                    bubblePosition.x -= arrowLength; //Bubble arrow.

                    arrowPosition = bubblePosition + new Vector2(halfBubbleSize.x + arrowLength, 0);
                    break;
                case Side.Right:
                    bubblePosition.x += rectSize.x / 2f; //Rect body.
                    bubblePosition.x += bubbleRT.pivot.x * bubbleSize.x; //Bubble body.
                    bubblePosition.x += arrowLength;

                    arrowPosition = bubblePosition - new Vector2(halfBubbleSize.x + arrowLength, 0);
                    break;
                case Side.Top:
                    bubblePosition.y += rectSize.y / 2f; //Rect body.
                    bubblePosition.y += bubbleRT.pivot.y * bubbleSize.y; //Bubble body.
                    bubblePosition.y += arrowLength; //Bubble arrow.

                    arrowPosition = bubblePosition - new Vector2(0, halfBubbleSize.y + arrowLength);
                    break;
                case Side.Bottom:
                    bubblePosition.y -= rectSize.y / 2f; //Rect body.
                    bubblePosition.y -= (1 - bubbleRT.pivot.y) * bubbleSize.y;   //Bubble body.
                    bubblePosition.y -= arrowLength; //Bubble arrow.

                    arrowPosition = bubblePosition + new Vector2(0, halfBubbleSize.y + arrowLength);
                    break;
            }
        }
        private static Vector3 GetBubbleClampedPosition(Vector2 bubblePosition, Vector2 halfBubbleSize, Side side)
        {
            float minX = bubblePosition.x - halfBubbleSize.x;
            float maxX = bubblePosition.x + halfBubbleSize.x;
            float minY = bubblePosition.y - halfBubbleSize.y;
            float maxY = bubblePosition.y + halfBubbleSize.y;

            Vector2 pushVector = Vector2.zero;

            switch (side)
            {
                case Side.Left:
                case Side.Right:
                    if(minY < paddingBottom)
                    {
                        if (minY < 0)
                            pushVector.y = minY * -1 + paddingBottom;
                        else
                            pushVector.y = paddingBottom + minY;
                    }
                    else if(maxY > (Screen.height - paddingTop))
                        pushVector.y = (Screen.height - paddingTop) - maxY;
                    break;
                case Side.Top:
                case Side.Bottom:
                    if(minX < paddingLeft)
                    {
                        if(minX < 0)
                            pushVector.x = minX * -1 + paddingLeft;
                        else
                            pushVector.x = paddingLeft + minX;
                    }
                    else if(maxX > (Screen.width - paddingRight))
                        pushVector.x = (Screen.width - paddingRight) - maxX;
                    break;
            }

            bubblePosition += pushVector;

            return bubblePosition;
        }
    }
}
