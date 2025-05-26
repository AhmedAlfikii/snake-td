using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit;
using TafraKit.Tasks;
using UnityEngine.Events;

namespace TafraKit.Ads
{
    public abstract class InterstitialAlert : MonoBehaviour 
    {
        public UnityEvent OnShow;
        public UnityEvent OnHide;

        public abstract Task<BoolOperationResult> Show(CancellationToken ct);
        public abstract void Hide();
    }
}