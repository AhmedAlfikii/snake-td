using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Narrative
{
    public class SpeechInvoker : MonoBehaviour
    {
        [Header("Speech")]
        [SerializeField] private SpeechData speechData;
        [TextArea()]
        [SerializeField] private string speech;
        [SerializeField] private AudioClip voiceLine;
        [Range(0, 1)]
        [SerializeField] private float volume = 1;
        [SerializeField] private bool useVoiceLineDuration = true;
        [SerializeField] private float duration;

        [Space()]

        [SerializeField] private bool isOverHead;
        [SerializeField] private SpeakerTransform speaker;

        public void Speak()
        {
            CharacterIdentity _identity = speechData? speechData.Speaker : speaker.Identity;
            AudioClip _voiceLine = speechData? speechData.VoiceLine : voiceLine;
            string _speech = speechData ? speechData.Text : speech;
            bool _useVoiceLineDuration = speechData ? speechData.UseVoiceLineDuration : useVoiceLineDuration;
            float _customDuration = speechData ? speechData.CustomDuration : duration;
            float _volume = speechData ? speechData.Volume : volume;

            if (_voiceLine)
            {
                if (isOverHead)
                {
                    if (_useVoiceLineDuration)
                        Conversation.SpeakOverHead(speaker, _speech, _voiceLine, _volume);
                    else
                        Conversation.SpeakOverHead(speaker, _speech, _customDuration, _voiceLine, _volume);
                }
                else
                {
                    if (_useVoiceLineDuration)
                        Conversation.SpeakOnScreen(_identity, _speech, _voiceLine, _volume);
                    else
                        Conversation.SpeakOnScreen(_identity, _speech, _customDuration, _voiceLine, _volume);
                }
            }
            else
            {
                if(isOverHead)
                    Conversation.SpeakOverHead(speaker, _speech, _customDuration, _voiceLine, _volume);
                else
                    Conversation.SpeakOnScreen(_identity, _speech, _customDuration, _voiceLine, _volume);
            }
        }
    }
}