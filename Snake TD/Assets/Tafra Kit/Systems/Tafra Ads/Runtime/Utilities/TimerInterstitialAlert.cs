using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TafraKit.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit.Ads
{
    public class TimerInterstitialAlert : InterstitialAlert
    {
        [SerializeField] private float time;
        [SerializeField] private bool freezeTime = true;

        private float currentTimer;

        public override async Task<BoolOperationResult> Show(CancellationToken ct)
        {
            try
            {
                if (freezeTime)
                    TimeScaler.SetTimeScale("timerInterstitialAlert", 0);

                currentTimer = time;
                
                OnShow?.Invoke();

                while (currentTimer > 0)
                {
                    ct.ThrowIfCancellationRequested();

                    currentTimer -= Time.unscaledDeltaTime;
                    await Task.Yield();
                }

                currentTimer = 0;

                return BoolOperationResult.Success;
            }
            catch (OperationCanceledException)
            {
                return BoolOperationResult.Canceled;
            }
            catch (Exception e)
            {
                TafraDebugger.Log("Tafra Ads - Timer Intersitial Alert", $"An error has occured while trying to display the intersitial timer alert. {e.Message}", TafraDebugger.LogType.Error);
                return BoolOperationResult.Fail;
            }
            finally
            {
                Hide();
            }
        }

        public override void Hide()
        {
            if (freezeTime)
                TimeScaler.RemoveTimeScaleControl("timerInterstitialAlert");

            OnHide?.Invoke();
        }

        public float GetCurrentTimer()
        {
            return currentTimer;
        }
    }
}