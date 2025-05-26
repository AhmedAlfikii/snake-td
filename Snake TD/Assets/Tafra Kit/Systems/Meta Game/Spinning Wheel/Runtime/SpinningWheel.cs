using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TafraKit.UI;
using TafraKit.Consumables;
using TafraKit.Ads;
using ZUI;
using System.Text;
using UnityEngine.Events;
using TafraKit.Mathematics;

namespace TafraKit.MetaGame
{
    public class SpinningWheel : MonoBehaviour
    {
        [SerializeField] private string wheelId = "spinningWheel";

        [Header("Rewards")]
        [SerializeField] private WeightedConsumableRewardsGroup rewardsGroup;

        [Header("Reward Calculation")]
        [SerializeField] private ScriptableFloat controller;
        [SerializeField] private FormulasContainer rewardEquation;

        [Header("Properties")]
        [SerializeField] private int freeSpins = 1;
        [SerializeField] private int rvSpins = 1;
        [SerializeField] private float slotsStartRotation = 90;
        [Tooltip("The order direction of which slots will be assigned in.")]
        [SerializeField] private bool slotsAssignClockwise = true;
        [SerializeField] private int fullRotations = 5;
        [SerializeField] private float rotationDuration = 4f;
        [SerializeField] private EasingType rotationEasingType;

        [Header("Idle State")]
        [SerializeField] private float idleAutoRotateSpeed;

        [Header("Spin Button")] 
        [SerializeField] private UIElement buttonsHolderUIE;
        [SerializeField] private ZButton freeSpinBtn;
        [SerializeField] private TextMeshProUGUI freeSpinBTNTXT;
        [SerializeField] private bool displayFreeSpinsProgress = true;
        [SerializeField] private ZButton rvSpinBtn;

        [Header("References")]
        [SerializeField] private UIElementsGroup myUIE;
        [SerializeField] private Transform rotatableBody;
        [SerializeField] private DynamicPool<SpinningWheelSlot> slotsPool;

        [Header("Events")] 
        [HideInInspector] private UnityEvent onShow = new UnityEvent();
        [HideInInspector] private UnityEvent onHide = new UnityEvent();
        [HideInInspector] private UnityEvent onSpinStarted = new UnityEvent();
        [HideInInspector] private UnityEvent onSpinCompleted = new UnityEvent();
        [HideInInspector] private UnityEvent<ConsumableChange> onRewardTaken = new UnityEvent<ConsumableChange>();

        public bool FreeSpinAvailable => availableFreeSpins > 0;
        public bool RVSpinAvailable => rvSpins == -1 || availableRVSpins > 0;

        private bool isTurning;
        private CancellationTokenSource turnCTS;
        private float slotAngle;
        private List<ConsumableChange> tempSFs = new List<ConsumableChange>();
        private int availableFreeSpins;
        private int availableRVSpins;
        private Vector3 curBodyLocalEuler;
        private StringBuilder spinButtonSB = new StringBuilder();
        private List<SpinningWheelSlot> spawnedSlots = new List<SpinningWheelSlot>();
        private float fullReward;

        private const string freeSpinsPrefsKeyPrefix = "TAFRA_SPINNING_WHEEL_FREE_SPINS_";
        private const string RVSpinsPrefsKeyPrefix = "TAFRA_SPINNING_WHEEL_RV_SPINS_";

        public UnityEvent OnShow => onShow;
        public UnityEvent OnHide => onHide;
        public UnityEvent OnSpinStarted => onSpinStarted;
        public UnityEvent OnSpinCompleted => onSpinCompleted;
        public UnityEvent<ConsumableChange> OnRewardTaken => onRewardTaken;

