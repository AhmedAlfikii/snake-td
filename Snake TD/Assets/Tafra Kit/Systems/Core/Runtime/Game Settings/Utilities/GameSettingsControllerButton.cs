using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TafraKit.UI;

namespace TafraKit.Internal
{
    public class GameSettingsControllerButton : MonoBehaviour
    {
        public enum ButtonAction
        { 
            Open,
            Close
        }

        [SerializeField] private ButtonAction action;

        private ZButton zbutton;
        private Button button;

        private void Awake()
        {
            zbutton = GetComponent<ZButton>();

            if (zbutton == null)
                button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            if (zbutton != null)
                zbutton.onClick.AddListener(OnClick);
            else if (button != null)
                button.onClick.AddListener(OnClick);
        }
        private void OnDisable()
        {
            if (zbutton != null)
                zbutton.onClick.RemoveListener(OnClick);
            else if (button != null)
                button.onClick.RemoveListener(OnClick);
        }

        public void OnClick()
        {
            if (action == ButtonAction.Open)
                GameSettings.OpenPopup();
            else if (action == ButtonAction.Close)
                GameSettings.ClosePopup();
        }
    }
}