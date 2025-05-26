using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZUI;

namespace TafraKit
{
    public class ZUIScreen : MonoBehaviour
    {
        [SerializeField] private ZUIElementBase[] uiElements;
        [SerializeField] private GraphicRaycaster graphicRaycaster;

        public void Show(bool instant = false)
        {
            for (int i = 0; i < uiElements.Length; i++)
            {
                if (!instant)
                    uiElements[i].ChangeVisibility(true);
                else
                    uiElements[i].ChangeVisibilityImmediate(true);
            }

            if(graphicRaycaster)
                graphicRaycaster.enabled = true;
        }
        public void Hide(bool instant = false)
        {
            for (int i = 0; i < uiElements.Length; i++)
            {
                if (!instant)
                    uiElements[i].ChangeVisibility(false);
                else
                    uiElements[i].ChangeVisibilityImmediate(false);
            }

            if(graphicRaycaster)
                graphicRaycaster.enabled = false;
        }
    }
}