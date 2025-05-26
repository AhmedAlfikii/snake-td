using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit
{
    public class GameSection : MonoBehaviour, IResettable
    {
        [SerializeField] private bool initializeOnStart;
        [SerializeField] private bool startOnInitialization;

        [Header("Binding")]
        [Tooltip("Game Objects that will be enabled if the section is active and disabled if it's not.")]
        [SerializeField] private GameObject[] boundGameObjects;

        [SerializeField] private UnityEvent onInitialize;
        [SerializeField] private UnityEvent onWasLoadedFlag;
        [SerializeField] private UnityEvent onStart;
        [SerializeField] private UnityEvent onComplete;


        private bool isInitialized;
        private bool isPlaying;

        public bool IsInitialized => isInitialized;
        public bool IsPlaying => isPlaying;
        public UnityEvent OnStart => onStart;
        public UnityEvent OnWasLoadedFlag => onWasLoadedFlag;
        public UnityEvent OnInitialize => onInitialize;
        public UnityEvent OnComplete => onComplete;

        private void Start()
        {
            if (initializeOnStart)
                Initialize();
        }

        public void Initialize()
        {
            if (isInitialized)
                TafraDebugger.Log("Game Section", $"Section ({this.GetType()}) is already initialized, no need to do it agian.", TafraDebugger.LogType.Info);

            OnInitialized();
            
            isInitialized = true;

            onInitialize?.Invoke();

            if (startOnInitialization)
                StartSection();
            else
            {
                for (int i = 0; i < boundGameObjects.Length; i++)
                {
                    boundGameObjects[i].SetActive(false);
                }
            }
        }
        public void StartSection()
        {
            for (int i = 0; i < boundGameObjects.Length; i++)
            {
                boundGameObjects[i].SetActive(true);
            }
            
            isPlaying = true;

            OnStarted();

            onStart?.Invoke();
        }
        public void Complete()
        {
            for (int i = 0; i < boundGameObjects.Length; i++)
            {
                boundGameObjects[i].SetActive(false);
            }
            
            isPlaying  = false;
            
            OnCompleted();

            onComplete?.Invoke();
        }
        public void RaiseLoadedFlag()
        {
            onWasLoadedFlag?.Invoke();
        }

        /// <summary>
        /// Contains the sub-sections of this section if any (will return null if this section doesn't contain a sub-section property).
        /// </summary>
        /// <returns></returns>
        public virtual List<GameSection> GetSubSections()
        {
            return null;
        }

        protected virtual void OnInitialized() { }
        protected virtual void OnStarted() { }
        protected virtual void OnCompleted() { }

        public virtual void ResetSavedData()
        {

        }
    }
}