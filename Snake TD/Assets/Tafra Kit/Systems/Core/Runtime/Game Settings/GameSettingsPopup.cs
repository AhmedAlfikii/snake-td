using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZUI;

namespace TafraKit.Internal
{
    public class GameSettingsPopup : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private GraphicRaycaster graphicRaycaster;
        [SerializeField] private UIElementsGroup uieg;
        [SerializeField] private GameSettingsSection[] sections;

        public Canvas Canvas => canvas;
        public GraphicRaycaster GraphicRaycaster => graphicRaycaster;
        public UIElementsGroup UIEG => uieg;

        private void Start()
        {
            UpdateSections();
        }

        public void UpdateSections()
        {
            for(int i = 0; i < sections.Length; i++)
            {
                GameSettingsSection section = sections[i];

                bool conditionsMet = section.AreConditionsSatisfied();

                if(section.gameObject.activeSelf != conditionsMet)
                    section.gameObject.SetActive(conditionsMet);
            }
        }
    }
}