using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Healthies
{
    public class HitEventArgs : EventArgs
    {
        private HitInfo manipulatedHitInfo;

        public HitInfo OriginalHitInfo { get; }
        public HitInfo ManipulatedHitInfo => manipulatedHitInfo;

        public HitEventArgs(HitInfo hitInfo)
        {
            OriginalHitInfo = hitInfo;
            manipulatedHitInfo = hitInfo;
        }

        public void OverrideHitInfo(HitInfo newHitInfo)
        {
            manipulatedHitInfo = newHitInfo;
        }

        public void SetDamage(float newDamage)
        {
            manipulatedHitInfo.damage = newDamage;
        }
    }
}