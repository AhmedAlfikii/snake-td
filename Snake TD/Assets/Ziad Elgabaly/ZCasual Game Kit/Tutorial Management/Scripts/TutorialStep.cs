using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TafraKit;
using TafraKit.UI;

namespace ZCasualGameKit
{
    [System.Serializable]
    public class TutorialStep
    {
        #region Stucts, Classes & Enums
        public enum SavingState { None, OnActivation, OnExecution, OnActivationAndExecution}
        public enum ResumingAction { None, Execute, ExecuteAndComplete, Skip }
        #endregion

        [Tooltip("Serves no purpose but to nicefy the array element name.")]
        [SerializeField] private string name;
        [SerializeField] private string guide;
        [Header("Settings")]
        [SerializeField] private SavingState savingState;
        
        [Header("Execution")]
        [Tooltip("Should this step automatically execute when the previous step is completed (or when the cycle executes in case this is the first step) or should it enter the \"Activated\" state?")]
        public bool AutomaticallyExecute = true;
        [Tooltip("The event that this step will be listening to during its \"Activated\" state, once it executes, this step will also be executed.")]
        public ListenToEvent ExecutionEvent = new ListenToEvent();

        [Header("Resuming")]
        [Tooltip("What should happen once this step is resumed?")]
        [SerializeField] private ResumingAction onResuming;

        [Header("Begining Functionality")]
        [SerializeField] private bool useHighlighter;
        [SerializeField] private GameObject[] gosToHighlight;
        [SerializeField] private RectTransform pointAt;
        [SerializeField] private int guidePositionIndex;

        [Header("Ending Functionality")]
        [SerializeField] private bool dehighlight;
        [SerializeField] private bool hidePointer;
        [SerializeField] private bool hideGuideBox;

        [Header("Completion")]
        [Tooltip("Set the duration that needs to pass after this step has executed in order for it to be completed (0 or less means don't automatically complete).")]
        [SerializeField] private float autoCompleteAfter = 0;
        [Tooltip("The event that needs to be fired for this step to complete.")]
        public ListenToEvent CompletionEvent = new ListenToEvent();

        [Header("Events")]
        [Tooltip("An event that fires once this step enters the \"Activated\" state.")]
        public UnityEvent OnActivate;
        [Tooltip("An event that fires once this step executes.")]
        public UnityEvent OnExecuted;
        [Tooltip("An event that fires once this step is completed.")]
        public UnityEvent OnCompleted;
        [Tooltip("An event that fires once this step is being resumed after the cycle has been forcefully terminated (game went offline) and then executed again.")]
        public UnityEvent OnResumed;

        private MonoBehaviour myMonoBehaviour;
        private IEnumerator autoCompleteEnum;

        public TutorialStep()
        {
            ExecutionEvent = new ListenToEvent();
            CompletionEvent = new ListenToEvent();
        }

        IEnumerator AutoCompleteAfterDelay()
        {
            yield return Yielders.GetWaitForSeconds(autoCompleteAfter);

            Complete();
        }

        public void Initialize(MonoBehaviour monoBehaviour)
        {
            myMonoBehaviour = monoBehaviour;
        }

        public void Activate()
        {
            if (!AutomaticallyExecute)
            {
                ExecutionEvent.Initialize(Execute);
                ExecutionEvent.StartListening();

                OnActivate?.Invoke();
            }
            else
                Execute();
        }

        public void Execute()
        {
            CompletionEvent.Initialize(Complete);
            CompletionEvent.StartListening();

            if (autoCompleteAfter > 0)
            {
                autoCompleteEnum = AutoCompleteAfterDelay();
                myMonoBehaviour.StartCoroutine(autoCompleteEnum);
            }

            if (useHighlighter)
                Highlight();

            if (pointAt)
                Point();

            if (!string.IsNullOrEmpty(guide))
                ShowGuide();

            ExecutionEvent.StopListening();
            OnExecuted?.Invoke();
        }

        public void Complete()
        {
            if(dehighlight)
                UIHighlighter.RemoveAllHighlightedObjects();

            if (hidePointer)
                UIHighlighter.RemovePointer();

            if (hideGuideBox)
                GuideBox.Instance.HideGuide();

            CompletionEvent.StopListening();
            
            if (autoCompleteEnum != null)
            myMonoBehaviour.StopCoroutine(autoCompleteEnum);

            OnCompleted?.Invoke();
        }

        public void Resume(int state)
        {
            OnResumed?.Invoke();

            if (onResuming == ResumingAction.ExecuteAndComplete)
            {
                Execute();
                Complete();
            }
            else if (onResuming == ResumingAction.Execute || state == 1)    //Resuming state 1 means that it should resume on execution (because it saved on execution).
                Execute();
            else if (state == 0)    //Resuming state 0 means that it should resume on activation (because it saved on activation).
                Activate();
        }

        public void Highlight()
        {
            for (int i = 0; i < gosToHighlight.Length; i++)
            {
                UIHighlighter.Highlight(gosToHighlight[i]);
            }
        }

        public void ShowGuide()
        {
            GuideBox.Instance.ShowGuide(guide, guidePositionIndex);
        }

        public void Point()
        {
            UIHighlighter.PointAt(pointAt);
        }

        public bool ShouldSaveAtActivation()
        {
            return savingState == SavingState.OnActivation || savingState == SavingState.OnActivationAndExecution;
        }
        public bool ShouldSaveAtExecution()
        {
            return savingState == SavingState.OnExecution || savingState == SavingState.OnActivationAndExecution;
        }
        public bool UsesHighlighter()
        {
            return useHighlighter;
        }
        public bool UsesPointer()
        {
            return pointAt != null;
        }
        public bool UsesGuide()
        {
            return !string.IsNullOrEmpty(guide);
        }
        public bool HidesHighlighter()
        {
            return dehighlight;
        }
        public bool HidesPointer()
        {
            return hidePointer;
        }
        public bool HidesGuide()
        {
            return hideGuideBox;
        }

        public string GetGuide()
        {
            return guide;
        }

        public ResumingAction GetOnResumeAction()
        {
            return onResuming;
        }
        public SavingState GetSavingState()
        {
            return savingState;
        }
    }
}