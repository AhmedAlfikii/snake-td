using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TafraKit.MetaGame
{
    public class SpinningWheelSlot : MonoBehaviour
    {
        [SerializeField] private Image iconIMG;
        [SerializeField] private TextMeshProUGUI amountTXT;
        [SerializeField] private bool useCompactNumberString = true;


        public Image IconIMG => iconIMG;
        
        public void SetData(Sprite icon, int amount)
        {
            iconIMG.sprite = icon;
            amountTXT.text = useCompactNumberString ? amount.ToString("N0") : amount.ToString();
        }
    }
}