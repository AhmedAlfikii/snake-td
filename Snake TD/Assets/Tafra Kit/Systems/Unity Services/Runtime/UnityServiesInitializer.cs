#if TAFRA_IAP
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using System;
using Unity.Services.Core.Environments;
using TafraKit.Tasks;

namespace TafraKit
{
    //TODO: Consider stopping the initialize async method when the editor stops playing.
    public static class UnityServiesInitializer
    {
        private static ServicesInitializationState state;
        private static Task<BoolOperationResult> initializationTask;

        public static async Task<BoolOperationResult> Initialize()
        {
            if (initializationTask != null)
            {
                if (initializationTask.IsCompleted)
                    return initializationTask.Result;
                else if (initializationTask.IsFaulted || initializationTask.IsCanceled)
                {
                    initializationTask = Initializing();
                    await initializationTask;
                    return initializationTask.Result;
                }
                else
                {
                    await initializationTask;
                    return initializationTask.Result;
                }
            }
            else
            {
                initializationTask = Initializing();
                await initializationTask;
                return initializationTask.Result;
            }
        }

        private static async Task<BoolOperationResult> Initializing()
        {
            try
            {
                InitializationOptions options = new InitializationOptions().SetEnvironmentName("production");

                await UnityServices.InitializeAsync();

                return BoolOperationResult.Success;
            }
            catch (Exception exception)
            {
                TafraDebugger.Log("Unity Service Initializer", $"Failed to initialize Unity Services. Error: {exception.Message}", TafraDebugger.LogType.Error);
                return BoolOperationResult.Fail;
            }
        }
    }
}
#endif