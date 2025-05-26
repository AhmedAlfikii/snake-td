using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit;
using TafraKit.ZTweeners;
using TMPro;
using TafraKit.UI;

namespace TafraKit.Demos
{
    public class ZTweenDemo : MonoBehaviour
    {
        #region Private Serialized Fields
        [SerializeField] private Transform target;
        [SerializeField] private TextMeshProUGUI floatTXT;
        [SerializeField] private RectToRect rectToRect;
        [SerializeField] private float floatTarget = 1f;
        [SerializeField] private Vector3 targetScale = new Vector3(1.2f, 1.2f, 1.2f);
        [SerializeField] private Vector3 targetEuler;
        [SerializeField] private float duration = 1f;
        [SerializeField] private EasingType easingType;
        #endregion

        #region Private Fields
        private bool goUp = true;
        private ZTweenVector3 scaleTween = new ZTweenVector3();
        private ZTweenQuaternion rotationTween = new ZTweenQuaternion();
        private ZTweenFloat floatTween;

        private Vector3 normalScale;
        private Vector3 normalEuler;
        private float normalFloat;
        #endregion

        #region MonoBehaviour Messages
        private void Awake()
        {
            floatTween = new ZTweenFloat(() => { floatTarget = floatTween.CurValue; });

            normalScale = target.localScale;
            normalEuler = target.eulerAngles;
            normalFloat = floatTarget;
        }
        private void OnDisable()
        {
            if (floatTween != null)
                floatTween.Stop();
            if (scaleTween != null)
                scaleTween.Stop();
            if (floatTween != null)
                floatTween.Stop();
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                target.ZTweenScale(scaleTween, goUp ? targetScale : normalScale, duration, this).SetEasingType(easingType);
                target.ZTweenRotate(rotationTween, goUp ? Quaternion.Euler(targetEuler) : Quaternion.Euler(normalEuler), duration, this).SetEasingType(easingType);
                floatTarget.ZTweenChange(floatTween, goUp ? 2 : normalFloat, duration, this).SetEasingType(easingType).SetOnUpdated(() =>
                {
                    floatTXT.text = floatTween.CurValue.ToString();
                });

                rectToRect.GoToRect(goUp ? 1 : 0);

                goUp = !goUp;
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                scaleTween.Stop();
                rotationTween.Stop();
                floatTween.Stop();
            }
        }
        #endregion

        #region Private Functions
        #endregion

        #region Public Functions
        #endregion
    }
}