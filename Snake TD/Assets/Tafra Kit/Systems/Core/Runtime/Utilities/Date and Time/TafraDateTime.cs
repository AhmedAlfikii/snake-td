using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TafraKit
{
    /// <summary>
    /// The purpose of this class is to have a single point that returns the current time. So that any calculations or fraud preventation could be done in this single place.
    /// </summary>
    public static class TafraDateTime
    {
        public static DateTime Now
        {
            get
            {
                return DateTime.Now;
            }
        }
        public static DateTime Today
        { 
            get 
            {
                return DateTime.Today; 
            }
        }
        /// <summary>
        /// The first time the game was opened.
        /// </summary>
        public static DateTime FirstOpen
        {
            get
            {
                return firstOpen;
            }
            private set
            {
                firstOpen = value;
                PlayerPrefs.SetString(playerPrefsFirstOpenKey, firstOpen.ToString());
            }
        }
        /// <summary>
        /// The number of days that have passed since the first time the game was opened.
        /// </summary>
        public static double DaysSinceFirstLogin
        {
            get
            {
                return (Now - FirstOpen).TotalDays;
            }
        }
        /// <summary>
        /// The number of full days that have passed since the first time the game was opened.
        /// </summary>
        public static int FullDaysSinceFirstLogin
        {
            get
            {
                return (int)((Now - FirstOpen).TotalDays);
            }
        }
        private const string playerPrefsFirstOpenKey = "TAFRA_DATETIME_FIRST_OPEN";

        private static DateTime firstOpen;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initialize()
        {
            if(!PlayerPrefs.HasKey(playerPrefsFirstOpenKey))
            {
                FirstOpen = Now;
            }
            else
                firstOpen = DateTime.Parse(PlayerPrefs.GetString(playerPrefsFirstOpenKey));
        }

        /// <summary>
        /// Convert seconds to timer string, example 310 seconds will be converted to "05:10" or "5m 10s".
        /// </summary>
        /// <param name="sb">The string builder that will be used to build the timer string.</param>
        /// <param name="seconds">The time in seconds that will be used to create the timer setring.</param>
        /// <param name="minUnit"></param>
        /// <param name="maxUnit"></param>
        /// <param name="isCountingDown">Will determine whether to round down or round up fractions.</param>
        /// <param name="lettersFormat">If true, the timer will be displayed as "5m 10s" instead of "05:10".</param>
        public static void GetTimeString(StringBuilder sb, double seconds, TimeUnit minUnit, TimeUnit maxUnit, bool isCountingDown, bool lettersFormat)
        {
            sb.Clear();

            double remainingSeconds = seconds;

            int maxUnitValue = (int)maxUnit;
            int minUnitValue = (int)minUnit;

            bool isFirst = true;
            for (int i = maxUnitValue; i >= minUnitValue; i--)
            {
                TimeUnit unit = (TimeUnit)i;

                bool isLast = i == minUnitValue;
                bool excluded = false;

                switch(unit)
                {
                    case TimeUnit.Millisecond:
                            remainingSeconds = ProcessUnit(sb, remainingSeconds, 0.001f, isCountingDown, lettersFormat, "ms", isFirst, isLast, out excluded);
                        break;
                    case TimeUnit.Second:
                            remainingSeconds = ProcessUnit(sb, remainingSeconds, 1, isCountingDown, lettersFormat, "s", isFirst, isLast, out excluded);
                        break;
                    case TimeUnit.Minute:
                            remainingSeconds = ProcessUnit(sb, remainingSeconds, 60, isCountingDown, lettersFormat, "m", isFirst, isLast, out excluded);
                        break;
                    case TimeUnit.Hour:
                            remainingSeconds = ProcessUnit(sb, remainingSeconds, 3600, isCountingDown, lettersFormat, "h", isFirst, isLast, out excluded);
                        break;
                    case TimeUnit.Day:
                            remainingSeconds = ProcessUnit(sb, remainingSeconds, 86400, isCountingDown, lettersFormat, "d", isFirst, isLast, out excluded);
                        break;
                    case TimeUnit.Week:
                            remainingSeconds = ProcessUnit(sb, remainingSeconds, 604800, isCountingDown, lettersFormat, "w", isFirst, isLast, out excluded);
                        break;
                    case TimeUnit.Month:
                            remainingSeconds = ProcessUnit(sb, remainingSeconds, 16934400d, isCountingDown, lettersFormat, "m", isFirst, isLast, out excluded);
                        break;
                    case TimeUnit.Year:
                            remainingSeconds = ProcessUnit(sb, remainingSeconds, 6181056000d, isCountingDown, lettersFormat, "y", isFirst, isLast, out excluded);
                        break;
                }

                if(isFirst && !excluded)
                    isFirst = false;
            }
        }
        private static double ProcessUnit(StringBuilder sb, double totalSeconds, double multiplier, bool isCountingDown, bool lettersFormat, string letter, bool isFirst, bool isLast, out bool excluded)
        {
            excluded = false;

            double fractionUnits = totalSeconds / multiplier;
            int wholeUnits;

            bool roundUp = isLast && isCountingDown;
            wholeUnits = roundUp ? Mathf.CeilToInt((float)fractionUnits) : Mathf.FloorToInt((float)fractionUnits);

            double remainder = (fractionUnits - wholeUnits) * multiplier;
            
            if (isCountingDown && Mathf.CeilToInt((float)remainder) >= multiplier)
            {
                remainder -= Mathf.RoundToInt((float)multiplier);
                wholeUnits++;
            }

            if(lettersFormat)
            {
                if(isFirst && wholeUnits <= 0)
                    excluded = true;
                else
                {
                    sb.Append(wholeUnits).Append(letter);

                    if(!isLast)
                        sb.Append(' ');
                }
            }
            else
            {
                sb.AppendFormat("{0:00}", wholeUnits);

                if(!isLast)
                    sb.Append(':');
            }
            
            return remainder;
        }
    }
}