using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TafraKit;
using TafraKit.Tasks;
using TafraKit.MotionFactory;

namespace TafraKit.MotionFactory
{
    public abstract class VisibilityMotionController : MotionController
    {
        [SerializeField] protected bool applyDefaultState = true;
        [SerializeField] protected VisibilityControllerState defaultState = VisibilityControllerState.Shown;
        [SerializeField] protected bool useUnscaledTime;
        [SerializeField] protected bool deactivateWhileHidden;

        [SerializeField] protected UnityEvent onShowStart = new UnityEvent();
        [SerializeField] protected UnityEvent onShowComplete = new UnityEvent();
        [SerializeField] protected UnityEvent onHideStart = new UnityEvent();
        [SerializeField] protected UnityEvent onHideComplete = new UnityEvent();

        protected VisibilityControllerState state = VisibilityControllerState.None;
        protected ControlReceiver additionalPlayingMotions;

        protected bool isAdditionalMotionPlaying;
        protected bool motionsInitialized;
        protected bool dataInitialized;

        public UnityEvent OnShowStart => onShowStart;
        public UnityEvent OnShowComplete => onShowComplete;
        public UnityEvent OnHideStart => onHideStart;
        public UnityEvent OnHideComplete => onHideComplete;

        /// <summary>
        /// Is the controller currently shown or is showing?
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return state == VisibilityControllerState.Shown || state == VisibilityControllerState.Showing;
            }
        }
        public VisibilityControllerState State => state;

        protected virtual void Awake()
        {
            if(!dataInitialized)
                InitializeData();
        }

        protected void InitializeData()
        {
            if(dataInitialized)
                return;

            additionalPlayingMotions = new ControlReceiver(OnFirstAdditionalAnimationAdded, null, OnAllAdditionalAnimationsStopped);

            dataInitialized = true;
        }

        private void OnFirstAdditionalAnimationAdded()
        {
            isAdditionalMotionPlaying = true;
        }
        private void OnAllAdditionalAnimationsStopped()
        {
            isAdditionalMotionPlaying = false;
        }

        protected virtual async void Start()
        {
            if(!motionsInitialized)
            {
                InitializeMotions(false);

                if(applyDefaultState)
                    await ApplyDefaultState();
                else
                    state = VisibilityControllerState.None;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if(state == VisibilityControllerState.Showing || state == VisibilityControllerState.Hiding)
            {
                if(motionCTS != null)
                {
                    motionCTS.Cancel();
                    motionCTS.Dispose();
                }

                motionCTS = new CancellationTokenSource();
                CancellationToken ct = motionCTS.Token;;

                if(state == VisibilityControllerState.Showing)
                {
                    OnShowRequest(true, false, ct);
                    state = VisibilityControllerState.Shown;
                }
                else if(state == VisibilityControllerState.Hiding)
                {
                    OnHideRequest(true, false, ct);
                    state = VisibilityControllerState.Hidden;
                }
            }
        }

        private async Task ApplyDefaultState()
        {
            Task instantMotionTask = null;
            Task motionTask = null;

            motionCTS = new CancellationTokenSource();
            CancellationToken ct = motionCTS.Token;;

            switch(defaultState)
            {
                case VisibilityControllerState.Shown:
                case VisibilityControllerState.Interrupted:
                    OnShowStarted();
               
                    instantMotionTask = OnShowRequest(true, false, ct);
                    await instantMotionTask;
                
                    OnShowCompleted();
                    state = VisibilityControllerState.Shown;
                    break;
                case VisibilityControllerState.Showing:
                    instantMotionTask = OnHideRequest(true, false, ct);
                    await instantMotionTask;

                    if(ct.IsCancellationRequested)
                        break;

                    OnShowStarted();
                 
                    motionTask = OnShowRequest(false, false, ct);
                    await motionTask;
                   
                    OnShowCompleted();

                    state = VisibilityControllerState.Shown;
                    break;
                case VisibilityControllerState.Hidden:
                    OnHideStarted();

                    instantMotionTask = OnHideRequest(true, false, ct);
                    await instantMotionTask;

                    OnHideCompleted();
                    state = VisibilityControllerState.Hidden;
                    break;
                case VisibilityControllerState.Hiding:
                    instantMotionTask = OnShowRequest(true, false, ct);
                    await instantMotionTask;

                    if(ct.IsCancellationRequested)
                        break;

                    OnHideStarted();
                  
                    motionTask = OnHideRequest(false, false, ct);
                    await motionTask;

                    OnHideCompleted();
                 
                    state = VisibilityControllerState.Hidden;
                    break;
                default:
                    state = defaultState;
                    break;
            }

            if(instantMotionTask != null)
                instantMotionTask.Dispose();
            if(motionTask != null)
                motionTask.Dispose();
        }

        /// <summary>
        /// Show the element. This will cause the "In" motions to play, and the "Stay" motions to play afterwards if they exist.
        /// </summary>
        /// <param name="instant">Should the element instantly show without playing any "In" motion? This would still play the stay motions afterwards if they exist.</param>
        public void Show(bool instant = false)
        {
            if(!dataInitialized)
                InitializeData();

            if(!motionsInitialized)
                InitializeMotions(true);

            ChangeVisibility(true, instant);
        }
        /// <summary>
        /// Hide the element. This will cause the "Out" motions to play.
        /// </summary>
        /// <param name="instant">Should the element instantly HIDE without playing any "Out" motion?</param>
        public void Hide(bool instant = false)
        {
            if(!dataInitialized)
                InitializeData();

            if(!motionsInitialized)
                InitializeMotions(true);

            ChangeVisibility(false, instant);
        }

        private async void ChangeVisibility(bool visible, bool instant)
        {
            //TODO: Confirm this is the best approach.
            if(visible && !gameObject.activeSelf)
                gameObject.SetActive(true);

            bool isMidAnimation = state == VisibilityControllerState.Showing || state == VisibilityControllerState.Hiding || isAdditionalMotionPlaying;
            
            VisibilityControllerState doneState = visible ? VisibilityControllerState.Shown : VisibilityControllerState.Hidden;
            VisibilityControllerState doingState = visible ? VisibilityControllerState.Showing : VisibilityControllerState.Hiding;
            VisibilityControllerState doingOppositeState = visible ? VisibilityControllerState.Hiding : VisibilityControllerState.Showing;

            if(state == doingOppositeState || (state == doingState && instant))
            {
                motionCTS.Cancel();
            }
            else if(!instant && (state == doneState || state == doingState))    //the !instant check is redundent but will leave it there for clarity.
            {
                //string doString = visible ? "show" : "hide";
                //string doneString = visible ? "shown" : "hidden";
                //string doingString = visible ? "showing" : "hiding";

                //TafraDebugger.Log("ZUI Pro", $"The element is already {doneString} or is {doingString}. No need to {doString} it again.", TafraDebugger.LogType.Verbose, gameObject);
                TafraDebugger.Log("ZUI Pro", $"The element's state is {state}, no need to change it's visibility to {visible}.", TafraDebugger.LogType.Verbose, gameObject);
                return;
            }

            if(motionCTS != null)
                motionCTS.Dispose();

            motionCTS = new CancellationTokenSource();

            await ChangeVisibilityAsync(visible, instant, isMidAnimation, doneState, doingState, motionCTS.Token);
        }
        private async Task ChangeVisibilityAsync(bool visible, bool instant, bool isMidAnimation, VisibilityControllerState doneState, VisibilityControllerState doingState, CancellationToken ct)
        {
            UnityEvent startEvent = visible ? onShowStart : onHideStart;
            UnityEvent completeEvent = visible ? onShowComplete : onHideComplete;

            if(visible)
                OnShowStarted();
            else
                OnHideStarted();

            startEvent?.Invoke();

            state = doingState;

            Task motionTask = null;

            try
            {
                ct.ThrowIfCancellationRequested();
               
                motionTask = visible? OnShowRequest(instant, isMidAnimation, ct) : OnHideRequest(instant, isMidAnimation, ct);

                await motionTask;

                ct.ThrowIfCancellationRequested();

                state = doneState;

                if (visible)
                {
                    OnShowCompleted();
               
                    completeEvent?.Invoke();
                }
                else
                {
                    OnHideCompleted();
                
                    completeEvent?.Invoke();

                    if(deactivateWhileHidden)
                        gameObject.SetActive(false);
                }

            }
            catch(OperationCanceledException)
            {

            }
            finally
            {
                if(motionTask != null)
                    motionTask.Dispose();
            }
        }

        protected void InitializeMotions(bool forced)
        {
            motionsInitialized = true;

            if(forced)
                state = VisibilityControllerState.None;

            OnInitializeMotions();
        }
        protected abstract Task OnShowRequest(bool instant, bool isMidAnimation, CancellationToken ct);
        protected abstract Task OnHideRequest(bool instant, bool isMidAnimation, CancellationToken ct);

        protected abstract void OnInitializeMotions();

        protected virtual void OnShowStarted()
        { 

        }
        protected virtual void OnShowCompleted()
        { 

        }
        protected virtual void OnHideStarted()
        { 

        }
        protected virtual void OnHideCompleted()
        { 

        }
    }
}