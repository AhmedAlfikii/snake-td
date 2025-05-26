using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    [Serializable]
    public class TimeSpanSimple
    {
        public int Seconds;
        public int Minutes;
        public int Hours;
        public int Days;

        public TimeSpan TimeSpan()
        {
            return new TimeSpan(Days, Hours, Minutes, Seconds, 0);
        }

        public TimeSpanSimple(int seconds,int minutes = 0,int hours = 0,int days = 0)
        {
            Seconds = seconds;
            Minutes = minutes;
            Hours = hours;
            Days = days;
        }
    }
}