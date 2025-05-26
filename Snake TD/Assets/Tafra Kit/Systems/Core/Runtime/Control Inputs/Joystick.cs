using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace TafraKit
{
    public class Joystick : MonoBehaviour
    {
        #region Serialized Private Fields
        [Header("Properties")]
        [Tooltip("If true, the joystick will not show if a click was made on a gameobject (UI or a 3D gameobject that has a collider and the camera has Physics Raycaster component).")]
        [SerializeField] private bool ignoreClicksOverObjects = true;
        [Tooltip("Should the joystick start listening for input automatically? if this is set to false, you'll need to call Activate() funciton.")]
        [SerializeField] private bool activateOnStart = true;
        [Tooltip("Should the joystick always make sure that the input position is within its radius by following it if it went outside?")]
        [SerializeField] private bool followInputPosition = false;

        [Header("Visual Settings")]
        [Range(0, 1)]
        [Tooltip("The size of the joystick in relation to the screen's width.")]
        [SerializeField] private float joystickSizeToScreenWidth = 0.25f;
        [Range(0, 1)]
        [Tooltip("The size of the joystick' stick in relation to the total joystick size.")]
        [SerializeField] private float stickSizeToJoystick = 0.5f;
        [Range(0, 1)]
        [Tooltip("How much of the stick will be outside of the joystick's body on max drag.")]
        [SerializeField] private float stickOutDistance = 0;

        [Header("Visual References")]
        [SerializeField] private Canvas myCanvas;
        [SerializeField] private RectTransform joystickRT;
        [SerializeField] private RectTransform stickRT;
        #endregion

        #region Non-serialized Private Fields
        private float joystickSize;
        private float stickSize;
        private float scaleFactor;
        private Vector3 joystickStartPos;
        private float stickMaxDistance;
        private bool beingControlled;
        #endregion

        #region Public Properties
        public bool IsActive { get; private set; }
        public bool BeingControlled
        {
            get
            {
                return beingControlled;
            }
            private set
            {
                if (beingControlled == value)
                    return;

                beingControlled = value;

                if (beingControlled)
                    OnPressed?.Invoke();
                else
                    OnReleased?.Invoke();
            }
        }
        #endregion

        #region Delegates & Events
        public delegate void ValueChange(Vector3 value);

        public event ValueChange OnValueChange;
        public UnityEvent OnPressed;
        public UnityEvent OnReleased;
        #endregion

        #region MonoBehaviour Messages
        void Start()
        {
            if (activateOnStart)
                IsActive = true;

            joystickRT.anchorMin = new Vector2(0.5f, 0.5f);
            joystickRT.anchorMax = new Vector2(0.5f, 0.5f);
            stickRT.anchorMin = new Vector2(0.5f, 0.5f);
            stickRT.anchorMax = new Vector2(0.5f, 0.5f);

            joystickRT.gameObject.SetActive(false);

            scaleFactor = myCanvas.scaleFactor;

            float joystickRawSize = joystickSizeToScreenWidth * Screen.width;
            float joystickScaledSize = joystickRawSize / scaleFactor;

            float stickRawSize = stickSizeToJoystick * joystickRawSize;
            float stickScaledSize = stickRawSize / scaleFactor;

            joystickSize = joystickScaledSize;

            joystickRT.sizeDelta = new Vector2(joystickSize, joystickSize);

            stickSize = stickScaledSize;

            stickRT.sizeDelta = new Vector2(stickSize, stickSize);

            stickMaxDistance = ((joystickRawSize - stickRawSize) / 2f) + stickRawSize * stickOutDistance;
        }

        void Update()
        {
            if (!IsActive) return;

            if (BeingControlled)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    HideJoystick();
                    OnValueChange?.Invoke(Vector3.zero);
                    return;
                }

                Vector3 pointerPos = Input.mousePosition;
                Vector3 rawDir = pointerPos - joystickStartPos;
                float rawDirLength = rawDir.magnitude;
                Vector3 dir = rawDir;

                if (rawDirLength > stickMaxDistance)
                {
                    float additionalLength = rawDirLength - stickMaxDistance;
                    float additionalLengthPercent = additionalLength / rawDirLength;
                    dir *= 1 - additionalLengthPercent;

                    if (followInputPosition)
                    {
                        joystickStartPos += rawDir * additionalLengthPercent;
                        joystickRT.position = joystickStartPos;
                    }
                }

                stickRT.position = joystickStartPos + dir;
                dir /= stickMaxDistance;

                OnValueChange?.Invoke(dir);
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && (!ignoreClicksOverObjects || !ZHelper.IsPointerOverGameObject()))
                    ShowJoystick(Input.mousePosition);
            }
        }
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                HideJoystick();
                OnValueChange?.Invoke(Vector3.zero);
            }
        }
        #endregion

        #region Private Functions
        void ShowJoystick(Vector3 pos)
        {
            BeingControlled = true;

            joystickStartPos = pos;

            joystickRT.position = joystickStartPos;
            stickRT.position = joystickStartPos;

            joystickRT.gameObject.SetActive(true);
        }

        void HideJoystick()
        {
            BeingControlled = false;

            joystickRT.gameObject.SetActive(false);
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Activates the joystick, making it ready to receive player input.
        /// </summary>
        public void Activate()
        {
            if (IsActive)
                return;

            IsActive = true;
        }

        /// <summary>
        /// Dectivates the joystick, hiding it if it's currently visible and makes it ignore player input.
        /// </summary>
        public void Deactivate()
        {
            if (!IsActive)
                return;

            if (BeingControlled)
            {
                HideJoystick();
                OnValueChange?.Invoke(Vector3.zero);
            }

            IsActive = false;
        }
        #endregion
    }
}