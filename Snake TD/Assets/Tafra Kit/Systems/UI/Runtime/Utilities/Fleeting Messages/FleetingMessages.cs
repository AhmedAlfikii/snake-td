using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit;
using TafraKit.Internal.UI;
using TafraKit.Internal;

namespace TafraKit.UI
{
    public static class FleetingMessages
    {
        private static DynamicPool<FleetingMessage> fleetingMessagesPool = new DynamicPool<FleetingMessage>();
        private static Canvas canvas;
        private static RectTransform placementPoint;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            GameObject fleetingMessageCanvasPrefab = Resources.Load<GameObject>("UI/FleetingMessages_Canvas");

            if(fleetingMessageCanvasPrefab == null)
                return;

            if(fleetingMessageCanvasPrefab.transform.childCount < 2)
                return;

            fleetingMessageCanvasPrefab.transform.GetChild(0).gameObject.SetActive(false);

            GameObject canvasGO = GameObject.Instantiate(fleetingMessageCanvasPrefab);
            

            fleetingMessageCanvasPrefab.transform.GetChild(0).gameObject.SetActive(true);

            FleetingMessage fleetingMessage = canvasGO.transform.GetChild(0).GetComponent<FleetingMessage>();

            if(fleetingMessage == null)
            {
                GameObject.Destroy(canvasGO);
                return;
            }

            canvas = canvasGO.GetComponent<Canvas>();

            placementPoint = canvasGO.transform.GetChild(1).GetComponent<RectTransform>();

            GameObject.DontDestroyOnLoad(canvasGO);

            fleetingMessagesPool.Holder = canvasGO.transform;
            fleetingMessagesPool.AddUnit(fleetingMessage);

            fleetingMessagesPool.Initialize();
        }

        public static void Show(string message)
        {
            FleetingMessage fleetingMessage = fleetingMessagesPool.RequestUnit(activateUnit: true);
            
            fleetingMessage.SetText(message);

            GeneralCoroutinePlayer.StartCoroutine(ShowingFleetingMessage(fleetingMessage));
        }

        private static IEnumerator ShowingFleetingMessage(FleetingMessage fleetingMessage)
        {
            fleetingMessage.CanvasGroup.alpha = 1;
            
            fleetingMessage.transform.SetAsLastSibling();

            fleetingMessage.gameObject.SetActive(true);

            float startTime = Time.unscaledTime;
            float duration = 0.35f;
            float endTime = startTime + duration;

            Vector3 startPos = placementPoint.position;
            startPos.y -= Screen.height * 0.1f;

            fleetingMessage.transform.position = startPos;

            Vector3 targetPos = placementPoint.transform.position;

            CanvasGroup canvasGroup = fleetingMessage.CanvasGroup;

            while(Time.unscaledTime < endTime)
            { 
                float t = (Time.unscaledTime - startTime) / duration;
                
                t = MotionEquations.EaseOut(t);

                fleetingMessage.transform.position = Vector3.Lerp(startPos, targetPos, t);
                canvasGroup.alpha = Mathf.Lerp(0, 1, t);

                yield return null;
            }
            fleetingMessage.transform.position = targetPos;
            canvasGroup.alpha = 1;

            yield return Yielders.GetWaitForSecondsRealtime(1f);

            startTime = Time.unscaledTime;
            duration = 0.75f;
            endTime = startTime + duration;

            while(Time.unscaledTime < endTime)
            {
                float t = (Time.unscaledTime - startTime) / duration;

                t = MotionEquations.EaseIn(t);

                canvasGroup.alpha = Mathf.Lerp(1, 0, t);

                yield return null;
            }
            canvasGroup.alpha = 0;

            fleetingMessagesPool.ReleaseUnit(fleetingMessage);
        }
    }
}