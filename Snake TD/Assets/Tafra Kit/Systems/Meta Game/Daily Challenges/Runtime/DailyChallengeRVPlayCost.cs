using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using TafraKit.Tasks;
using TafraKit.Ads;

namespace TafraKit.MetaGame
{
    public class DailyChallengeRVPlayCost : DailyChallengePlayCost
    {
        public override async Task<BoolOperationResult> PayCost(int year, int month, int day, CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();

                Task<bool> showingRVTask = ShowRV(ct);

                await showingRVTask;

                ct.ThrowIfCancellationRequested();

                return showingRVTask.Result == true? BoolOperationResult.Success : BoolOperationResult.Fail;
            }
            catch(OperationCanceledException)
            {
                Debug.Log("Paying Cost Canceled");
                return BoolOperationResult.Canceled;
            }
        }

        private async Task<bool> ShowRV(CancellationToken ct)
        {
            
            try
            {
                ct.ThrowIfCancellationRequested();

                bool rvConcluded = false;
                bool rvReward = false;

                TafraAds.ShowRewardedAd("dailyChallengePlay", null, 
                onReward: () => 
                {
                    rvReward = true;
                },
                onComplete: () =>
                {
                    rvConcluded = true;
                },
                onFailed: () => 
                {
                    rvConcluded = true;
                });

                while (!rvConcluded)
                {
                    await Task.Yield();

                    ct.ThrowIfCancellationRequested();
                }

                return rvReward;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }

        public override string GetCostString()
        {
            return "<sprite=\"RVIcon\" index=0> ";
        }
    }
}