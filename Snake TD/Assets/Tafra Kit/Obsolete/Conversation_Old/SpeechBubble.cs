using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using TMPro;

namespace TafraKit
{
    public class SpeechBubble : MonoBehaviour
    {
        [SerializeField] private Speaker speaker;
        
        [Space()]

        [SerializeField] private float textStartDelay = 0.25f;
        [SerializeField] private float durationBetweenLetters = 0.035f;

        [Header("Bubble Holder")]
        [SerializeField] private Transform holder;
        [SerializeField] private float showDuration = 1f;
        [SerializeField] private EasingType showEasing;
        [SerializeField] private float hideDuration = 0.3f;
        [SerializeField] private EasingType hideEasing;

        [Header("References")]
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private TextIndentLevels textIndenter;


        private string speech;
        private int lettersCount;
        private float totalTime;
        private float typingDuration;
        private float hideStartTime;

        public void OnSpeechStart()
        {
            lettersCount = speech.Length;
            
            typingDuration = lettersCount * durationBetweenLetters;

            hideStartTime = totalTime - hideDuration;

            //text.useMaxVisibleDescender = false;
         
            textIndenter.AdaptTextToIndentLevels(speech);
        }

        public void OnSpeechStop()
        {

        }

        public void SetTime(double time)
        {
            float floatTime = (float)time;

            #region Typing
            int passedLetters = 0;

            if (durationBetweenLetters > 0.001f)
                passedLetters = Mathf.FloorToInt((floatTime - textStartDelay) / durationBetweenLetters);
            else
            {
                if (floatTime > textStartDelay)
                    passedLetters = lettersCount;
            }

            if (passedLetters < 0)
                passedLetters = 0;
            text.maxVisibleCharacters = passedLetters;
            #endregion

            #region Show Animation
            if (floatTime < hideStartTime)
            {
                float localShowTime = Mathf.Min(floatTime, showDuration);

                float t = localShowTime / showDuration;
                t = MotionEquations.GetEaseFloat(t, showEasing);

                holder.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, t);
            }
            #endregion

            #region Hide Animation
            if (floatTime > hideStartTime)
            {
                float localHideTime = Mathf.Min(floatTime - hideStartTime, hideDuration);
                float t = localHideTime / hideDuration;
                t = MotionEquations.GetEaseFloat(t, hideEasing);

                holder.localScale = Vector3.LerpUnclamped(Vector3.one, Vector3.zero, t);
            }
            #endregion
        }

        public void SetText(string speechText)
        {
            speech = speechText;
        }
        public void SetDuration(float duration)
        {
            totalTime = duration;
            hideStartTime = duration - hideDuration;
        }

        public void ResetVisuals()
        {
            holder.localScale = Vector3.one;
            text.text = "-";
        }
    }
}