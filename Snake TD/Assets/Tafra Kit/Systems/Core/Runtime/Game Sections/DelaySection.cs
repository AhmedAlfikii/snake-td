using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public class DelaySection : GameSection
    {
        [SerializeField] private float delay = 3f;

        protected override void OnStarted()
        {
            base.OnStarted();

            StartCoroutine("DelayComplete");
        }
        protected override void OnCompleted()
        {
            base.OnCompleted();

            //Just in case the section was stopped by an external object.
            StopCoroutine("DelayComplete");
        }
        IEnumerator DelayComplete()
        { 
            yield return Yielders.GetWaitForSeconds(delay);

            Complete();
        }
    }
}