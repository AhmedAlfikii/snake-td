using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Narrative
{
    public class SpeechTrigger : MonoBehaviour
    {
        [Header("Speech")]
        [SerializeField] private SpeechData speechData;
        [SerializeField] private string speech;
        [SerializeField] private AudioClip voiceLine;
        [Range(0, 1)]
        [SerializeField] private float volume = 1;
        [SerializeField] private bool useVoiceLineDuration = true;
        [SerializeField] private float duration;
  
        [Space()]
     
        [SerializeField] private bool isOverHead;
        [SerializeField] private bool triggererIsSpeaker = true;
        [SerializeField] private SpeakerTransform speaker;

        [Header("Detection")]
        [SerializeField] private LayerMask layerMask;

        private void OnTriggerEnter(Collider other)
        {
            if (layerMask != (layerMask | (1 << other.gameObject.layer)))
                return;

            SpeakerTransform speakerTransform;

            if (triggererIsSpeaker)
            {
                speakerTransform = ComponentProvider.GetComponent<SpeakerTransform>(other.gameObject);

                if (!speakerTransform)
                    return;
            }
            else
            {
                speakerTransform = speaker;
            }

            CharacterIdentity _identity = speechData? speechData.Speaker : speaker.Identity;
            string _speech = speechData? speechData.Text : speech;
            AudioClip _voiceline = speechData ? speechData.VoiceLine : voiceLine;
            bool _useVoiceLineDuration = speechData ? speechData.UseVoiceLineDuration : useVoiceLineDuration;
            float _customDuration = speechData ? speechData.CustomDuration : duration;
            float _volume = speechData ? speechData.Volume : volume;

            float _duration = _useVoiceLineDuration ? _voiceline.length : _customDuration;

            if(isOverHead)
                Conversation.SpeakOverHead(speakerTransform, _speech, _duration, _voiceline, _volume);
            else
                Conversation.SpeakOnScreen(_identity, _speech, _duration, _voiceline, _volume);

            gameObject.SetActive(false);
        }
    }
}