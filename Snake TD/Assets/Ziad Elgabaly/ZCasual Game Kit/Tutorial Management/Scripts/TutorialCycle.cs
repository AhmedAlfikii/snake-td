using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ZUtilities;
using TafraKit;

namespace ZCasualGameKit
{
    public class TutorialCycle : MonoBehaviour
    {
        [SerializeField] private string cycleID = "tutorialCycle";
        [Tooltip("The ID of the cycle this cycle is tied to, meaning that this cycle will not be considered completed unless the tied cycle is completed.")]
        [SerializeField] private string tiedCycleID;
        [SerializeField] private bool canRestartAfterCompletion;
        [SerializeField] private bool disableInEditor;

        [Header("Execution")]
        [Tooltip("Should this cycle automatically start listening to its execution event? If false, it will have to be activated during runtime.")]
        [SerializeField] private bool activateByDefault;
        [Tooltip("The event that will execute this cycle.")]
        [SerializeField] private ListenToEvent executionEvent;

        [Header("Steps")]
        [SerializeField] private TutorialStep[] steps;

        [Header("Events")]
        [SerializeField] public UnityEvent OnActivated;
        [SerializeField] public UnityEvent OnExecuted;
        [SerializeField] public UnityEvent OnCompleted;
        [SerializeField] public UnityEvent OnNoNeedToActivate;
        [SerializeField] public IntUnityEvent OnStepActivated;
        [SerializeField] public IntUnityEvent OnStepExecuted;
        [SerializeField] public IntUnityEvent OnStepCompleted;

        private int curStepIndex = -1;

        void Start()
        {
#if UNITY_EDITOR
            //return;
            //string text = $"<b>{cycleID}</b>\n";
            //for (int i = 0; i < steps.Length; i++)
            //{
            //    text += "step " + i.ToString("000") + "\n";
            //    text += "\"" + steps[i].GetGuide() + "\"\n";
            //}
            //Debug.Log(text);

            if(disableInEditor)
                return;
#endif
            for (int i = 0; i < steps.Length; i++)
            {
                steps[i].Initialize(this);
            }

            //If this cycle wasn't completed before.
            if (!PlayerPrefs.HasKey(cycleID + "_completed") || (!string.IsNullOrEmpty(tiedCycleID) && !PlayerPrefs.HasKey(tiedCycleID + "_completed")))
            {
                //If there's a progress saved for this cycle.
                if (PlayerPrefs.HasKey(cycleID + "_step"))
                {
                    int savedStepIndex = PlayerPrefs.GetInt(cycleID + "_step");
                    curStepIndex = savedStepIndex;
                    StartCoroutine(ResumeCurrentStepAfterOneFrame(PlayerPrefs.GetInt(cycleID + "_saved_step_state")));
                    OnExecuted?.Invoke();
                }
                //If this is a fresh start.
                else
                {
                    if (activateByDefault)
                        Activate();
                }
            }
            else if (activateByDefault)
                OnNoNeedToActivate?.Invoke();
        }

        void Execute()
        {
            if (steps.Length > 0)
            {
                curStepIndex = 0;

                StartCoroutine(ActivateCurrentStepAfterOneFrame());
            }
            else
                CompleteCycle();

            executionEvent.StopListening();
            OnExecuted?.Invoke();
        }

        void CompleteCycle()
        {
            if (!canRestartAfterCompletion)
                PlayerPrefs.SetInt(cycleID + "_completed", 1);
            else
            {
                PlayerPrefs.DeleteKey(cycleID + "_completed");
                PlayerPrefs.DeleteKey(cycleID + "_step");
                PlayerPrefs.DeleteKey(cycleID + "_saved_step_state");
            }


            OnCompleted?.Invoke();
        }

        void OnCurStepExecuted()
        {
            if (steps[curStepIndex].ShouldSaveAtExecution())
            {
                PlayerPrefs.SetInt(cycleID + "_step", curStepIndex);
                PlayerPrefs.SetInt(cycleID + "_saved_step_state", 1);   //1 means it's executed. 0 Means activated.
            }

            OnStepExecuted?.Invoke(curStepIndex);
        }

        void OnCurStepCompleted()
        {
            steps[curStepIndex].OnExecuted.RemoveListener(OnCurStepExecuted);
            steps[curStepIndex].OnCompleted.RemoveListener(OnCurStepCompleted);

            OnStepCompleted?.Invoke(curStepIndex);
            
            if (curStepIndex >= steps.Length - 1)
            {
                CompleteCycle();
                return;
            }

            curStepIndex++;

            StartCoroutine(ActivateCurrentStepAfterOneFrame());
        }