        private void Awake()
        {
            availableFreeSpins = PlayerPrefs.GetInt(freeSpinsPrefsKeyPrefix + wheelId, freeSpins);
            availableRVSpins = PlayerPrefs.GetInt(RVSpinsPrefsKeyPrefix + wheelId, rvSpins);

            slotsPool.Initialize();

            slotAngle = 360 / (float)rewardsGroup.rewards.Length;

            fullReward = rewardEquation.Evaluate(controller.Value);

            for(int i = 0; i < rewardsGroup.rewards.Length; i++)
            {
                SpinningWheelSlot slot = slotsPool.RequestUnit();
                float angle = slotsStartRotation + (slotAngle * i * (slotsAssignClockwise ? -1 : 1));

                slot.transform.eulerAngles = new Vector3(0, 0, angle);

                slot.SetData(rewardsGroup.rewards[i].consumable.GetIcon(0), Mathf.RoundToInt(rewardsGroup.rewards[i].GetAmount(fullReward)));
                
                spawnedSlots.Add(slot);
            }

            curBodyLocalEuler = rotatableBody.localEulerAngles;
        }

        private void OnEnable()
        {
            transform.eulerAngles = Vector3.zero;
            
            freeSpinBtn.onClick.AddListener(OnSpinButtonClicked);
            rvSpinBtn.onClick.AddListener(OnSpinButtonClicked);
        }
        private void OnDisable()
        {
            if(turnCTS != null)
            {
                turnCTS.Cancel();
                turnCTS.Dispose();
                turnCTS = null;
            }

            freeSpinBtn.onClick.RemoveListener(OnSpinButtonClicked);
            rvSpinBtn.onClick.RemoveListener(OnSpinButtonClicked);
        }

