using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    [Serializable]
    public class ValueManipulator
    {
        public NumberOperation operation;
        public TafraAdvancedFloat value;
        [Tooltip("If true, the value added to the stat by this element will be part of the \"Extra Value\", which is the value caused by stat values or manipulators that are not considered base." +
            "Will not change the results of the stat calculation.")]
        public bool isExtra;

        public ValueManipulator(NumberOperation operation, TafraAdvancedFloat value, bool isExtra)
        {
            this.operation = operation;
            this.value = value;
            this.isExtra = isExtra;
        }

        public ValueManipulator()
        {

        }
    }
}