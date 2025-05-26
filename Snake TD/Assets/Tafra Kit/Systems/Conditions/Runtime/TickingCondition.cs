using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Conditions
{
    /// <summary>
    /// A ticking condition has to be deactivated whenever it's holder is deactivated/destroyed to avoid having its task run forever.
    /// NOTE: DO NOT SATISFY THE CONDITION ON ACTIVATE. Simply implement the Check function.
    /// </summary>
    public abstract class TickingCondition : Condition
    {
        [SerializeField] private bool activelyCheck;

        [NonSerialized] private CancellationTokenSource updateCTS;
        [NonSerialized] private Task updateTask;

        protected override void OnActivate()
        {
            //We have to make sure the condition is still active in case it was satisfied in the update made.
            if (activelyCheck)
            {
                updateCTS = new CancellationTokenSource();

                //TODO: what if the task has a different state? handle disposing it.
                if(updateTask != null && (updateTask.Status == TaskStatus.RanToCompletion || updateTask.Status == TaskStatus.Faulted || updateTask.Status == TaskStatus.Canceled))
                    updateTask.Dispose();

                updateTask = Update(updateCTS.Token);
            }
        }
        protected override void OnDeactivate()
        {
            if (updateCTS != null)
            {
                updateCTS.Cancel();
                updateCTS.Dispose();
                updateCTS = null;
            }
        }

        private async Task Update(CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();

                await Task.Yield();

                ct.ThrowIfCancellationRequested();

                while(isActive)
                {
                    ct.ThrowIfCancellationRequested();

                    Check();

                    ct.ThrowIfCancellationRequested();

                    await Task.Yield();
                }
            }
            catch(OperationCanceledException)
            {
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}