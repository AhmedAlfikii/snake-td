using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TafraKit;

namespace ZUtilities
{
    public class ZCamera : MonoBehaviour
    {
        #region Classes, Structs & Enums
        public enum CoveringType { FitInside = 0, Envelope = 1 }
        public enum Pivot { Center, Top, Bottom, Left, Right, TopLeft, TopRight, BottomLeft, BottomRight }
        public enum AdaptionType { Pos, FoV }
        #endregion

        #region Private Serialized Fields
        [SerializeField] private ZCameraView initialView;
        [SerializeField] private AdaptionType camAdaptionType;

        [Header("UI - Effects")]
        [SerializeField] private Image flasher;
        #endregion

        #region Private Fields
        private Camera myCam;
        private Vector3 normalPos;
        private IEnumerator coveringEnum;
        private IEnumerator shakingEnum;
        private IEnumerator flashingEnum;
        #endregion

        void Awake()
        {
            myCam = GetComponent<Camera>();

            if (initialView)
                CoverView(initialView, 0, 0);

            normalPos = transform.position;

            GlobalCamMan.Initialize();
            GlobalCamMan.OnChangeView.AddListener(CoverView);
            GlobalCamMan.OnShake.AddListener(Shake);
            GlobalCamMan.OnFlash.AddListener(Flash);
        }

