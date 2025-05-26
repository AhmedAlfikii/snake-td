using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Healthies
{
    public class AboutToDieEventArgs : EventArgs
    {
        public bool PreventDeath { get; set; }

        public AboutToDieEventArgs()
        {
            PreventDeath = false;
        }
    }
}