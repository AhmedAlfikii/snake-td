using System;
using TafraKit.MobileNotifications;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit.Consumables
{
    [CreateAssetMenu(fileName = "Rechargeable Consumable", menuName = "Tafra Kit/Consumables/Rechargeable Consumable")]
    public class RechargeableConsumable : Consumable
    {
        #region Serialized Fields
        [Header("Recharging")]
        [SerializeField] private TimeSpanSimple rechargeDuration;
        [SerializeField] private TafraFloat autoRechargeCap;
        #if TAFRA_MOBILE_NOTIFICATIONS
        [SerializeField] private bool notifyOnFullyRecharge;
        [SerializeField] private Notification fullyRechargedNotification;
        #endif
        #endregion

        #region Events
        [NonSerialized] private readonly UnityEvent onRechargeStarted = new UnityEvent();
        [NonSerialized] private readonly UnityEvent<TimeSpan> onRechargeTimerUpdated = new UnityEvent<TimeSpan>();
        [NonSerialized] private readonly UnityEvent onRechargeCompleted = new UnityEvent();
        [NonSerialized] private readonly UnityEvent onRechargeCapChanged = new UnityEvent();
        #endregion

        #region Private Fields
        [NonSerialized] private DateTime lastRechargeTime;
        [NonSerialized] private DateTime nextRechargeTime;
        [NonSerialized] private TimeSpan remainingRechargeTime;
        [NonSerialized] private bool isRechargining;
        [NonSerialized] private bool sentFullChargeNotification;
        [NonSerialized] private int savedCap;
        [NonSerialized] private bool isTimerInitialized;
        private const string PrefsRechargeCap = "RECHARGE_CAP";
        private const string PrefsLastRechargeTime = "RECHARGE_LAST_TIME";
        private const string PrefsSentFullChargeNotification = "SENT_FULL_CHARGE";
        #endregion

        #region Public Properties
        public UnityEvent OnRechargeStarted => onRechargeStarted;
        public UnityEvent<TimeSpan> OnRechargeTimerUpdated => onRechargeTimerUpdated;
        public UnityEvent OnRechargeCompleted => onRechargeCompleted;
        public UnityEvent OnRechargeCapChanged => onRechargeCapChanged;
        public bool IsRecharging => isRechargining;
        public int CurrentRechargeCap => autoRechargeCap.ValueInt;
        #endregion

        #region Scriptable Object Messages
        protected override void OnEnable()
        {
            #if UNITY_EDITOR
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            #endif

            base.OnEnable();

            if (autoRechargeCap.ScriptableVariable)
                autoRechargeCap.ScriptableVariable.OnValueChange.AddListener(OnRechargeCapSFValueChanged);
        }
        private void OnDisable()
        {
            #if UNITY_EDITOR
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                return;
            #endif

            if (autoRechargeCap.ScriptableVariable)
                autoRechargeCap.ScriptableVariable.OnValueChange.RemoveListener(OnRechargeCapSFValueChanged);
        }
        #endregion

        #region Private Functions
        protected override void OnInitialized()
        {
            base.OnInitialized();

            LoadRechargeData();
        }
        private void LoadRechargeData()
        {
            savedCap = PlayerPrefs.GetInt(id + PrefsRechargeCap, autoRechargeCap.ValueInt);

            lastRechargeTime = DateTime.Parse(PlayerPrefs.GetString(id + PrefsLastRechargeTime,TafraDateTime.Now.ToString()));

            sentFullChargeNotification = PlayerPrefs.GetInt(id + PrefsSentFullChargeNotification, 0)==1;
        }
        private void SaveRechargeData()
        {
            PlayerPrefs.SetInt(id + PrefsRechargeCap, autoRechargeCap.ValueInt);

            PlayerPrefs.SetString(id + PrefsLastRechargeTime, lastRechargeTime.ToString());

            PlayerPrefs.SetInt(id + PrefsSentFullChargeNotification, sentFullChargeNotification ? 1:0);
        }
        private void StartRecharging(DateTime startTime)
        {
            isRechargining = true;

            lastRechargeTime = startTime;

            nextRechargeTime = lastRechargeTime + rechargeDuration.TimeSpan();

            #if TAFRA_MOBILE_NOTIFICATIONS
            //TODO: Do we unschedule this when the game restarts?
            if (notifyOnFullyRecharge)
            {
                int fullyRechargeDuration = (int)((autoRechargeCap.Value - currentValue) * rechargeDuration.TimeSpan().TotalSeconds);
                
                fullyRechargedNotification.DeliverAfter = new TimeSpanSimple(fullyRechargeDuration);

                NotificationsManager.ScheduleClosingNotification(fullyRechargedNotification, 0);
            }
            #endif

            SaveRechargeData();

            onRechargeStarted?.Invoke();
        }
        private void StopRecharging()
        {
            isRechargining = false;

            #if TAFRA_MOBILE_NOTIFICATIONS
            if (notifyOnFullyRecharge)
                NotificationsManager.UnscheduleClosingNotification(fullyRechargedNotification);
            #endif

            SaveRechargeData();
            
            onRechargeCompleted?.Invoke();
        }
        private void CalculateRecharges()
        {
            int remainingRecharges = autoRechargeCap.ValueInt - ValueInt;
            
            if(remainingRecharges <= 0)
                return;

            int rechargesToAdd = 0;
            
            nextRechargeTime = lastRechargeTime + rechargeDuration.TimeSpan();

            while (TafraDateTime.Now > nextRechargeTime)
            {
                if(rechargesToAdd >= remainingRecharges)
                    break;
                
                rechargesToAdd++;

                lastRechargeTime = nextRechargeTime;
                
                nextRechargeTime = lastRechargeTime + rechargeDuration.TimeSpan();
                
                SaveRechargeData();
            }
            
            if (rechargesToAdd > 0)
                Add(rechargesToAdd);

            remainingRechargeTime = nextRechargeTime - TafraDateTime.Now;

            onRechargeTimerUpdated?.Invoke(remainingRechargeTime);
        }
        #endregion

        #region Public Functions
        public void InitializeTimer()
        {
            CalculateRecharges();

            if(!isRechargining && ValueInt < autoRechargeCap.ValueInt)
                StartRecharging(lastRechargeTime);

            isTimerInitialized = true;
        }
        public void RechargeTick()
        {
            if(ValueInt >= autoRechargeCap.ValueInt)
            {
                StopRecharging();
                return;
            }

            CalculateRecharges();

            if(ValueInt >= autoRechargeCap.ValueInt)
                StopRecharging();
        }
        public override bool Deduct(float value, bool hidden = false)
        {
            if (!base.Deduct(value, hidden))
                return false;

            if (!isRechargining && currentValue < autoRechargeCap.ValueInt)
                StartRecharging(TafraDateTime.Now);

            return true;
        }
        #endregion

        #region Callbacks
        private void OnRechargeCapSFValueChanged(float value)
        {
            if (!isTimerInitialized)
                return;

            if (autoRechargeCap.ValueInt >= savedCap)
            {
                if (isRechargining)
                    StopRecharging();

                Add(autoRechargeCap.ValueInt - ValueInt);
            }
            else
            {
                if (!IsRecharging)
                    StartRecharging(TafraDateTime.Now);
            }

            savedCap = autoRechargeCap.ValueInt;

            SaveRechargeData();

            onRechargeCapChanged?.Invoke();
        }
        #endregion
    }
}
