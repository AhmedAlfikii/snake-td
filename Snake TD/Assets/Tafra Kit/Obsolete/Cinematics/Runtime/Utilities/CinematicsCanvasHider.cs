using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TafraKit;
using TafraKit.ZTweeners;

namespace TafraKit
{
    public class CinematicsCanvasHider : MonoBehaviour
    {
        [SerializeField] private float hideDuration = 0.5f;
        [SerializeField] private bool disableGraphicsRaycaster = true;

        private CanvasGroup canvasGroup;
        private GraphicRaycaster canvasRayCaster;
        private ZTweenFloat alphaTween;

        private void Awake()
        {
            canvasRayCaster = GetComponent<GraphicRaycaster>();
            canvasGroup = GetComponent<CanvasGroup>();

            alphaTween = new ZTweenFloat(() =>
            {
                canvasGroup.alpha = alphaTween.CurValue;
            });
        }
        private void OnEnable()
        {
            Cinematics.OnCinematicStart.AddListener(OnCinematicStart);
            Cinematics.OnCinematicEnd.AddListener(OnCinematicEnd);
        }
        private void OnDisable()
        {
            Cinematics.OnCinematicStart.RemoveListener(OnCinematicStart);
            Cinematics.OnCinematicEnd.RemoveListener(OnCinematicEnd);
        }

        private void OnCinematicStart(bool instant)
        {
            canvasGroup.alpha.ZTweenChange(alphaTween, 0, instant ? 0 : hideDuration);

            if(disableGraphicsRaycaster && canvasRayCaster)
                canvasRayCaster.enabled = false;
        }
        private void OnCinematicEnd(bool instant)
        {
            canvasGroup.alpha.ZTweenChange(alphaTween, 1, instant? 0 : hideDuration);

            if(disableGraphicsRaycaster && canvasRayCaster)
                canvasRayCaster.enabled = true;
        }
    }
}