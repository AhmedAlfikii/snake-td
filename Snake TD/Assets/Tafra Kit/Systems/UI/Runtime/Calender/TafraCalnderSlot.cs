using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TafraKit.UI
{
    public class TafraCalnderSlot : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI dayTXT;

        protected int year;
        protected int month;
        protected int day;

        public virtual void SetDate(int year, int month, int day)
        {
            this.year = year;
            this.month = month;
            this.day = day;

            dayTXT.text = day.ToString();
        }
    }
}