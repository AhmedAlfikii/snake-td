using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Narrative
{
    public class SpeechData : ScriptableObject
    {
        [SerializeField] private AudioClip voiceline;
        [TextArea]
        [SerializeField] private string text;
        [SerializeField] private bool useVoiceLineDuration = true;
        [SerializeField] private float customDuration = 1.5f;
        [Range(0f, 1f)]
        [SerializeField] private float volume = 1f;
        [Space]

        [Tooltip("If you're invoking the speech through this speech data, then you need to assign a speaker (can only be used for on screen speeches)")]
        [SerializeField] private CharacterIdentity speaker;

        public AudioClip VoiceLine => voiceline;
        public string Text => text;
        public bool UseVoiceLineDuration => useVoiceLineDuration;
        public float CustomDuration => customDuration;
        public float Volume => volume;
        public CharacterIdentity Speaker => speaker;

        private void StartSpeaking(Action onFinish = null)
        {
            if(speaker == null)
            {
                TafraDebugger.Log("Speech Data", "Can't speak while there's no speaker assigned to this data.", TafraDebugger.LogType.Error, this);
                return;
            }

            float _duration = useVoiceLineDuration ? voiceline.length : customDuration;

            Conversation.SpeakOnScreen(speaker, text, _duration, voiceline, volume, onFinish);
        }

        public void Speak()
        {
            StartSpeaking();
        }
        public void Speak(Action onFinish)
        {
            StartSpeaking(onFinish);
        }

        public void EditorInitialize(AudioClip voiceline)
        {
            this.voiceline = voiceline;
        }
    }
}