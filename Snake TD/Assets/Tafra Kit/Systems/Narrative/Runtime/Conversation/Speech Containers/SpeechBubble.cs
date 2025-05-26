using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Narrative
{
    public class SpeechBubble : SpeechContainer
    {
        [SerializeField] private RectTransform mainRT;
        [SerializeField] private RectTransform arrowRT;

        private Camera mainCam;
        private Transform speakerHead;

        private void Awake()
        {
            mainCam = Camera.main;
        }

        protected override void OnTick()
        {
            if(!speakerHead || !mainCam)
                return;

            Vector2 headScreenPosition = mainCam.WorldToScreenPoint(speakerHead.position);
            mainRT.position = headScreenPosition;
        }
        protected override void OnSpeechStart()
        {
            speakerHead = speakerTransform != null ? speakerTransform.Head : null;
        }
    }
}