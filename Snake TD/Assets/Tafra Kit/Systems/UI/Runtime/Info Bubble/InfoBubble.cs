using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TafraKit.MotionFactory;
using System.Text;

namespace TafraKit.Internal.UI
{
    public class InfoBubble : MonoBehaviour
    {
        [System.Serializable]
        public class RectBySide
        {
            public Side side;
            public RectTransform rt;
        }

        [SerializeField] private VisibilityMotionController motionController;
        [SerializeField] private TextMeshProUGUI titleTXT;
        [SerializeField] private TextMeshProUGUI infoTXT;
        [SerializeField] private RectTransform infoTextRT;
        [SerializeField] private RectTransform mainRT;
        [SerializeField] private List<RectBySide> arrows = new List<RectBySide>();
        [SerializeField] private RectTransform layoutGroupRT;

        public RectTransform MainRT => mainRT;

        public void SetInfo(string info, string title = null)
        {
            if(!string.IsNullOrEmpty(title))
            {
                titleTXT.gameObject.SetActive(true);
                titleTXT.text = title;
            }
            else
                titleTXT.gameObject.SetActive(false);

            infoTXT.text = info;
        }
        public void RefreshLayoutGroup()
        {
            //LayoutRebuilder.ForceRebuildLayoutImmediate(infoTextRT);
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroupRT);
        }
        public RectTransform GetArrow(Side pointingSide)
        {
            Side targetArrowSide = pointingSide;

            switch(pointingSide)
            {
                case Side.Left:
                    targetArrowSide = Side.Right;
                    break;
                case Side.Right:
                    targetArrowSide = Side.Left;
                    break;
                case Side.Top:
                    targetArrowSide = Side.Bottom;
                    break;
                case Side.Bottom:
                    targetArrowSide = Side.Top;
                    break;
            }

            RectTransform arrow = null;

            for(int i = 0; i < arrows.Count; i++)
            {
                var a = arrows[i];

                if(a.side == targetArrowSide)
                {
                    arrow = a.rt;
                    a.rt.gameObject.SetActive(true);
                }
                else
                    a.rt.gameObject.SetActive(false);
            }

            return arrow;
        }

        public void Show(bool instant)
        {
            motionController.Show(instant);
        }
        public void Hide(bool instant)
        {
            motionController.Hide(instant);
        }
    }
}