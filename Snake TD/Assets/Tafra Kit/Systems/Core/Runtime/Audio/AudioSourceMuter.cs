using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class AudioSourceMuter : MonoBehaviour
    {
        private AudioSource myAS;

        private void Awake()
        {
            if(myAS == null)
                myAS = GetComponent<AudioSource>();
        }
        private void OnEnable()
        {
            myAS.mute = SFXPlayer.IsMuted;

            SFXPlayer.MuteStateChanged.AddListener(OnSFXPlayerMuteStateChanged);
        }
        private void OnDisable()
        {
            SFXPlayer.MuteStateChanged.RemoveListener(OnSFXPlayerMuteStateChanged);
        }

        private void OnSFXPlayerMuteStateChanged(bool isMuted)
        {
            myAS.mute = isMuted;
        }
    }
}