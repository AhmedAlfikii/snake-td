using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TafraKit.UI
{
    public class ScrollDynamicMask : RectMask2D
    {
        [Header("Dynamic Fading")]
        [SerializeField] private Vector4 fadedPadding;
        [SerializeField] private Vector4 normalPadding = new Vector4(-140, -140, -140, -140);
        [SerializeField] private float fadeDuration = 1f;

        private ScrollRect scrollRect;
        private bool horizontal;
        private bool vertical;
        private bool xInitialized;
        private bool yInitialized;
        private bool zInitialized;
        private bool wInitialized;
        private bool xFaded;
        private bool yFaded;
        private bool zFaded;
        private bool wFaded;
        private IEnumerator xPaddingEnum;
        private IEnumerator yPaddingEnum;
        private IEnumerator zPaddingEnum;
        private IEnumerator wPaddingEnum;

        protected override void Awake()
        {
            base.Awake();

            scrollRect = GetComponent<ScrollRect>();

            if(scrollRect != null)
            {
                horizontal = scrollRect.horizontal;
                vertical = scrollRect.vertical;
                scrollRect.onValueChanged.AddListener(OnScrollValueChange);
            }
            else
                TafraDebugger.Log("Scroll Dynamic Mask", "There's no Scroll Rect component on the game obejct. Will not work.", TafraDebugger.LogType.Error, gameObject);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            OnScrollValueChange(scrollRect.normalizedPosition);
        }

        private void OnScrollValueChange(Vector2 value)
        {
            Vector4 currentPadding = padding;

            if(value.x >= 1)
            {
                if(zFaded || !zInitialized)
                {
                    ChangePadding(zPaddingEnum, 2, normalPadding.z);

                    zFaded = false;
                    zInitialized = true;
                }
            }
            else if (!zFaded || !zInitialized)
            {
                ChangePadding(zPaddingEnum, 2, fadedPadding.z);

                zFaded = true;
                zInitialized = true;
            }

            if(value.x <= 0)
            {
                if(xFaded || !xInitialized)
                {
                    ChangePadding(xPaddingEnum, 0, normalPadding.x);

                    xFaded = false;
                    xInitialized = true;
                }
            }
            else if(!xFaded || !xInitialized)
            {
                ChangePadding(xPaddingEnum, 0, fadedPadding.x);

                xFaded = true;
                xInitialized = true;
            }

            if(value.y >= 1)
            {
                if(wFaded || !wInitialized)
                {
                    ChangePadding(wPaddingEnum, 3, normalPadding.w);

                    wFaded = false;
                    wInitialized = true;
                }
            }
            else if(!wFaded || !wInitialized)
            {
                ChangePadding(wPaddingEnum, 3, fadedPadding.w);

                wFaded = true;
                wInitialized = true;
            }

            if(value.y <= 0)
            {
                if(yFaded || !yInitialized)
                {
                    ChangePadding(yPaddingEnum, 1, normalPadding.y);

                    yFaded = false;
                    yInitialized = true;
                }
            }
            else if(!yFaded || !yInitialized)
            {
                ChangePadding(yPaddingEnum, 1, fadedPadding.y);

                yFaded = true;
                yInitialized = true;
            }

            padding = currentPadding;
        }

        private void ChangePadding(IEnumerator paddingEnum, int paddingAxis, float paddingValue)
        {
            if(paddingEnum != null)
                StopCoroutine(paddingEnum);

            float startValue = 0;

            if(paddingAxis == 0)
                startValue = padding.x;
            else if(paddingAxis == 1)
                startValue = padding.y;
            else if(paddingAxis == 2)
                startValue = padding.z;
            else if(paddingAxis == 3)
                startValue = padding.w;

            paddingEnum = CompactCouroutines.CompactCoroutine(0, fadeDuration, true, (t) =>
            {
                Vector4 p = padding;
                t = MotionEquations.EaseOut(t);

                if (paddingAxis == 0)
                    p.x = Mathf.Lerp(startValue, paddingValue, t);
                else if (paddingAxis == 1)
                    p.y = Mathf.Lerp(startValue, paddingValue, t);
                else if (paddingAxis == 2)
                    p.z = Mathf.Lerp(startValue, paddingValue, t);
                else if (paddingAxis == 3)
                    p.w = Mathf.Lerp(startValue, paddingValue, t);

                padding = p;
            }, null, () =>
            {
                paddingEnum = null;
            });

            StartCoroutine(paddingEnum);
        }
    }
} 