        #region Public Functions
        public void CoverView(ZCameraView view, float delay, float duration, Action onReachedView = null)
        {
            float camAspect = myCam.aspect;

            Transform viewBounds = view.transform;

            float boundsAspect = viewBounds.localScale.x / viewBounds.localScale.y;

            float distance = Vector3.Distance(viewBounds.position, transform.position);// bounds.center.z - transform.position.z;
            float fov = myCam.fieldOfView;

            Vector3 pos = transform.position;
            Quaternion rot = Quaternion.LookRotation(-viewBounds.forward);

            if ((view.CoveringType() == CoveringType.FitInside && camAspect > boundsAspect) ||
                (view.CoveringType() == CoveringType.Envelope && camAspect < boundsAspect))
            {
                //Fit the width.
                float neededWidth = viewBounds.localScale.x;
                float neededHeight = (1 / camAspect) * neededWidth;

                if (camAdaptionType == AdaptionType.Pos)
                    distance = neededHeight * 0.5f / Mathf.Tan(myCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
                else if (camAdaptionType == AdaptionType.FoV)
                    fov = 2.0f * Mathf.Atan(neededHeight * 0.5f / distance) * Mathf.Rad2Deg;

                float heightDiff = Mathf.Abs(neededHeight - viewBounds.localScale.y);

                switch (view.Pivot())
                {
                    case Pivot.Bottom:
                    case Pivot.BottomLeft:
                    case Pivot.BottomRight:
                        if (view.CoveringType() == CoveringType.Envelope)
                        {
                            //pos = new Vector3(bounds.center.x, bounds.center.y + (heightDiff / 2f), bounds.center.z - distance);
                            pos = viewBounds.position + (viewBounds.transform.forward * distance) + (viewBounds.transform.up * (heightDiff / 2f));
                        }
                        else if (view.CoveringType() == CoveringType.FitInside)
                        {
                            Debug.LogError("FitInside is not developed yet!");
                        }
                        break;
                    case Pivot.Top:
                    case Pivot.TopLeft:
                    case Pivot.TopRight:
                        if (view.CoveringType() == CoveringType.Envelope)
                        {
                            //pos = new Vector3(bounds.center.x, bounds.center.y - (heightDiff / 2f), bounds.center.z - distance);
                            pos = viewBounds.position + (viewBounds.transform.forward * distance) - (viewBounds.transform.up * (heightDiff / 2f));
                        }
                        else if (view.CoveringType() == CoveringType.FitInside)
                        {
                            Debug.LogError("FitInside is not developed yet!");
                        }
                        break;
                    default:
                        //pos = new Vector3(bounds.center.x, bounds.center.y, bounds.center.z - distance);
                        pos = viewBounds.position + (viewBounds.transform.forward * distance);
                        break;

                }
            }
            else if ((view.CoveringType() == CoveringType.FitInside && camAspect <= boundsAspect) ||
                (view.CoveringType() == CoveringType.Envelope && camAspect >= boundsAspect))
            {
                //Fit the height.
                float neededHeight = viewBounds.localScale.y;
                float frustumWidth = myCam.aspect * neededHeight;

                if (camAdaptionType == AdaptionType.Pos)
                    distance = neededHeight * 0.5f / Mathf.Tan(myCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
                else if (camAdaptionType == AdaptionType.FoV)
                    fov = 2.0f * Mathf.Atan(neededHeight * 0.5f / distance) * Mathf.Rad2Deg;

                float widthDiff = Mathf.Abs(frustumWidth - viewBounds.localScale.x);

                switch (view.Pivot())
                {
                    case Pivot.Left:
                    case Pivot.TopLeft:
                    case Pivot.BottomLeft:
                        if (view.CoveringType() == CoveringType.Envelope)
                        {
                            //pos = new Vector3(bounds.center.x + (widthDiff / 2f), bounds.center.y, bounds.center.z - distance);
                            pos = viewBounds.position + (viewBounds.transform.forward * distance) + (viewBounds.transform.right * (widthDiff / 2f));
                        }
                        else if (view.CoveringType() == CoveringType.FitInside)
                        {
                            Debug.LogError("FitInside is not developed yet!");
                        }
                        break;
                    case Pivot.Right:
                    case Pivot.TopRight:
                    case Pivot.BottomRight:
                        if (view.CoveringType() == CoveringType.Envelope)
                        {
                            //pos = new Vector3(bounds.center.x - (widthDiff / 2f), bounds.center.y, bounds.center.z - distance);
                            pos = viewBounds.position + (viewBounds.transform.forward * distance) - (viewBounds.transform.right * (widthDiff / 2f));
                        }
                        else if (view.CoveringType() == CoveringType.FitInside)
                        {
                            Debug.LogError("FitInside is not developed yet!");
                        }
                        break;
                    default:
                        //pos = new Vector3(bounds.center.x, bounds.center.y, bounds.center.z - distance);
                        pos = viewBounds.position + (viewBounds.transform.forward * distance);
                        break;
                }
            }

            if (delay > 0.00f || duration > 0.001f)
            {
                Vector3 startPos = transform.position;
                Quaternion startRot = transform.rotation;
                float startFOV = myCam.fieldOfView;

                if (coveringEnum != null)
                    StopCoroutine(coveringEnum);

                coveringEnum = CompactCouroutines.CompactCoroutine(delay, duration, false, (t) =>
                {
                    t = MotionEquations.EaseInOut(t, 2);

                    transform.position = Vector3.LerpUnclamped(startPos, pos, t);
                    transform.rotation = Quaternion.LerpUnclamped(startRot, rot, t);
                    myCam.fieldOfView = Mathf.LerpUnclamped(startFOV, fov, t);
                }, null, () =>
                {
                    transform.position = pos;
                    transform.rotation = rot;
                    myCam.fieldOfView = fov;

                    onReachedView?.Invoke();
                });

                StartCoroutine(coveringEnum);
            }
            else
            {
                transform.position = pos;
                transform.rotation = rot;
                myCam.fieldOfView = fov;

                onReachedView?.Invoke();
            }
        }

        public void Shake(float power, float duration)
        {
            if (shakingEnum != null)
                StopCoroutine(shakingEnum);

            shakingEnum = Shaking(power, duration);
            StartCoroutine(shakingEnum);
        }
        IEnumerator Shaking(float power, float duration)
        {
            float startTime = Time.time;

            while (Time.time < startTime + duration)
            {
                float t = (Time.time - startTime) / duration;

                transform.position = normalPos + new Vector3(UnityEngine.Random.Range(-power, power), UnityEngine.Random.Range(-power, power), UnityEngine.Random.Range(-power, power));

                yield return null;
            }
            transform.position = normalPos;
        }

        public void Flash(GlobalCamMan.FlashData data)
        {
            if (flashingEnum != null)
                StopCoroutine(flashingEnum);

            flashingEnum = Flashing(data);
            StartCoroutine(flashingEnum);
        }
        IEnumerator Flashing(GlobalCamMan.FlashData data)
        {
            data.Color.a = 0;
            flasher.color = data.Color;
            flasher.gameObject.SetActive(true);

            float startTime = Time.time;

            if (data.ShowDuration > 0)
            {
                //Show the fader.
                while (Time.time < startTime + data.ShowDuration)
                {
                    float t = (Time.time - startTime) / data.ShowDuration;

                    data.Color.a = Mathf.Lerp(0, data.Power, t);
                    flasher.color = data.Color;

                    yield return null;
                }
            }

            data.Color.a = data.Power;
            flasher.color = data.Color;

            if (data.VisibleDuration > 0)
            {
                //Keep it on display.
                yield return Yielders.GetWaitForSeconds(data.VisibleDuration);
            }

            if (data.HideDuration > 0)
            {
                startTime = Time.time;

                //Hide the fader.
                while (Time.time < startTime + data.HideDuration)
                {
                    float t = (Time.time - startTime) / data.HideDuration;
                    t = MotionEquations.EaseIn(t, 2);

                    data.Color.a = Mathf.Lerp(data.Power, 0, t);
                    flasher.color = data.Color;

                    yield return null;
                }
            }

            data.Color.a = 0;
            flasher.color = data.Color;

            flasher.gameObject.SetActive(false);
        }
        #endregion

        #region Editor Functions
        public void CoverViewEditor(ZCameraView view)
        {
            Camera cam = GetComponent<Camera>();
            float camAspect = cam.aspect;

            Transform viewBounds = view.transform;

            float boundsAspect = viewBounds.localScale.x / viewBounds.localScale.y;

            float distance = Vector3.Distance(viewBounds.position, transform.position);// bounds.center.z - transform.position.z;
            float fov = cam.fieldOfView;

            Vector3 pos = transform.position;
            Quaternion rot = Quaternion.LookRotation(-viewBounds.forward);

            if ((view.CoveringType() == CoveringType.FitInside && camAspect > boundsAspect) ||
                (view.CoveringType() == CoveringType.Envelope && camAspect < boundsAspect))
            {
                //Fit the width.
                float neededWidth = viewBounds.localScale.x;
                float neededHeight = (1 / camAspect) * neededWidth;

                if (camAdaptionType == AdaptionType.Pos)
                    distance = neededHeight * 0.5f / Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
                else if (camAdaptionType == AdaptionType.FoV)
                    fov = 2.0f * Mathf.Atan(neededHeight * 0.5f / distance) * Mathf.Rad2Deg;

                float heightDiff = Mathf.Abs(neededHeight - viewBounds.localScale.y);

                switch (view.Pivot())
                {
                    case Pivot.Bottom:
                    case Pivot.BottomLeft:
                    case Pivot.BottomRight:
                        if (view.CoveringType() == CoveringType.Envelope)
                        {
                            //pos = new Vector3(bounds.center.x, bounds.center.y + (heightDiff / 2f), bounds.center.z - distance);
                            pos = viewBounds.position + (viewBounds.transform.forward * distance) + (viewBounds.transform.up * (heightDiff / 2f));
                        }
                        else if (view.CoveringType() == CoveringType.FitInside)
                        {
                            Debug.LogError("FitInside is not developed yet!");
                        }
                        break;
                    case Pivot.Top:
                    case Pivot.TopLeft:
                    case Pivot.TopRight:
                        if (view.CoveringType() == CoveringType.Envelope)
                        {
                            //pos = new Vector3(bounds.center.x, bounds.center.y - (heightDiff / 2f), bounds.center.z - distance);
                            pos = viewBounds.position + (viewBounds.transform.forward * distance) - (viewBounds.transform.up * (heightDiff / 2f));
                        }
                        else if (view.CoveringType() == CoveringType.FitInside)
                        {
                            Debug.LogError("FitInside is not developed yet!");
                        }
                        break;
                    default:
                        //pos = new Vector3(bounds.center.x, bounds.center.y, bounds.center.z - distance);
                        pos = viewBounds.position + (viewBounds.transform.forward * distance);
                        break;

                }
            }
            else if ((view.CoveringType() == CoveringType.FitInside && camAspect <= boundsAspect) ||
                (view.CoveringType() == CoveringType.Envelope && camAspect >= boundsAspect))
            {
                //Fit the height.
                float neededHeight = viewBounds.localScale.y;
                float frustumWidth = cam.aspect * neededHeight;

                if (camAdaptionType == AdaptionType.Pos)
                    distance = neededHeight * 0.5f / Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
                else if (camAdaptionType == AdaptionType.FoV)
                    fov = 2.0f * Mathf.Atan(neededHeight * 0.5f / distance) * Mathf.Rad2Deg;

                float widthDiff = Mathf.Abs(frustumWidth - viewBounds.localScale.x);

                switch (view.Pivot())
                {
                    case Pivot.Left:
                    case Pivot.TopLeft:
                    case Pivot.BottomLeft:
                        if (view.CoveringType() == CoveringType.Envelope)
                        {
                            //pos = new Vector3(bounds.center.x + (widthDiff / 2f), bounds.center.y, bounds.center.z - distance);
                            pos = viewBounds.position + (viewBounds.transform.forward * distance) + (viewBounds.transform.right * (widthDiff / 2f));
                        }
                        else if (view.CoveringType() == CoveringType.FitInside)
                        {
                            Debug.LogError("FitInside is not developed yet!");
                        }
                        break;
                    case Pivot.Right:
                    case Pivot.TopRight:
                    case Pivot.BottomRight:
                        if (view.CoveringType() == CoveringType.Envelope)
                        {
                            //pos = new Vector3(bounds.center.x - (widthDiff / 2f), bounds.center.y, bounds.center.z - distance);
                            pos = viewBounds.position + (viewBounds.transform.forward * distance) - (viewBounds.transform.right * (widthDiff / 2f));
                        }
                        else if (view.CoveringType() == CoveringType.FitInside)
                        {
                            Debug.LogError("FitInside is not developed yet!");
                        }
                        break;
                    default:
                        //pos = new Vector3(bounds.center.x, bounds.center.y, bounds.center.z - distance);
                        pos = viewBounds.position + (viewBounds.transform.forward * distance);
                        break;
                }
            }

            transform.position = pos;
            transform.rotation = rot;
            cam.fieldOfView = fov;
        }
        #endregion
    }
}