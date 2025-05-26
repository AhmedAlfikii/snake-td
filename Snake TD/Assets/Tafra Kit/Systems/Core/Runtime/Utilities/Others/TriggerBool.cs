using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    /// <summary>
    /// A bool that automatically switches to false once a true value was returned.
    /// </summary>
    [Serializable]
    public class TriggerBool
    {
        [SerializeField] private bool trigger;

        public bool Trigger
        {
            set
            {
                trigger = value;
            }
            get
            {
                if (trigger == true)
                {
                    trigger = false;
                    return true;
                }
                else
                    return false;
            }
        }
    }
}