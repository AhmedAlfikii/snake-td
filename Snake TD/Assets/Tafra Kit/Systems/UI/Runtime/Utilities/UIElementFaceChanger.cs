using System.Collections;
using System.Collections.Generic;
using TafraKit;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit.UI
{
    [ExecuteAlways]
    public class UIElementFaceChanger : MonoBehaviour
    {
        [SerializeField] private RectTransform myRT;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private bool isFrontFace = true;

        public UnityEvent OnShouldShow;
        public UnityEvent OnShouldHide;

        private bool curViewIsFront;

        void Start()
        {
            float yAngle = ZHelper.GetContainedAngle(myRT.eulerAngles.y);
            curViewIsFront = yAngle < 90 && yAngle > -90;
            OnViewChanged();
        }

        void Update()
        {
            if(!myRT)
                return;

            float yAngle = ZHelper.GetContainedAngle(myRT.eulerAngles.y);
            bool newViewIsFront =
                (yAngle <= 360 && yAngle >= 270) || (yAngle <= 90 && yAngle >= 0);

            if(newViewIsFront != curViewIsFront)
            {
                curViewIsFront = newViewIsFront;
                OnViewChanged();
            }
        }

        void OnViewChanged()
        {
            if(isFrontFace)
            {

                if(curViewIsFront)
                {
                    if(canvasGroup != null)
                        canvasGroup.alpha = 1;

                    OnShouldShow?.Invoke();
                }
                else
                {
                    if(canvasGroup != null)
                        canvasGroup.alpha = 0;

                    OnShouldHide?.Invoke();
                }
            }
            else
            {
                if(curViewIsFront)
                {
                    if(canvasGroup != null)
                        canvasGroup.alpha = 0;

                    OnShouldHide?.Invoke();
                }
                else
                {
                    if(canvasGroup != null)
                        canvasGroup.alpha = 1;

                    OnShouldShow?.Invoke();
                }
            }
        }
    }
}