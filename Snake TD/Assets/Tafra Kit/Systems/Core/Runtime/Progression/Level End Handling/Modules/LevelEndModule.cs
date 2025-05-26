using System;
using System.Collections;
using UnityEngine;

namespace TafraKit.Internal
{
    [Serializable]
    public abstract class LevelEndModule
    {
        [Header("Triggers")]
        [SerializeField] private bool onWin;
        [SerializeField] private bool onFail;
        [SerializeField] private bool onQuit;

        protected LevelEndHandler handler;
        private Action onEnd;

        public bool OnWin => onWin;
        public bool OnFail => onFail;
        public bool OnQuit => onQuit;

        public void Initialize(LevelEndHandler handler)
        {
            this.handler = handler;

            OnInitialize();
        }

        public void Start(Action onEnd)
        {
            this.onEnd = onEnd;

            OnStart();
        }
        protected void End()
        {
            onEnd?.Invoke();
            OnEnd();
        }

        protected virtual void OnInitialize() { }
        protected virtual void OnStart() { }
        protected virtual void OnEnd() { }
        protected virtual void OnResetSavedData() { }

        public void ResetSavedData()
        {
            OnResetSavedData();
        }
    }
}