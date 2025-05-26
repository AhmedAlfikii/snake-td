using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ZCasualGameKit
{
    public class ConsumableBar : MonoBehaviour
    {
        [Header("Bar Elements")]
        [Tooltip("The text object that displays the value of the consumable.")]
        [SerializeField] private TextMeshProUGUI amountTXT;

        [Tooltip("The consumable icon displayed on the bar.")]
        [SerializeField] private RectTransform iconRT;

        private float curAmount;

        public float Amount {
            get {
                return curAmount;
            }
            set
            {
                curAmount = value;

                amountTXT.text = value.ToString();
            }
        }

        public RectTransform IconRectTransform()
        {
            return iconRT;
        }

        public TextMeshProUGUI AmountTXT()
        {
            return amountTXT;
        }
    }
}