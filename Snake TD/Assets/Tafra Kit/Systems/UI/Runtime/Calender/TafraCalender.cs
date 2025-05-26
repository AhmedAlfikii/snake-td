using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TafraKit.UI
{
    public class TafraCalender : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI monthNameTXT;
        [SerializeField] private List<TafraCalnderSlot> slots;
        [Tooltip("6 slots that should be present in the begining of the grid layout group, a number of them will be enabled in order to push day 1 slot further in the week days list.")]
        [SerializeField] private List<GameObject> initialEmptyslots;

        [Header("Properties")]
        [SerializeField] private bool autoDisplayCurrentMonthOnAwake = true;
        [SerializeField] private DayOfWeek calenderDayOne = DayOfWeek.Monday;

        private int curMonthDaysCount;

        private void Awake()
        {
            if (autoDisplayCurrentMonthOnAwake)
            {
                DateTime today = TafraDateTime.Today;
                DisplayMonth(today.Year, today.Month);
            }
        }

        public void DisplayMonth(int year, int month)
        {
            DateTime today = TafraDateTime.Today;
            DateTime displayedMonthFirstDay = new DateTime(year, month, 1);

            curMonthDaysCount = DateTime.DaysInMonth(displayedMonthFirstDay.Year, displayedMonthFirstDay.Month);

            monthNameTXT.text = displayedMonthFirstDay.ToString("MMMM");

            for (int i = 0; i < slots.Count; i++)
            {
                TafraCalnderSlot slot = slots[i];

                if (i < curMonthDaysCount)
                {
                    slot.SetDate(today.Year, today.Month, i + 1);
                    slot.gameObject.SetActive(true);
                }
                else
                    slot.gameObject.SetActive(false);
            }

            int dayOneDayOfWeek = (int)displayedMonthFirstDay.DayOfWeek;

            int requiredEmptySlots;
            int calenderDayOneInt = (int)calenderDayOne;

            if (dayOneDayOfWeek >= calenderDayOneInt)
                requiredEmptySlots = dayOneDayOfWeek - calenderDayOneInt;
            else
                requiredEmptySlots = (6 - calenderDayOneInt) + dayOneDayOfWeek + 1;

            for (int i = 0; i < initialEmptyslots.Count; i++)
            {
                initialEmptyslots[i].SetActive(i < requiredEmptySlots);
            }
        }
    }
}