using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace TafraKit
{
    [Serializable]
    [DisplayName("Tafra Kit/Conversation/Speech Control Clip")]
    public class SpeechControlClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] private SpeechControlBehaviour template = new SpeechControlBehaviour();
        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<SpeechControlBehaviour>.Create(graph, template);
        }
    }
}