using System.Collections;
using UnityEngine;
#if TAFRA_CINEMACHINE
using Unity.Cinemachine;
#endif

namespace TafraKit.Cinemachine
{
    public class ShakeableCamera : MonoBehaviour
    {
        #if TAFRA_CINEMACHINE
        #region Private Serialized Fields
        [SerializeField] private NoiseSettings noiseSettings;
        [SerializeField] private float defaultAmplitudeGain = 1f;
        [SerializeField] private float defaultFrequencyGain = 1f;
        #endregion

        #region Private Fields
        private CinemachineCamera cam;
        private CinemachineBasicMultiChannelPerlin noiseComponent;
        #endregion

        public CinemachineBasicMultiChannelPerlin NoiseComponent => noiseComponent;

        private void Awake()
        {
            cam = GetComponent<CinemachineCamera>();
            
            noiseComponent = cam.gameObject.AddComponent<CinemachineBasicMultiChannelPerlin>();
            noiseComponent.NoiseProfile = noiseSettings;

            noiseComponent.AmplitudeGain = 0;
            noiseComponent.FrequencyGain = 0;
        }
        private void OnEnable()
        {
            ScreenShaker.RegisterCamera(this);
        }
        private void OnDisable()
        {
            ScreenShaker.UnregisterCamera(this);
        }
        #endif
    }
}