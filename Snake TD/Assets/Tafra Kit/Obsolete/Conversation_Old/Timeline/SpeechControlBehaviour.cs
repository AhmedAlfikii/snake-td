using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace TafraKit
{
    [Serializable]
    public class SpeechControlBehaviour : PlayableBehaviour
    {
        [SerializeField] private string msg = "Message";

        public float HideDelay
        {
            get { return hideDelay; }
            set { hideDelay = value; }
        }

        private SpeechBubble speechBubble;
        private float hideDelay;
        private bool processedFirstFrame;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (speechBubble == null)
                speechBubble = playerData as SpeechBubble;

            if(speechBubble == null)
                return;

            if (!processedFirstFrame)
            {
                speechBubble.gameObject.SetActive(true);
                speechBubble.SetDuration((float)playable.GetDuration());
                speechBubble.SetText(msg);
                speechBubble.OnSpeechStart();

                processedFirstFrame = true;
            }

            speechBubble.SetTime(playable.GetTime());
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if(speechBubble == null)
                return;

            speechBubble.OnSpeechStop();
            speechBubble.ResetVisuals();
            speechBubble.gameObject.SetActive(false);

            processedFirstFrame = false;
        }
    }
}