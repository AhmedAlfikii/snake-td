using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using ZUI;

namespace TafraKit.UI
{
    public class ToggleWithMotion : MonoBehaviour
    {
        #region Private Serialized Fields
        [SerializeField] private bool prewarm = true;
        [SerializeField] private bool defaultValue = true;
        [SerializeField] private RectToRect checkMarkRTR;
        [SerializeField] private List<UIElement> onUIE;
        [SerializeField] private List<UIElement> offUIE;

        [Header("SFX")]
        [SerializeField] private SFXClip turnOnAudio;
        [SerializeField] private SFXClip turnOffAudio;
        #endregion

        #region Public Events
        public BoolUnityEvent OnValueChanged;
        #endregion

        #region Private Fields
        private bool isOn;
        #endregion

        #region MonoBehaviour Messages
        private void Start()
        {
            if (prewarm)
                SetValue(defaultValue, true);
        }
        #endregion

        #region Public Functions
        public void SetValue(bool on, bool immediate = false)
        {
            if (immediate)
            {
                checkMarkRTR.GoToRect(on ? 1 : 0, 0);
                for (int i = 0; i < onUIE.Count; i++)
                {
                    onUIE[i].ChangeVisibilityImmediate(on);
                }
                for (int i = 0; i < offUIE.Count; i++)
                {
                    offUIE[i].ChangeVisibilityImmediate(!on);
                }
            }
            else
            {
                checkMarkRTR.GoToRect(on ? 1 : 0);
                for (int i = 0; i < onUIE.Count; i++)
                {
                    onUIE[i].ChangeVisibility(on);
                }
                for (int i = 0; i < offUIE.Count; i++)
                {
                    offUIE[i].ChangeVisibility(!on);
                }

                if (on && turnOnAudio != null)
                    SFXPlayer.Play(turnOnAudio);
                else if (!on && turnOffAudio != null)
                    SFXPlayer.Play(turnOffAudio);
            }

            isOn = on;

            OnValueChanged?.Invoke(isOn);
        }
        public void Switch()
        {
            SetValue(!isOn);
        }

        public bool IsOn()
        {
            return isOn;
        }
        #endregion
    }
}