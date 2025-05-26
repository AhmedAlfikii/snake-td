using System.Collections;
using System.Collections.Generic;
using TafraKit;
using TafraKit.MotionFactory;
using UnityEngine;
using UnityEngine.UI;
using ZUI;

namespace TafraKit.UI
{
    public class UIAlert : MonoBehaviour
    {
        [Tooltip("Reference to an object that implements the IUIAlertContainer interface.")]
        [SerializeField] private Object uiAlertContainer;
        [SerializeField] private Image alertImage;
        [SerializeField] private VisibilityMotionController alertMotionController;

        private IUIAlertContainer alertContainer;

        private void Awake()
        {
            if(uiAlertContainer == null)
                return;

            SetAlertContainer(uiAlertContainer);
        }
        private void OnEnable()
        {
            if(uiAlertContainer == null)
                return;

            SetAlert(alertContainer.CurrentAlertState, true);

            alertContainer.OnAlertStateChange.AddListener(OnAlertChanged);
        }
        private void OnDisable()
        {
            if(uiAlertContainer == null)
                return;

            alertContainer.OnAlertStateChange.RemoveListener(OnAlertChanged);
        }

        private void OnAlertChanged(UIAlertState state)
        {
            SetAlert(state);
        }

        public void SetAlertContainer(UnityEngine.Object container)
        {
            if (gameObject.activeSelf && alertContainer != null)
                alertContainer.OnAlertStateChange.RemoveListener(OnAlertChanged);

            if(container is IUIAlertContainer ac)
                alertContainer = ac;
            else if(container is Component comp)
                alertContainer = comp.GetComponent<IUIAlertContainer>();
            else if(container is GameObject go)
                alertContainer = go.GetComponent<IUIAlertContainer>();
            else
                TafraDebugger.Log("UIAlert", "Unrecognized alert container. Make sure it implements the \"IUIAlertContainer\" interface.", TafraDebugger.LogType.Error);

            SetAlert(alertContainer.CurrentAlertState, true);

            if(gameObject.activeSelf)
                alertContainer.OnAlertStateChange.AddListener(OnAlertChanged);
        }

        void SetAlert(UIAlertState alert, bool instant = false)
        {
            if(alert == UIAlertState.None)
            {
                alertMotionController.Hide(instant);

                return;
            }

            alertImage.sprite = UIAlertsManager.GetStateIcon(alert);

            if(!alertMotionController.IsVisible)
                alertMotionController.Show(instant);
        }
    }
}