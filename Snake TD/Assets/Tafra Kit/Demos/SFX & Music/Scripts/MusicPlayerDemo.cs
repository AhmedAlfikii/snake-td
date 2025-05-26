using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TafraKit.Demos
{
    public class MusicPlayerDemo : MonoBehaviour
    {
        #region Private Serialized Fields
        [SerializeField] private AudioClip track;
        [SerializeField] private AudioClip overlayTrack;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI isMutedTXT;
        [SerializeField] private TextMeshProUGUI muteButtonTXT;
        #endregion

        #region Private Fields
        private int latestSFXId;
        #endregion

        #region MonoBehaviour Messages
        private void Awake()
        {
            isMutedTXT.text = MusicPlayer.IsMuted ? "Muted" : "Not Muted";
            muteButtonTXT.text = MusicPlayer.IsMuted ? "Unmute" : "Mute";
        }
        #endregion

        #region Public Functions
        public void Play(float fadeDuraiton)
        {
            latestSFXId = MusicPlayer.Play(track, 0.25f, fadeDuraiton, fadeDuraiton, true);
        }
        public void PlayDefault(float fadeDuration)
        {
            MusicPlayer.PlayDefault(fadeDuration, true);
        }
        public void PlayDefaultImmediate()
        {
            MusicPlayer.PlayDefault(0, true, -1, 0);
        }
        public void PlayOverlay(float fadeDuraiton)
        {
            latestSFXId = MusicPlayer.PlayOverlay(overlayTrack, 1, fadeDuraiton, fadeDuraiton, true);
        }

        public void Stop(float fadeDuraiton)
        {
            MusicPlayer.Stop(fadeDuraiton);
        }
        public void StopOverlay(float fadeDuraiton)
        {
            MusicPlayer.StopOverlay(fadeDuraiton);
        }
        public void StopAll(float fadeDuration)
        {
            MusicPlayer.StopAll(fadeDuration);
        }

        public void MuteSwitch()
        {
            MusicPlayer.IsMuted = !MusicPlayer.IsMuted;

            isMutedTXT.text = MusicPlayer.IsMuted ? "Muted" : "Not Muted";
            muteButtonTXT.text = MusicPlayer.IsMuted ? "Unmute" : "Mute";
        }
        #endregion
    }
}