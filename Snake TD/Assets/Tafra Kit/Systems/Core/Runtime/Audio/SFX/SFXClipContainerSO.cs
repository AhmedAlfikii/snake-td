using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    [CreateAssetMenu(menuName = "Tafra Kit/Audio/SFX Clip Container", fileName = "SFXClipContainer")]
    public class SFXClipContainerSO : ScriptableObject
    {
        #region Private Serialized Fields
        [SerializeField] private AudioClip clip;
        #endregion

        #region Public Functions
        public AudioClip GetClip()
        {
            return clip;
        }
        #endregion
    }
}