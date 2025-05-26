using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class MusicTrackPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip track;
        [Range(0f, 1f)]
        [SerializeField] private float volume = 1;
        [SerializeField] private float fadeInDuration = 0;
        [SerializeField] private float previousTrackFadeOutDuration = 0;
        [SerializeField] private bool waitPreviousTrackFade = true;
        [SerializeField] private bool loop = true;

        private int trackID;

        private void OnEnable()
        {
            if(MusicPlayer.GetPlayingMainTrack() == track)
                return;

            trackID = MusicPlayer.Play(track, volume, fadeInDuration, previousTrackFadeOutDuration, waitPreviousTrackFade, loop);    
        }
    }
}