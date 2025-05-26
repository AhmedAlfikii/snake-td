using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TafraKit.Tasks;

namespace TafraKit.MetaGame
{
    public class DailyChallengePlayCost
    {
        public virtual async Task<BoolOperationResult> PayCost(int year, int month, int day, CancellationToken ct)
        {
            try
            {
                await Task.Yield();
                
                return BoolOperationResult.Success;
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Paying Cost Canceled");
                return  BoolOperationResult.Canceled;
            }
        }

        public virtual string GetCostString()
        {
            return "X";
        }
    }
}