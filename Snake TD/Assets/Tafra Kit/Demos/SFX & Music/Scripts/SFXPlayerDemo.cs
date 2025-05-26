using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TafraKit.Demos
{
    public class SFXPlayerDemo : MonoBehaviour
    {
        #region Private Serialized Fields
        [SerializeField] private SFXClip sfxClip;

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
            isMutedTXT.text = SFXPlayer.IsMuted ? "Muted" : "Not Muted";
            muteButtonTXT.text = SFXPlayer.IsMuted ? "Unmute" : "Mute";
        }
        #endregion

        #region Public Functions
        public void PlayAudio()
        {
            latestSFXId = SFXPlayer.Play(sfxClip, true);
        }
        public void StopAudio()
        {
            SFXPlayer.Stop(latestSFXId);
        }

        public void MuteSwitch()
        {
            SFXPlayer.IsMuted = !SFXPlayer.IsMuted;

            isMutedTXT.text = SFXPlayer.IsMuted ? "Muted" : "Not Muted";
            muteButtonTXT.text = SFXPlayer.IsMuted ? "Unmute" : "Mute";
        }
        #endregion
    }
}