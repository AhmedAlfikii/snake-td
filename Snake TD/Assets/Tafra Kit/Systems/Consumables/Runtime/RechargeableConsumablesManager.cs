using System.Collections;
using TafraKit.ExternalSDKs.BalancySDK;
using UnityEngine;

namespace TafraKit.Consumables
{
    public class RechargeableConsumablesManager : MonoBehaviour
    {
        #region Private Serialized Fields
        [SerializeField] private float tickInterval = 1;
        [SerializeField] private RechargeableConsumable[] consumables;
        #endregion

        #region Private Fields
        private float remainingTimeToTick;
        private bool isInitialized;
        #endregion

        #region Monobehaviour Messages
        private void Start()
        {
            Initialize();
        }
        private void Update()
        {
            if (!isInitialized)
                return;

            if (remainingTimeToTick <= 0)
            {
                remainingTimeToTick = tickInterval;

                for (int i = 0; i < consumables.Length; i++)
                {
                    if (consumables[i].IsRecharging)
                        consumables[i].RechargeTick();
                }
            }
            else
                remainingTimeToTick -= Time.unscaledDeltaTime;
        }
        #endregion

        #region Private Functions
        private void Initialize()
        {
            StartCoroutine(DelayedInitialize());
        }
        private IEnumerator DelayedInitialize()
        {
            yield return null;

            for (int i = 0; i < consumables.Length; i++)
            {
                consumables[i].InitializeTimer();
            }

            isInitialized = true;
        }
        #endregion
    }
}
