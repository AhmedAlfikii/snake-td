using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class MusicPlayerSettings : SettingsModule
    {
        public enum PlayPoint
        { 
            Before,
            BeforeOrAt,
            At,
            AtOrAfter,
            After
        }

        public bool Enabled = true;
        [Tooltip("The default music track to play when needed.")]
        public AudioClip DefaultTrack;
        [Tooltip("The scene in the build settings of which the default track will either start playing when loaded, or when a scene before it was loaded, or when a scene after it was loaded (not that the default track will not stop when the scene changes, you'll have to manually stop it if you need to).")]
        public int DefaultTrackCenterScene = -1;
        [Tooltip("Determine if you want the defaul track to start playing if any scene that preceeds the center scene was loaded, or if that particular scene was loaded, or if any scene that follows it was loaded.")]
        public PlayPoint DefaultTackPlayPointAroundScene = PlayPoint.At;
        [Tooltip("The volume of the default track.")]
        [Range(0, 1)]
        public float DefaultTrackStartVolume = 1;
        [Tooltip("The duration in seconds for the default track to reach full volume.")]
        public float DefaultTrackFadeInDuration = 0;
        [Tooltip("The default scale of the volume of all the SFXs (e.g. if this is set to 0.5, then any SFX that is set to play at 1 volume will actually play at 0.5, and if it's set to play at 0.5, it will play at 0.25, and so on).")]
        [Range(0, 1)]
        public float VolumeScale = 1;
        public bool MutedByDefault = false;

        public override int Priority => 5;

        public override string Name => "Audio/Music Player";

        public override string Description => "Play music tracks, fade between them and control volume and mute state.";
    }
}