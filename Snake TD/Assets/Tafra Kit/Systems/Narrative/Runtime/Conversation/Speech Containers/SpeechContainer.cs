using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace TafraKit.Narrative
{
    public class SpeechContainer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected TextMeshProUGUI speechTXT;
        [SerializeField] protected TextMeshProUGUI speakerNameTXT;
        [SerializeField] protected Image speakerAvatar;

        [Header("Properties")]
        [SerializeField] protected float typingStartDelay = 0.25f;
        [SerializeField] protected float durationBetweenLetters = 0.035f;
        [SerializeField] protected float hideDelay = 1f;

        [Header("Animation (Scale)")]
        [SerializeField] protected float showDuration = 1f;
        [SerializeField] protected EasingType showEasing;
        [SerializeField] protected Vector3 hidingScale;
        [SerializeField] protected float hideDuration = 0.3f;
        [SerializeField] protected EasingType hideEasing;

        protected SpeakerTransform speakerTransform;
        protected IdentifiableScriptableObject speakerIdentity;
        protected bool isSpeaking;
        protected string speech;
        protected int lettersCount;
        protected float speechStartTime;
        protected float typingStartTime;
        protected float typingEndTime;
        protected float typingDuration;
        protected float hideStartTime;
        protected float totalSpeechDuration;
        private AudioClip voiceLine;
        protected float volume;
        protected int voiceLineSFXID;
        protected Action onDeactivate;
        protected bool playedVoiceLine;

        private void Update()
        {
            if(!isSpeaking)
                return;

            double passedTime = Time.unscaledTime - speechStartTime;

            Tick(passedTime);

            if(passedTime > totalSpeechDuration)
            {
                Deactivate();
            }
        }

        public void Speak(SpeakerTransform speakerTransform, string speech, float typingDuration, AudioClip voiceLine, float volume, Action onDeactivate)
        {
            this.speakerTransform = speakerTransform;

            InitializeSpeech(speech, speakerTransform.Identity, typingDuration, voiceLine, volume, onDeactivate);

            OnSpeechStart();
        }
        public void Speak(CharacterIdentity speakerIdentity, string speech, float typingDuration, AudioClip voiceLine, float volume, Action onDeactivate)
        {
            this.speakerIdentity = speakerIdentity;

            speakerTransform = null;
           
            InitializeSpeech(speech, speakerIdentity, typingDuration, voiceLine, volume, onDeactivate);

            OnSpeechStart();
        }

        public void Tick(double passedTime)
        {
            float floatPassedTime = (float)passedTime;

            #region Voice Line
            if (!playedVoiceLine && Time.unscaledTime > typingStartTime)
            {
                playedVoiceLine = true;

                if(voiceLine != null)
                    voiceLineSFXID = SFXPlayer.Play(voiceLine, volume);
                else
                    voiceLineSFXID = -1;
            }
            #endregion

            #region Typing
            float passedPercentage = Mathf.Clamp01((Time.unscaledTime - typingStartTime) / typingDuration);
            int passedLetters = Mathf.FloorToInt(passedPercentage * lettersCount);

            if(passedLetters < 0)
                passedLetters = 0;

            speechTXT.maxVisibleCharacters = passedLetters;
            #endregion

            #region Show Animation
            if(Time.unscaledTime < hideStartTime)
            {
                float localShowTime = Mathf.Min(floatPassedTime, showDuration);

                float t = localShowTime / showDuration;
                t = MotionEquations.GetEaseFloat(t, showEasing);

                transform.localScale = Vector3.LerpUnclamped(hidingScale, Vector3.one, t);
            }
            #endregion

            #region Hide Animation
            if(Time.unscaledTime > hideStartTime)
            {
                float localHideTime = Mathf.Min(Time.unscaledTime - hideStartTime, hideDuration);
                float t = localHideTime / hideDuration;
                t = MotionEquations.GetEaseFloat(t, hideEasing);

                transform.localScale = Vector3.LerpUnclamped(Vector3.one, hidingScale, t);
            }
            #endregion

            OnTick();
        }

        private void InitializeSpeech(string speech, CharacterIdentity speakerIdentity, float typingDuration, AudioClip voiceLine, float volume, Action onDeactivate)
        {
            lettersCount = speech.Length;

            this.voiceLine = voiceLine;
            this.volume = volume;
            this.speech = speech;
            this.onDeactivate = onDeactivate;
            this.speakerIdentity = speakerIdentity;

            this.typingDuration = typingDuration;

            speechStartTime = Time.unscaledTime;
            typingStartTime = speechStartTime + typingStartDelay;
            typingEndTime = typingStartTime + typingDuration;
            
            hideStartTime = typingEndTime + hideDelay;

            float speechEndTime = hideStartTime + hideDuration;

            totalSpeechDuration = speechEndTime - speechStartTime;

            if (speakerNameTXT)
                speakerNameTXT.text = speakerIdentity.DisplayName;

            if (speakerAvatar)
                speakerAvatar.sprite = speakerIdentity.GetIconIfLoaded();

            speechTXT.text = speech;

            transform.localScale = hidingScale;
            
            playedVoiceLine = false;

            isSpeaking = true;
        }
        private void Deactivate()
        {
            isSpeaking = false;

            if(voiceLineSFXID != -1)
                SFXPlayer.Stop(voiceLineSFXID);

            onDeactivate?.Invoke();

            OnDeactivate();
        }

        protected virtual void OnSpeechStart() { }
        protected virtual void OnTick() { }
        protected virtual void OnDeactivate() { }
    }
}