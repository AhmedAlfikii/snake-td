using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit
{
    [Serializable]
    public class ListenToEvent
    {
        public enum AcceptedTypes
        {
            None, Void, Int, Float, String, Bool
        }
        public enum NumberRelations
        {
            Any, Equals, GreaterThan, LessThan, GreaterThanOrEqual, LessThanOrEqual
        }
        public enum StringRelations
        {
            Any, Equals, Contiains, EndsWith, StartsWith
        }
        public enum BoolRelations
        {
            Any, True, False
        }

        public UnityEngine.Object Target;
        public string ComponentName;
        public string EventName;

        public UnityEvent OnEventFulfilled = new UnityEvent();

        #region Condition Fields
        //Number events condition fields
        [SerializeField] private NumberRelations numberRelation;
        [SerializeField] private float numberToCompareAgainst;

        //String events condition fields
        [SerializeField] private StringRelations stringRelation;
        [SerializeField] private string stringToCompareAgainst;

        //Boolk events condition fields
        [SerializeField] private BoolRelations boolRelation;
        #endregion

        private UnityEvent targetEvent;
        private UnityEvent<int> intTargetEvent;
        private UnityEvent<float> floatTargetEvent;
        private UnityEvent<string> stringTargetEvent;
        private UnityEvent<bool> boolTargetEvent;

        private UnityAction eventFulfilledAction;

        [SerializeField] private AcceptedTypes selectedType;

        #if UNITY_EDITOR
        [SerializeField] private bool isExpanded;
        #endif

        private bool PreInitialization(out object evFieldValue)
        {
            evFieldValue = null;

            if (Target == null)
                return false;

            if (Target is Component)
                Target = ((Component)Target).gameObject;

            Type t = null;
            UnityEngine.Object eventHolder = null;
            if (Target is GameObject)
            {
                Component c = ((GameObject)Target).GetComponent(ComponentName);
                eventHolder = c;

                if (c == null)
                    return false;

                t = c.GetType();
            }
            else
            {
                t = Target.GetType();
                eventHolder = Target;
            }

            if (t == null)
                return false;

            FieldInfo evField = t.GetField(EventName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            evFieldValue = evField.GetValue(eventHolder);

            return true;
        }

        public bool Initialize(UnityAction onEventFulfilled)
        {
            #region Getting Required Field
            if (Target == null)
                return false;

            if (Target is Component)
                Target = ((Component)Target).gameObject;

            Type t = null;
            UnityEngine.Object eventHolder = null;
            if (Target is GameObject)
            {
                Component c = ((GameObject)Target).GetComponent(ComponentName);
                eventHolder = c;

                if (c == null)
                    return false;

                t = c.GetType();
            }
            else
            {
                t = Target.GetType();
                eventHolder = Target;
            }

            if (t == null)
                return false;

            FieldInfo evField = t.GetField(EventName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (evField == null) return false;

            object evFieldValue = evField.GetValue(eventHolder);

            if (evFieldValue == null) return false;
            #endregion

            switch (selectedType)
            {
               case AcceptedTypes.Void:
                    targetEvent = (UnityEvent)evFieldValue;
                    break;
                case AcceptedTypes.Int:
                    intTargetEvent = (UnityEvent<int>)evFieldValue;
                    break;
                case AcceptedTypes.Float:
                    floatTargetEvent = (UnityEvent<float>)evFieldValue;
                    break;
                case AcceptedTypes.String:
                    stringTargetEvent = (UnityEvent<string>)evFieldValue;
                    break;
                case AcceptedTypes.Bool:
                    boolTargetEvent = (UnityEvent<bool>)evFieldValue;
                    break;
                default:
                    return false;
            }

            OnEventFulfilled.RemoveAllListeners();

            if (onEventFulfilled != null)
                OnEventFulfilled.AddListener(onEventFulfilled);

            return true;
        }

        public void StartListening()
        {
            if (targetEvent != null) targetEvent.AddListener(EventFired);
            else if (intTargetEvent != null) intTargetEvent.AddListener(EventFired);
            else if (floatTargetEvent != null) floatTargetEvent.AddListener(EventFired);
            else if (stringTargetEvent != null) stringTargetEvent.AddListener(EventFired);
            else if (boolTargetEvent != null) boolTargetEvent.AddListener(EventFired);
        }

        public void StopListening()
        {
            if (targetEvent != null) targetEvent.RemoveListener(EventFired);
            else if (intTargetEvent != null) intTargetEvent.RemoveListener(EventFired);
            else if (floatTargetEvent != null) floatTargetEvent.RemoveListener(EventFired);
            else if (stringTargetEvent != null) stringTargetEvent.RemoveListener(EventFired);
            else if (boolTargetEvent != null) boolTargetEvent.RemoveListener(EventFired);
        }

        void EventFired()
        {
            OnEventFulfilled?.Invoke();
        }
        void EventFired(int i)
        {
            switch (numberRelation)
            {
                case NumberRelations.Any:
                    OnEventFulfilled?.Invoke();
                    break;
                case NumberRelations.Equals:
                    if (i == numberToCompareAgainst)
                        OnEventFulfilled?.Invoke();
                    break;
                case NumberRelations.GreaterThan:
                    if (i > numberToCompareAgainst)
                        OnEventFulfilled?.Invoke();
                    break;
                case NumberRelations.LessThan:
                    if (i < numberToCompareAgainst)
                        OnEventFulfilled?.Invoke();
                    break;
                case NumberRelations.GreaterThanOrEqual:
                    if (i >= numberToCompareAgainst)
                        OnEventFulfilled?.Invoke();
                    break;
                case NumberRelations.LessThanOrEqual:
                    if (i <= numberToCompareAgainst)
                        OnEventFulfilled?.Invoke();
                    break;
            }
        }
        void EventFired(float f)
        {
            switch (numberRelation)
            {
                case NumberRelations.Any:
                    OnEventFulfilled?.Invoke();
                    break;
                case NumberRelations.Equals:
                    if (f == numberToCompareAgainst)
                        OnEventFulfilled?.Invoke();
                    break;
                case NumberRelations.GreaterThan:
                    if (f > numberToCompareAgainst)
                        OnEventFulfilled?.Invoke();
                    break;
                case NumberRelations.LessThan:
                    if (f < numberToCompareAgainst)
                        OnEventFulfilled?.Invoke();
                    break;
                case NumberRelations.GreaterThanOrEqual:
                    if (f >= numberToCompareAgainst)
                        OnEventFulfilled?.Invoke();
                    break;
                case NumberRelations.LessThanOrEqual:
                    if (f <= numberToCompareAgainst)
                        OnEventFulfilled?.Invoke();
                    break;
            }
        }
        void EventFired(string s)
        {
            switch (stringRelation)
            {
                case StringRelations.Any:
                    OnEventFulfilled?.Invoke();
                    break;
                case StringRelations.Equals:
                    if (s == stringToCompareAgainst)
                        OnEventFulfilled?.Invoke();
                    break;
                case StringRelations.Contiains:
                    if (s.Contains(stringToCompareAgainst))
                        OnEventFulfilled?.Invoke();
                    break;
                case StringRelations.StartsWith:
                    if (s.StartsWith(stringToCompareAgainst))
                        OnEventFulfilled?.Invoke();
                    break;
                case StringRelations.EndsWith:
                    if (s.EndsWith(stringToCompareAgainst))
                        OnEventFulfilled?.Invoke();
                    break;
            }
        }
        void EventFired(bool b)
        {
            switch (boolRelation)
            {
                case BoolRelations.Any:
                    OnEventFulfilled?.Invoke();
                    break;
                case BoolRelations.True:
                    if (b)
                        OnEventFulfilled?.Invoke();
                    break;
                case BoolRelations.False:
                    if (!b)
                        OnEventFulfilled?.Invoke();
                    break;
            }
        }
    }
}