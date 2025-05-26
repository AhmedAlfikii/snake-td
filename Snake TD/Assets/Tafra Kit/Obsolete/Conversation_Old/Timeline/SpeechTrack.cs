using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace TafraKit
{
    [TrackColor(241f/255f, 249f/255f, 99f/255f)]
    [TrackBindingType(typeof(SpeechBubble))]
    [TrackClipType(typeof(SpeechControlClip))]
    [DisplayName("Tafra Kit/Conversation/Speech Track")]
    public class SpeechTrack : TrackAsset
    {
        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            SpeechBubble speechBubble = director.GetGenericBinding(this) as SpeechBubble;

            if(speechBubble == null) 
                return;

            speechBubble.gameObject.SetActive(false);
        }
    }
}