        private void Update()
        {
            if (!isTurning && (idleAutoRotateSpeed > 0.001f || idleAutoRotateSpeed < -0.001f))
            {
                curBodyLocalEuler += new Vector3(0, 0, idleAutoRotateSpeed * Time.unscaledDeltaTime);
                rotatableBody.localEulerAngles = curBodyLocalEuler;
            }
        }
        private void OnSpinButtonClicked()
        {
            Turn();
        }
        private void RegisterFreeSpinUsed()
        {
            if(!FreeSpinAvailable)
                return;

            availableFreeSpins--;
            PlayerPrefs.SetInt(freeSpinsPrefsKeyPrefix + wheelId, availableFreeSpins);

            UpdateSpinStateVisuals();
        }
        private void RegisterRVSpinUsed()
        {
            if(!RVSpinAvailable)
                return;

            if (rvSpins != -1)
            {
                availableRVSpins--;
                PlayerPrefs.SetInt(RVSpinsPrefsKeyPrefix + wheelId, availableRVSpins);
            }
            
            UpdateSpinStateVisuals();
        }
        private void UpdateSpinStateVisuals()
        {
            freeSpinBtn.gameObject.SetActive(false);
            rvSpinBtn.gameObject.SetActive(false);

            freeSpinBtn.interactable = false;

            if (FreeSpinAvailable)
            {
                if(freeSpinBTNTXT)
                {
                    spinButtonSB.Clear();
                    
                    spinButtonSB.Append("Spin");

                    if(displayFreeSpinsProgress && freeSpins > 1)
                    {
                        spinButtonSB.Append(' ');
                        spinButtonSB.Append((freeSpins - availableFreeSpins) + 1);
                        spinButtonSB.Append(" / ");
                        spinButtonSB.Append(freeSpins);
                    }

                    freeSpinBTNTXT.text = spinButtonSB.ToString();
                }
                
                freeSpinBtn.gameObject.SetActive(true);
                freeSpinBtn.interactable = true;
            }
            else if (RVSpinAvailable)
                rvSpinBtn.gameObject.SetActive(true);
        }
        [ContextMenu("Show")]
        public void Show()
        {
            TimeScaler.SetTimeScale("spinningWheel", 0);

            myUIE.ChangeVisibility(true);

            buttonsHolderUIE.ChangeVisibilityImmediate(false);
            buttonsHolderUIE.ChangeVisibility(true);
            
            UpdateSpinStateVisuals();
            
            onShow?.Invoke();
        }
        [ContextMenu("Hide")]
        public void Hide()
        {
            TimeScaler.RemoveTimeScaleControl("spinningWheel");
            myUIE.ChangeVisibility(false);
            
            buttonsHolderUIE.ChangeVisibility(false);

            onHide?.Invoke();
        }
        public void ResetFreeSpin()
        {
            availableFreeSpins = freeSpins;
            PlayerPrefs.SetInt(freeSpinsPrefsKeyPrefix + wheelId, availableFreeSpins);

            UpdateSpinStateVisuals();
        }
        public void ResetRVSpin()
        {
            availableRVSpins = rvSpins;
            PlayerPrefs.SetInt(RVSpinsPrefsKeyPrefix + wheelId, availableRVSpins);

            UpdateSpinStateVisuals();
        }
        public void Turn()
        {
            if(turnCTS != null)
            {
                turnCTS.Cancel();
                turnCTS.Dispose();
            }

            turnCTS = new CancellationTokenSource();

            TurnAsync(FreeSpinAvailable, turnCTS.Token);
        }
        public async Task<ConsumableChange> TurnAsync(bool isFreeSpin, CancellationToken ct)
        {
            try
            {
                onSpinStarted?.Invoke();
                
                isTurning = true;
                
                buttonsHolderUIE.ChangeVisibility(false);

                freeSpinBtn.interactable = false;

                if(!isFreeSpin)
                {
                    bool rvConcluded = false;
                    bool rvReward = false;

                    TafraAds.ShowRewardedAd("spinningWheel", null,
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

                    while(!rvConcluded)
                    {
                        await Task.Yield();

                        ct.ThrowIfCancellationRequested();
                    }

                    if(!rvReward)
                        return null;
                }

                int chosenRewardIndex = rewardsGroup.GetRandomRewardIndex();
               
                float startTime = Time.unscaledTime;
                float endTime = startTime + rotationDuration;

                int slotsAssignDirection = slotsAssignClockwise ? 1 : -1;
                int rotationDirection = Mathf.RoundToInt(Mathf.Sign(fullRotations));

                float absZEuler = Mathf.Abs(curBodyLocalEuler.z);
                float closest360 = (absZEuler + (360 - (absZEuler % 360))) * rotationDirection;
                float endZEuler = closest360 + slotsStartRotation + (chosenRewardIndex * slotAngle * slotsAssignDirection) + (360 * fullRotations);
                float startZEuler = curBodyLocalEuler.z;

                while(Time.unscaledTime < endTime)
                {
                    ct.ThrowIfCancellationRequested();

                    float t = (Time.unscaledTime - startTime) / rotationDuration;
                    t = MotionEquations.GetEaseFloat(t, rotationEasingType);

                    curBodyLocalEuler = new Vector3(curBodyLocalEuler.x, curBodyLocalEuler.y, Mathf.LerpUnclamped(startZEuler, endZEuler, t));
                    rotatableBody.localEulerAngles = curBodyLocalEuler;

                    await Task.Yield();
                }
                curBodyLocalEuler = new Vector3(curBodyLocalEuler.x, curBodyLocalEuler.y, endZEuler);
                rotatableBody.localEulerAngles = curBodyLocalEuler;

                ConsumableChange reward = new ConsumableChange
                { 
                    changeAmount = (int) rewardsGroup.rewards[chosenRewardIndex].GetAmount(fullReward), 
                    consumable = rewardsGroup.rewards[chosenRewardIndex].consumable
                };

                tempSFs.Clear();
                tempSFs.Add(reward);

                bool rewardTaken = false;
                
                ItemRewarder.AddConsumables(tempSFs);
                onRewardTaken?.Invoke(reward);
                ItemRewarder.ShowScreen(()=> rewardTaken = true);

                while(!rewardTaken)
                {
                    ct.ThrowIfCancellationRequested();
                   
                    await Task.Yield();
                }

                return reward;
            }
            catch(OperationCanceledException) 
            {
                return null;
            }
            finally
            {
                isTurning = false;

                if(myUIE.Visible)
                    buttonsHolderUIE.ChangeVisibility(true);

                if(isFreeSpin)
                    RegisterFreeSpinUsed();
                else
                    RegisterRVSpinUsed();
                
                onSpinCompleted?.Invoke();
                
                //if(availableFreeSpins == 0 && availableRVSpins == 0)
                    //Hide();
            }
        }

        public Vector3 GetBodyCurrentEuler()
        {
            return curBodyLocalEuler;
        }
        public bool IsTurning()
        {
            return isTurning;
        }
    }
}