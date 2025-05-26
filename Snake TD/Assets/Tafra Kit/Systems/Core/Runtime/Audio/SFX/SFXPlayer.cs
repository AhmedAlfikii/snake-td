using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit
{
    public static class SFXPlayer
    {
        private static SFXPlayerSettings settings;
        private static bool initialized;
        private static bool isEnabled;

        private static TafraAudioSourcePool audioSourcePool = new TafraAudioSourcePool();
        /// <summary>
        /// Contains the current playing SFXs. Key: ID of the sound effect. Value: the audio source playing it.
        /// </summary>
        private static Dictionary<int, TafraAudioSource> playingClips = new Dictionary<int, TafraAudioSource>();
        private static int lastID = -1;
        private static bool isMuted;
        private static UnityEvent<bool> muteStateChanged = new UnityEvent<bool>();

        public static bool IsEnabled => isEnabled;
        public static bool IsMuted
        {
            get 
            {
                if (!initialized)
                    TafraDebugger.Log("SFX Player", "SFX Player is not enabled, please enable it from Tafra Kit > Settings > SFX Player to be able to check its mute state.", TafraDebugger.LogType.Error);
                
                return isMuted;
            }
            set
            {
                if (!initialized)
                {
                    TafraDebugger.Log("SFX Player", "SFX Player is not enabled, please enable it from Tafra Kit > Settings > SFX Player to be able to adjust its mute state.", TafraDebugger.LogType.Error);
                    return;
                }

                isMuted = value;
                
                PlayerPrefs.SetInt("TAFRAKIT_SFXPLAYER_ISMUTED", isMuted ? 1 : 0);

                List<TafraAudioSource> allSources = audioSourcePool.GetAllPoolUnits();
                for (int i = 0; i < allSources.Count; i++)
                {
                    allSources[i].IsMuted = value;
                }

                muteStateChanged?.Invoke(isMuted);
            }
        }
        public static UnityEvent<bool> MuteStateChanged => muteStateChanged;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            settings = TafraSettings.GetSettings<SFXPlayerSettings>();

            if (settings)
            {
                if (!settings.Enabled)
                    return;

                isEnabled = true;

                initialized = true;

                GameObject sfxSourcesParent = new GameObject("SFXSources");
                
                MonoBehaviour.DontDestroyOnLoad(sfxSourcesParent);

                GameObject audioSourceGO = new GameObject("AudioSource", typeof(AudioSource), typeof(TafraAudioSource));
                TafraAudioSource audioSource = audioSourceGO.GetComponent<TafraAudioSource>();

                audioSourceGO.transform.SetParent(sfxSourcesParent.transform);
                audioSourcePool.Construct(new List<TafraAudioSource>(1) { audioSource }, sfxSourcesParent.transform, true, 0, false);

                audioSource.OnCompleted.AddListener(OnAudioSourceFinished);
                audioSource.OnStopped.AddListener(OnAudioSourceFinished);

                audioSourcePool.OnNewUnitCreated.AddListener((source) => {
                    source.IsMuted = IsMuted;
                    source.OnCompleted.AddListener(OnAudioSourceFinished);
                    source.OnStopped.AddListener(OnAudioSourceFinished);
                });

                audioSourcePool.Initialize();

                if (PlayerPrefs.HasKey("TAFRAKIT_SFXPLAYER_ISMUTED"))
                    IsMuted = PlayerPrefs.GetInt("TAFRAKIT_SFXPLAYER_ISMUTED") == 1;
                else
                    IsMuted = settings.MutedByDefault;
            }
        }

        /// <summary>
        /// Gets called when an audio source is completed or stopped.
        /// </summary>
        /// <param name="soundEffectId"></param>
        private static void OnAudioSourceFinished(int soundEffectId)
        {
            if (playingClips.ContainsKey(soundEffectId))
            {
                audioSourcePool.ReleaseUnit(playingClips[soundEffectId]);
                playingClips.Remove(soundEffectId);
            }
        }

        /// <summary>
        /// Plays a sound effect.
        /// </summary>
        /// <param name="soundEffect"></param>
        /// <param name="loop"></param>
        /// <returns>The ID of the sound effect played. Can be used to stop the sound effect or check its playing state.</returns>
        public static int Play(ISFX soundEffect, bool loop = false)
        {
            if (!initialized)
            {
                TafraDebugger.Log("SFX Player", "SFX Player is not enabled, please enable it from Tafra Kit > Settings > SFX Player to be able to play sound effects.", TafraDebugger.LogType.Error);
                return -1;
            }

            AudioClip clip = soundEffect.GetClip();

            if (clip == null)
            {
                //TafraDebugger.Log("SFX Player", "No clip found to play.", TafraDebugger.LogType.Verbose);
                return -1;
            }

            TafraAudioSource source = audioSourcePool.RequestUnit();

            lastID++;

            playingClips.Add(lastID, source);


            source.Play(clip, lastID, loop, soundEffect.GetVolume() * settings.VolumeScale, soundEffect.GetPitch(), soundEffect.GetFadeInDuration(), soundEffect.GetDelay());
            return lastID;
        }
        /// <summary>
        /// Plays a sound effect.
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="volume"></param>
        /// <param name="pitch"></param>
        /// <returns>The ID of the sound effect played. Can be used to stop the sound effect or check its playing state.</returns>
        public static int Play(AudioClip clip, float volume = 1, float pitch = 1, float fadeInDuration = 0, float delay = 0, bool loop = false)
        {
            if (!initialized)
            {
                TafraDebugger.Log("SFX Player", "SFX Player is not enabled, please enable it from Tafra Kit > Settings > SFX Player to be able to play sound effects.", TafraDebugger.LogType.Error);
                return -1;
            }

            TafraAudioSource source = audioSourcePool.RequestUnit();

            lastID++;

            playingClips.Add(lastID, source);

            source.Play(clip, lastID, loop, volume * settings.VolumeScale, pitch, fadeInDuration, delay);
            return lastID;
        }
        /// <summary>
        /// Plays a sound effect.
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="volumeRange"></param>
        /// <param name="pitchRange"></param>
        /// <returns>The ID of the sound effect played. Can be used to stop the sound effect or check its playing state.</returns>
        public static int Play(AudioClip clip, FloatRange volumeRange, FloatRange pitchRange, FloatRange fadeInDurationRange, FloatRange delayRange, bool loop = false)
        {
            if (!initialized)
            {
                TafraDebugger.Log("SFX Player", "SFX Player is not enabled, please enable it from Tafra Kit > Settings > SFX Player to be able to play sound effects.", TafraDebugger.LogType.Error);
                return -1;
            }

            TafraAudioSource source = audioSourcePool.RequestUnit();

            lastID++;

            playingClips.Add(lastID, source);

            source.Play(clip, lastID, loop, volumeRange.GetRandomValue() * settings.VolumeScale, pitchRange.GetRandomValue(), fadeInDurationRange.GetRandomValue(), delayRange.GetRandomValue());
            return lastID;
        }

        /// <summary>
        /// Stops a sound effect by ID.
        /// </summary>
        /// <param name="soundEffectId"></param>
        /// <returns>Whether the sound effect was playing or not.</returns>
        public static bool Stop(int soundEffectId, float fadeOutDuration = 0)
        {
            if (!initialized)
            {
                TafraDebugger.Log("SFX Player", "SFX Player is not enabled, please enable it from Tafra Kit > Settings > SFX Player to be able to stop sound effects.", TafraDebugger.LogType.Error);
                return false;
            }

            if (playingClips.ContainsKey(soundEffectId))
            {
                playingClips[soundEffectId].Stop(fadeOutDuration);

                return true;
            }
            else
            {
                TafraDebugger.Log("SFX Player", "The sound effect you're trying to stop isn't playing.", TafraDebugger.LogType.Verbose);

                return false;
            }
        }

        /// <summary>
        /// Stop all playing SFX.
        /// </summary>
        public static void StopAll(float fadeOutDuration)
        {
            if (!initialized)
            {
                TafraDebugger.Log("SFX Player", "SFX Player is not enabled, please enable it from Tafra Kit > Settings > SFX Player to be able to stop sound effects.", TafraDebugger.LogType.Error);
                return;
            }

            int totalPlayingClips = playingClips.Count;
            int firstPlayingID = lastID - (totalPlayingClips - 1);

            for (int trackID = firstPlayingID; trackID <= lastID; trackID++)
            {
                if (playingClips.ContainsKey(trackID))
                    playingClips[trackID].Stop(fadeOutDuration);
            }
        }

        /// <summary>
        /// Is the sound effect with the given id currently playing?
        /// </summary>
        /// <param name="soundEffectId"></param>
        /// <returns></returns>
        public static bool IsPlaying(int soundEffectId)
        {
            if (!initialized)
            {
                TafraDebugger.Log("SFX Player", "SFX Player is not enabled, please enable it from Tafra Kit > Settings > SFX Player to be able to check if sound effect is playing.", TafraDebugger.LogType.Error);
                return false;
            }

            if (playingClips.ContainsKey(soundEffectId))
                return true;
            else
                return false;
        }
    }
}