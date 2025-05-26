using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace TafraKit
{
    public static class MusicPlayer
    {
        private static MusicPlayerSettings settings;
        private static bool initialized;
        private static bool isEnabled;

        private static TafraAudioSourcePool audioSourcePool = new TafraAudioSourcePool();
        private static TafraAudioSource mainTrackAudioSource;
        private static TafraAudioSource overlayTrackAudioSource;
        /// <summary>
        /// Contains the current playing music trackss. Key: ID of the track. Value: the audio source playing it.
        /// </summary>
        private static Dictionary<int, TafraAudioSource> playingClips = new Dictionary<int, TafraAudioSource>();
        private static int lastID = -1;
        private static bool isMuted;
        private static bool autoPlayDefaultTrack;
        public static UnityEvent<bool> OnToggled = new UnityEvent<bool>();

        public static bool IsEnabled => isEnabled;
        public static bool IsMuted
        {
            get
            {
                if (!initialized)
                    TafraDebugger.Log("Music Player", "Music Player is not enabled, please enable it from Tafra Kit > Settings > Music Player to be able to check its mute state.", TafraDebugger.LogType.Error);

                return isMuted;
            }
            set
            {
                if (!initialized)
                {
                    TafraDebugger.Log("Music Player", "Music Player is not enabled, please enable it from Tafra Kit > Settings > Music Player to be able to adjust its mute state.", TafraDebugger.LogType.Error);
                    return;
                }
                isMuted = value;

                PlayerPrefs.SetInt("TAFRAKIT_MUSICPLAYER_ISMUTED", isMuted ? 1 : 0);

                List<TafraAudioSource> allSources = audioSourcePool.GetAllPoolUnits();
                for (int i = 0; i < allSources.Count; i++)
                {
                    allSources[i].IsMuted = value;
                }

                OnToggled.Invoke(isMuted);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            settings = TafraSettings.GetSettings<MusicPlayerSettings>();

            if (settings)
            {
                if (!settings.Enabled) 
                    return;

                initialized = true;
                isEnabled = true;

                GameObject musicSourcesParent = new GameObject("MusicSources");

                MonoBehaviour.DontDestroyOnLoad(musicSourcesParent);

                GameObject audioSourceGO = new GameObject("AudioSource", typeof(AudioSource), typeof(TafraAudioSource));
                TafraAudioSource audioSource = audioSourceGO.GetComponent<TafraAudioSource>();

                audioSourceGO.transform.SetParent(musicSourcesParent.transform);
                audioSourcePool.Construct(new List<TafraAudioSource>(1) { audioSource }, musicSourcesParent.transform, true, 0, false);

                audioSource.OnCompleted.AddListener(OnAudioSourceFinished);
                audioSource.OnStopped.AddListener(OnAudioSourceFinished);
                audioSource.OnStartedStopping.AddListener(OnAudioSourceStartedStopping);

                audioSourcePool.OnNewUnitCreated.AddListener((source) => {
                    source.IsMuted = IsMuted;
                    source.OnCompleted.AddListener(OnAudioSourceFinished);
                    source.OnStopped.AddListener(OnAudioSourceFinished);
                    source.OnStartedStopping.AddListener(OnAudioSourceStartedStopping);
                });

                audioSourcePool.Initialize();

                if(PlayerPrefs.HasKey("TAFRAKIT_MUSICPLAYER_ISMUTED"))
                    IsMuted = PlayerPrefs.GetInt("TAFRAKIT_MUSICPLAYER_ISMUTED") == 1;
                else
                    IsMuted = settings.MutedByDefault;

                SceneManager.sceneLoaded += OnSceneLoaded;

                if (settings.DefaultTrack && settings.DefaultTrackCenterScene > -2)
                    autoPlayDefaultTrack = true;
            }
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            if (!autoPlayDefaultTrack) return;

            if(settings.DefaultTrack && IsSceneAMusicScene(scene.buildIndex))
            {
                Play(settings.DefaultTrack, settings.DefaultTrackStartVolume, settings.DefaultTrackFadeInDuration);

                autoPlayDefaultTrack = false;
            }
        }

        /// <summary>
        /// Gets called when an audio source is completed or stopped.
        /// </summary>
        /// <param name="trackId"></param>
        private static void OnAudioSourceFinished(int trackId)
        {
            if (playingClips.ContainsKey(trackId))
            {
                TafraAudioSource source = playingClips[trackId];
                audioSourcePool.ReleaseUnit(source);
                playingClips.Remove(trackId);

                if (source == mainTrackAudioSource)
                    mainTrackAudioSource = null;
                else if (source == overlayTrackAudioSource)
                {
                    overlayTrackAudioSource = null;

                    if (mainTrackAudioSource != null)
                        FadeInVolume(0.5f);
                }
            }
        }   
        private static void OnAudioSourceStartedStopping(int trackId, float fadeOutDuration)
        {
            if (playingClips.ContainsKey(trackId))
            {
                TafraAudioSource source = playingClips[trackId];

                if (source == mainTrackAudioSource)
                    mainTrackAudioSource = null;
                else if (source == overlayTrackAudioSource)
                {
                    overlayTrackAudioSource = null;

                    if (mainTrackAudioSource != null)
                        FadeInVolume(0.5f, fadeOutDuration);
                }
            }
        }

        /// <summary>
        /// Play a music track.
        /// </summary>
        /// <param name="track">The track to play.</param>
        /// <param name="volume">The target volume that this track should play at.</param>
        /// <param name="fadeInDuration">Duration in seconds this track should take to reach its target volume.</param>
        /// <param name="previousTrackFadeOutDuration">Duration in seconds to fade out the volume of the track that was playing.</param>
        /// <param name="waitPreviousTrackFade">Should the new track wait until the previous track finishes fading before playing?</param>
        /// <returns></returns>
        public static int Play(AudioClip track, float volume = 1, float fadeInDuration = 0, float previousTrackFadeOutDuration = 0, bool waitPreviousTrackFade = true, bool loop = true)
        {
            if (!initialized)
            {
                TafraDebugger.Log("Music Player", "Music Player is not enabled, please enable it from Tafra Kit > Settings > Music Player to be able to play sound effects.", TafraDebugger.LogType.Error);
                return -1;
            }

            float delay = 0;

            if (waitPreviousTrackFade && mainTrackAudioSource != null)
                delay = previousTrackFadeOutDuration;

            if (mainTrackAudioSource != null)
                Stop(previousTrackFadeOutDuration);

            lastID++;

            TafraAudioSource source = audioSourcePool.RequestUnit();

            source.Play(track, lastID, loop, volume * settings.VolumeScale, 1, fadeInDuration, delay);

            if (overlayTrackAudioSource != null)
                source.FadeOutVolume(0);

            playingClips.Add(lastID, source);

            mainTrackAudioSource = source;

            return lastID;
        }
        /// <summary>
        /// Play the default music track if it was set in the settings.
        /// </summary>
        /// <param name="previousTrackFadeOutDuration">Duration in seconds to fade out the volume of the track that was playing.</param>
        /// <param name="waitPreviousTrackFade">Should the new track wait until the previous track finishes fading before playing?</param>
        /// <param name="defaultVolumeOverride">Add a value that is 0 or higher to override the volume that was set in the Music Player's settings for the default track.</param>
        /// <param name="defaultFadeInDurationOverride">Add a value that is 0 or higher to override the fade in duration that was set in the Music Player's settings for the default track.</param>
        /// <param name="loop"></param>
        /// <returns></returns>
        public static int PlayDefault(float previousTrackFadeOutDuration = 0, bool waitPreviousTrackFade = true, float defaultVolumeOverride = -1, float defaultFadeInDurationOverride = -1, bool loop = true)
        {
            if (!initialized)
            {
                TafraDebugger.Log("Music Player", "Music Player is not enabled, please enable it from Tafra Kit > Settings > Music Player to be able to play sound effects.", TafraDebugger.LogType.Error);
                return -1;
            }
            if (settings.DefaultTrack == null)
            {
                TafraDebugger.Log("Music Player", "There's no default music track, you can assign one by going to Tafra Kit > Settings > Music Player.", TafraDebugger.LogType.Info);
                return -1;
            }

            return Play(settings.DefaultTrack, 
                defaultVolumeOverride >= 0? defaultVolumeOverride : settings.DefaultTrackStartVolume, 
                defaultFadeInDurationOverride >= 0? defaultFadeInDurationOverride : settings.DefaultTrackFadeInDuration, 
                previousTrackFadeOutDuration, waitPreviousTrackFade, loop);
        }
        /// <summary>
        /// Plays a track as an overlay on top of the main track. Doesn't stop the main track, only lowers its volume down to 0. Once it stops, the main track's volume is risen back to its default volume.
        /// </summary>
        /// <param name="track">The track to play temporarily.</param>
        /// <param name="volume"></param>
        /// <param name="fadeInDuration"></param>
        /// <param name="previousTrackFadeOutDuration"></param>
        /// <param name="waitPreviousTrackFade"></param>
        /// <returns></returns>
        public static int PlayOverlay(AudioClip track, float volume = 1, float fadeInDuration = 0, float previousTrackFadeOutDuration = 0, bool waitPreviousTrackFade = true, bool loop = true)
        {
            if (!initialized)
            {
                TafraDebugger.Log("Music Player", "Music Player is not enabled, please enable it from Tafra Kit > Settings > Music Player to be able to play sound effects.", TafraDebugger.LogType.Error);
                return -1;
            }

            float delay = 0;

            if (waitPreviousTrackFade && (overlayTrackAudioSource != null || mainTrackAudioSource != null))
                delay = previousTrackFadeOutDuration;

            if (overlayTrackAudioSource != null)
                StopOverlay(previousTrackFadeOutDuration);
            if (mainTrackAudioSource != null)
                FadeOutVolume(previousTrackFadeOutDuration);

            lastID++;

            TafraAudioSource source = audioSourcePool.RequestUnit();

            source.Play(track, lastID, loop, volume * settings.VolumeScale, 1, fadeInDuration, delay);

            playingClips.Add(lastID, source);

            overlayTrackAudioSource = source;

            return lastID;
        }

        /// <summary>
        /// Stop the currently playing main track if any, by fading its volume out.
        /// </summary>
        /// <param name="fadeOutDuration">The duration the track will take to reach 0 volume before stopping.</param>
        public static void Stop(float fadeOutDuration = 0)
        {
            if (!initialized)
            {
                TafraDebugger.Log("Music Player", "Music Player is not enabled, please enable it from Tafra Kit > Settings > Music Player to be able to play sound effects.", TafraDebugger.LogType.Error);
                return;
            }

            if (mainTrackAudioSource != null)
                mainTrackAudioSource.Stop(fadeOutDuration);
            else
                TafraDebugger.Log("Music Player", "There's no playing main track to stop.", TafraDebugger.LogType.Verbose);
        }
        /// <summary>
        /// Stop the currently playing overlay track if any, by fading its volume out.
        /// </summary>
        /// <param name="fadeOutDuration">The duration the track will take to reach 0 volume before stopping.</param>
        public static void StopOverlay(float fadeOutDuration = 0)
        {
            if (!initialized)
            {
                TafraDebugger.Log("Music Player", "Music Player is not enabled, please enable it from Tafra Kit > Settings > Music Player to be able to play sound effects.", TafraDebugger.LogType.Error);
                return;
            }

            if (overlayTrackAudioSource != null)
                overlayTrackAudioSource.Stop(fadeOutDuration);
            else
                TafraDebugger.Log("Music Player", "There's no overlay playing track to stop.", TafraDebugger.LogType.Verbose);
        }

        /// <summary>
        /// Stop the track with the given ID if it's the currently playing main track or overlay track by fading its volume out.
        /// </summary>
        /// <param name="trackId">The ID of the track to stop if it's playing.</param>
        /// <param name="fadeOutDuration">The duration the track will take to reach 0 volume before stopping.</param>
        /// <returns></returns>
        public static bool StopTrack(int trackId, float fadeOutDuration)
        {
            if (!initialized)
            {
                TafraDebugger.Log("Music Player", "Music Player is not enabled, please enable it from Tafra Kit > Settings > Music Player to be able to play sound effects.", TafraDebugger.LogType.Error);
                return false;
            }

            if (mainTrackAudioSource && mainTrackAudioSource.GetAudioID() == trackId)
            {
                Stop(fadeOutDuration);
                return true;
            }
            else if (overlayTrackAudioSource && overlayTrackAudioSource.GetAudioID() == trackId)
            {
                StopOverlay(fadeOutDuration);
                return true;
            }

            TafraDebugger.Log("Music Player", "The track you're trying to stop isn't playing.", TafraDebugger.LogType.Verbose);

            return false;
        }

        /// <summary>
        /// Stop all the music tracks playing, whether its the main track, overlay track or tracks that are fading out.
        /// </summary>
        public static void StopAll(float fadeOutDuration)
        {
            if (!initialized)
            {
                TafraDebugger.Log("Music Player", "Music Player is not enabled, please enable it from Tafra Kit > Settings > Music Player to be able to play sound effects.", TafraDebugger.LogType.Error);
                return;
            }

            int totalPlayingClips = playingClips.Count;
            int firstPlayingID = lastID - (totalPlayingClips - 1);

            for (int trackID = firstPlayingID; trackID <= lastID; trackID++)
                playingClips[trackID].Stop(fadeOutDuration);
        }        

        /// <summary>
        /// Fade out the volume of the main track without stopping it.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="delay"></param>
        public static void FadeOutVolume(float duration, float delay = 0)
        {
            if (!initialized)
            {
                TafraDebugger.Log("Music Player", "Music Player is not enabled, please enable it from Tafra Kit > Settings > Music Player to be able to play sound effects.", TafraDebugger.LogType.Error);
                return;
            }

            if (mainTrackAudioSource == null)
            {
                TafraDebugger.Log("Music Player", "There's no main track playing to fade out its volume.", TafraDebugger.LogType.Verbose);
                return;
            }

            mainTrackAudioSource.FadeOutVolume(duration, delay);
        }
        /// <summary>
        /// Fade out the volume of the overlay track without stopping it.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="delay"></param>
        public static void FadeOutVolumeOverlay(float duration, float delay = 0)
        {
            if (!initialized)
            {
                TafraDebugger.Log("Music Player", "Music Player is not enabled, please enable it from Tafra Kit > Settings > Music Player to be able to play sound effects.", TafraDebugger.LogType.Error);
                return;
            }

            if (overlayTrackAudioSource == null)
            {
                TafraDebugger.Log("Music Player", "There's no overlay track playing to fade out its volume.", TafraDebugger.LogType.Verbose);
                return;
            }

            overlayTrackAudioSource.FadeOutVolume(duration, delay);
        }
        /// <summary>
        /// Fade the volume of the main track back to its normal volume.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="delay"></param>
        public static void FadeInVolume(float duration, float delay = 0)
        {
            if (!initialized)
            {
                TafraDebugger.Log("Music Player", "Music Player is not enabled, please enable it from Tafra Kit > Settings > Music Player to be able to play sound effects.", TafraDebugger.LogType.Error);
                return;
            }

            if (mainTrackAudioSource == null)
            {
                TafraDebugger.Log("Music Player", "There's no main track playing to fade in its volume.", TafraDebugger.LogType.Verbose);
                return;
            }

            mainTrackAudioSource.FadeInVolume(duration, delay);
        }
        /// <summary>
        /// Fade the volume of the overlay track back to its normal volume.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="delay"></param>
        public static void FadeInVolumeOverlay(float duration, float delay = 0)
        {
            if (!initialized)
            {
                TafraDebugger.Log("Music Player", "Music Player is not enabled, please enable it from Tafra Kit > Settings > Music Player to be able to play music.", TafraDebugger.LogType.Error);
                return;
            }

            if (overlayTrackAudioSource == null)
            {
                TafraDebugger.Log("Music Player", "There's no overlay track playing to fade in its volume.", TafraDebugger.LogType.Verbose);
                return;
            }

            overlayTrackAudioSource.FadeInVolume(duration, delay);
        }
        /// <summary>
        /// Fade out the volume of the main track without stopping it.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="delay"></param>
        public static void FadeToVolume(float volume, float duration, float delay = 0)
        {
            if (!initialized)
            {
                TafraDebugger.Log("Music Player", "Music Player is not enabled, please enable it from Tafra Kit > Settings > Music Player to be able to play music.", TafraDebugger.LogType.Error);
                return;
            }

            if (mainTrackAudioSource == null)
            {
                TafraDebugger.Log("Music Player", "There's no main track playing to fade its volume.", TafraDebugger.LogType.Verbose);
                return;
            }

            mainTrackAudioSource.FadeVolume(volume, duration, delay);
        }

        public static AudioClip GetPlayingMainTrack()
        {
            if (mainTrackAudioSource == null)
                return null;

            return mainTrackAudioSource.GetPlayingClip();
        }

        /// <summary>
        /// Checks if the given track is playing in either the main layer or the overlay layer.
        /// </summary>
        /// <param name="trackId"></param>
        /// <returns></returns>
        public static bool IsPlaying(int trackId)
        {
            if (mainTrackAudioSource != null && mainTrackAudioSource.GetAudioID() == trackId)
                return true;
            if (overlayTrackAudioSource != null && overlayTrackAudioSource.GetAudioID() == trackId)
                return true;

            return false;
        }
        /// <summary>
        /// Checks if the given track is playing in the main layer.
        /// </summary>
        /// <param name="trackId"></param>
        /// <returns></returns>
        public static bool IsPlayingMain(int trackId)
        {
            if (mainTrackAudioSource != null && mainTrackAudioSource.GetAudioID() == trackId)
                return true;

            return false;
        }
        /// <summary>
        /// Checks if the given track is playing in the overlay layer.
        /// </summary>
        /// <param name="trackId"></param>
        /// <returns></returns>
        public static bool IsPlayingOverlay(int trackId)
        {
            if (overlayTrackAudioSource != null && overlayTrackAudioSource.GetAudioID() == trackId)
                return true;

            return false;
        }
        public static bool IsSceneAMusicScene(int sceneBuildIndex)
        {
            if (settings.DefaultTrackCenterScene > -2)
            {
                if (settings.DefaultTrackCenterScene == -1 /*Any SCENE*/

                    || (sceneBuildIndex == settings.DefaultTrackCenterScene &&
                        (settings.DefaultTackPlayPointAroundScene == MusicPlayerSettings.PlayPoint.At
                        || settings.DefaultTackPlayPointAroundScene == MusicPlayerSettings.PlayPoint.BeforeOrAt
                        || settings.DefaultTackPlayPointAroundScene == MusicPlayerSettings.PlayPoint.AtOrAfter))

                    || (sceneBuildIndex > settings.DefaultTrackCenterScene && (settings.DefaultTackPlayPointAroundScene == MusicPlayerSettings.PlayPoint.After
                        || settings.DefaultTackPlayPointAroundScene == MusicPlayerSettings.PlayPoint.AtOrAfter))

                    || (sceneBuildIndex < settings.DefaultTrackCenterScene && (settings.DefaultTackPlayPointAroundScene == MusicPlayerSettings.PlayPoint.Before
                        || settings.DefaultTackPlayPointAroundScene == MusicPlayerSettings.PlayPoint.BeforeOrAt)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}