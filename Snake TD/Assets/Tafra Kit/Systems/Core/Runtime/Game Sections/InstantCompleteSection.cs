using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class InstantCompleteSection : GameSection
    {
        protected override void OnStarted()
        {
            base.OnStarted();

            Complete();
        }
    }
}