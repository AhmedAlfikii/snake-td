using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Narrative
{
    public static class Conversation
    {
        private static ConversationSettings settings;
        private static bool isEnabled;
        private static Canvas speechContainersCanvas;
        private static SpeechContainer onScreenSpeechContainer;
        private static DynamicPool<SpeechContainer> overHeadSpeechContainersPool = new DynamicPool<SpeechContainer>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            settings = TafraSettings.GetSettings<ConversationSettings>();

            if(settings == null || !settings.Enabled)
                return;

            isEnabled = true;

            speechContainersCanvas = GameObject.Instantiate(settings.speechContainersCanvas);
            GameObject.DontDestroyOnLoad(speechContainersCanvas);

            //We assume that the first child of the canvas is a safe area so we use it as the root.
            Transform root = speechContainersCanvas.transform.childCount > 0 ? speechContainersCanvas.transform.GetChild(0) : speechContainersCanvas.transform;

            if (settings.overHeadSpeechContainer != null)
            {
                overHeadSpeechContainersPool.Holder = root;

                SpeechContainer sampleOverHeadSpeechContainer = GameObject.Instantiate(settings.overHeadSpeechContainer, root);

                overHeadSpeechContainersPool.AddUnit(sampleOverHeadSpeechContainer);

                overHeadSpeechContainersPool.Initialize();
            }

            if (settings.onScreenSpeechContainer != null)
            {
                onScreenSpeechContainer = GameObject.Instantiate(settings.onScreenSpeechContainer, root);
                onScreenSpeechContainer.gameObject.SetActive(false);
            }
        }

        public static void SpeakOverHead(SpeakerTransform speakerTransform, string speech, float duration, AudioClip voiceLine, float volume = 1)
        {
            if(!isEnabled)
            {
                TafraDebugger.Log("Conversation", "Conversation isn't enabled, please enable it in Tafra Kit window.", TafraDebugger.LogType.Error);
                return;
            }

            SpeechContainer container = overHeadSpeechContainersPool.RequestUnit();

            container.Speak(speakerTransform, speech, duration, voiceLine, volume, () =>
            {
                overHeadSpeechContainersPool.ReleaseUnit(container);
            });
        }
        public static void SpeakOverHead(SpeakerTransform speakerTransform, string speech, AudioClip voiceLine, float volume = 1)
        {
            if(!isEnabled)
            {
                TafraDebugger.Log("Conversation", "Conversation isn't enabled, please enable it in Tafra Kit window.", TafraDebugger.LogType.Error);
                return;
            }

            SpeechContainer container = overHeadSpeechContainersPool.RequestUnit();

            float duration = voiceLine != null? voiceLine.length : speech.Length * settings.DurationBetweenLetters;

            container.Speak(speakerTransform, speech, duration, voiceLine, volume, () =>
            {
                overHeadSpeechContainersPool.ReleaseUnit(container);
            });
        }

        public static void SpeakOnScreen(CharacterIdentity speaker, string speech, float duration, AudioClip voiceLine, float volume = 1, Action onFinish = null)
        {
            if (!isEnabled)
            {
                TafraDebugger.Log("Conversation", "Conversation isn't enabled, please enable it in Tafra Kit window.", TafraDebugger.LogType.Error);
                return;
            }

            onScreenSpeechContainer.gameObject.SetActive(true);

            onScreenSpeechContainer.Speak(speaker, speech, duration, voiceLine, volume, onDeactivate: () =>
            {
                onScreenSpeechContainer.gameObject.SetActive(false);
                onFinish?.Invoke();
            });
        }
        public static void SpeakOnScreen(CharacterIdentity speaker, string speech, AudioClip voiceLine, float volume = 1)
        {
            if (!isEnabled)
            {
                TafraDebugger.Log("Conversation", "Conversation isn't enabled, please enable it in Tafra Kit window.", TafraDebugger.LogType.Error);
                return;
            }

            onScreenSpeechContainer.gameObject.SetActive(true);

            float duration = voiceLine != null ? voiceLine.length : speech.Length * settings.DurationBetweenLetters;

            onScreenSpeechContainer.Speak(speaker, speech, duration, voiceLine, volume, onDeactivate: () =>
            {
                onScreenSpeechContainer.gameObject.SetActive(false);
            });
        }

    }
}