        IEnumerator ActivateCurrentStepAfterOneFrame()
        {
            yield return null;

            if (steps[curStepIndex].ShouldSaveAtActivation())
            {
                PlayerPrefs.SetInt(cycleID + "_step", curStepIndex);
                PlayerPrefs.SetInt(cycleID + "_saved_step_state", 0);   //0 means it's activated. 1 Means executed.
            }

            steps[curStepIndex].OnExecuted.AddListener(OnCurStepExecuted);
            steps[curStepIndex].OnCompleted.AddListener(OnCurStepCompleted);

            steps[curStepIndex].Activate();

            OnStepActivated?.Invoke(curStepIndex);
        }
        IEnumerator ResumeCurrentStepAfterOneFrame(int state)
        {
            yield return null;

            steps[curStepIndex].OnExecuted.AddListener(OnCurStepExecuted);
            steps[curStepIndex].OnCompleted.AddListener(OnCurStepCompleted);

            #region Highlight, point or show guide that should've remained shown from previous steps in a normal sequence once the step is executed.
            if (curStepIndex > 0)
            {
                steps[curStepIndex].OnExecuted.AddListener(() =>
                {
                    bool highlighterResolved = false;
                    bool pointerResolved = false;
                    //bool guideResolved = false;

                    for (int i = curStepIndex - 1; i > -1; i--)
                    {
                        if (!highlighterResolved)
                        {
                            if (steps[i].HidesHighlighter())
                                highlighterResolved = true;
                            else if (steps[i].UsesHighlighter())
                            {
                                steps[i].Highlight();
                                highlighterResolved = true;
                            }
                        }

                        if (!pointerResolved)
                        {
                            if (steps[i].HidesPointer())
                                pointerResolved = true;
                            else if (steps[i].UsesPointer())
                            {
                                steps[i].Point();
                                pointerResolved = true;
                            }
                        }

                        //if (!guideResolved)
                        //{
                        //    if (steps[i].HidesGuide())
                        //        guideResolved = true;
                        //    else if (steps[i].UsesGuide())
                        //    {
                        //        steps[i].ShowGuide();
                        //        guideResolved = true;
                        //    }
                        //}

                        if (highlighterResolved && pointerResolved /*&& guideResolved*/)
                            break;
                    }
                });
            }
            #endregion

            int resumedStep = curStepIndex;
            if (steps[curStepIndex].GetOnResumeAction() == TutorialStep.ResumingAction.Skip)
            {
                curStepIndex++;

                if (curStepIndex < steps.Length - 1)
                    StartCoroutine(ActivateCurrentStepAfterOneFrame());
                else
                    CompleteCycle();
            }
            else
                steps[curStepIndex].Resume(state);

            #if UNITY_EDITOR
            Debug.Log($"<b>Tutorial Cycle: checking for chained steps resuming. Starting at step {curStepIndex}</b>");
            #endif

            //If the resumed step was set to complete on resume, then check if the next step should also complete (and repeat the check on next steps until one breaks the cycle).
            while (resumedStep != curStepIndex)
            {
                if (curStepIndex > steps.Length - 1)
                {
                    curStepIndex = steps.Length - 1;
                    break;
                }

                Debug.Log("<b>The current step is: " + curStepIndex + "</b>");
                resumedStep = curStepIndex;
               
                //If the new step is set to not save, then this means we should move to the next step since it should be ignored from saving/loading.
                if (steps[curStepIndex].GetSavingState() == TutorialStep.SavingState.None)
                {
                    steps[curStepIndex].Complete();
                    //OnCurStepCompleted();
                    curStepIndex++;
                }
                //If the new step is set to save on activation (at least), then check its resuming action and perform it.
                else if (steps[curStepIndex].GetSavingState() == TutorialStep.SavingState.OnActivation || steps[curStepIndex].GetSavingState() == TutorialStep.SavingState.OnActivationAndExecution)
                {
                    steps[curStepIndex].OnResumed?.Invoke();

                    if (steps[curStepIndex].GetOnResumeAction() == TutorialStep.ResumingAction.Execute)
                    {
                        steps[curStepIndex].Execute();
                        OnCurStepExecuted();
                        curStepIndex++;
                    }
                    else if (steps[curStepIndex].GetOnResumeAction() == TutorialStep.ResumingAction.ExecuteAndComplete)
                    {
                        steps[curStepIndex].Complete();
                        //OnCurStepCompleted();
                        curStepIndex++;
                    }
                    else if (steps[curStepIndex].GetOnResumeAction() == TutorialStep.ResumingAction.Skip)
                    {
                        curStepIndex++;
                    }
                }
                //If the new step is set to save on exection, and is set to automatically execute, then check if it should complete on resuming.
                else if (steps[curStepIndex].GetSavingState() == TutorialStep.SavingState.OnExecution && steps[curStepIndex].AutomaticallyExecute)
                {
                    steps[curStepIndex].OnResumed?.Invoke();

                    if (steps[curStepIndex].GetOnResumeAction() == TutorialStep.ResumingAction.ExecuteAndComplete)
                    {
                        steps[curStepIndex].Complete();
                        //OnCurStepCompleted();
                        curStepIndex++;
                    }
                    else if (steps[curStepIndex].GetOnResumeAction() == TutorialStep.ResumingAction.Execute)
                    {
                        steps[curStepIndex].Execute();
                        OnCurStepExecuted();
                    }
                    else if (steps[curStepIndex].GetOnResumeAction() == TutorialStep.ResumingAction.Skip)
                    {
                        curStepIndex++;
                        Debug.Log("Should skip");
                    }
                }
            }
            #if UNITY_EDITOR
            Debug.Log($"<b>Tutorial Cycle: finished chained steps resuming.Ending at step {curStepIndex}</b>");
            #endif
        }

        public void Activate()
        {
            //if (PlayerPrefs.HasKey(cycleID + "_completed"))
            //    return;
            //If it's already playing, no need to activate it again.
            if (curStepIndex > -1)
                return;

            bool foundExecutionEvent = executionEvent.Initialize(Execute);

            OnActivated?.Invoke();

            if (!foundExecutionEvent)
                Execute();
            else
                executionEvent.StartListening();
        }
        public void ForceCompleteCurStep()
        { 
            if (curStepIndex > -1 && curStepIndex < steps.Length)
                steps[curStepIndex].Complete();
        }

        public string GetCycleID()
        {
            return cycleID;
        }
    }
}