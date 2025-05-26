using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ZUI;

namespace ZCasualGameKit
{
    public class GuideBox : MonoBehaviour
    {
        public static GuideBox Instance;

        [SerializeField] private UIElement boxUIE;
        [SerializeField] private TextMeshProUGUI guideTXT;
        [SerializeField] private RectTransform[] positions;

        void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        public void ShowGuide(string guide, int positionIndex)
        {
            boxUIE.transform.position = positions[positionIndex].position;

            guideTXT.text = guide;

            boxUIE.ChangeVisibility(true);
        }

        public void HideGuide()
        {
            boxUIE.ChangeVisibility(false);
        }
    }
}
