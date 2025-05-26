using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit
{
    public class AnimationClipEventRedirector : MonoBehaviour
    {
        #region Private Fields
        private Dictionary<string, UnityEvent> eventsDic;
        #endregion
    
        #region Events
        public UnityEvent OnEvent1;
        public UnityEvent OnEvent2;
        public UnityEvent OnEvent3;
        public UnityEvent OnEvent4;
        public UnityEvent OnEvent5;
        public UnityEvent OnEvent6;
        public UnityEvent OnEvent7;
        public UnityEvent OnEvent8;
        public UnityEvent OnEvent9;
        public UnityEvent OnEvent10;
        #endregion
    
        #region Monobehaviour Messages
        private void Awake()
        {
            eventsDic = new Dictionary<string, UnityEvent>()
            {
                {"Event1" , OnEvent1},
                {"Event2" , OnEvent2},
                {"Event3" , OnEvent3},
                {"Event4" , OnEvent4},
                {"Event5" , OnEvent5},
                {"Event6" , OnEvent6},
                {"Event7" , OnEvent7},
                {"Event8" , OnEvent8},
                {"Event9" , OnEvent9},
                {"Event10", OnEvent10}
            };
        }
        #endregion

        #region Public Methods
        public UnityEvent GetEvent(string eventName)
        {
            return !eventsDic.ContainsKey(eventName) ? null : eventsDic[eventName];
        }
        public void Event1()
        {
            OnEvent1?.Invoke();
        }

        public void Event2()
        {
            OnEvent2?.Invoke();
        }

        public void Event3()
        {
            OnEvent3?.Invoke();
        }

        public void Event4()
        {
            OnEvent4?.Invoke();
        }

        public void Event5()
        {
            OnEvent5?.Invoke();
        }

        public void Event6()
        {
            OnEvent6?.Invoke();
        }

        public void Event7()
        {
            OnEvent7?.Invoke();
        }

        public void Event8()
        {
            OnEvent8?.Invoke();
        }

        public void Event9()
        {
            OnEvent9?.Invoke();
        }

        public void Event10()
        {
            OnEvent10?.Invoke();
        }
        #endregion
    }
}
