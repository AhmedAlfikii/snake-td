using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public static class InputTransmitter
    {
        public class InputTrack
        {
            public List<Input> ActiveInputs = new List<Input>();
            public float InputsLifeTime;
            public int MaxActiveInputs;

            public InputTrack(float inputsLifeTime, int maxActiveInputs)
            {
                ActiveInputs = new List<Input>();
                InputsLifeTime = inputsLifeTime;
                MaxActiveInputs = maxActiveInputs;
            }
        }
        public struct Input
        {
            public object Value;
            public float RegisteredTime;

            public Input(object value, float registeredTime)
            {
                Value = value;
                RegisteredTime = registeredTime;
            }
        }

        public static Action<string> OnTrackReceivedInput;
        public static Action OnInputsDisabled;
        public static Action OnInputsEnabled;

        private static List<string> inputTrackIds = new List<string>();
        private static Dictionary<string, InputTrack> inputTracks = new Dictionary<string, InputTrack>();
        private static InfluenceReceiver<bool> inputDisablers = new InfluenceReceiver<bool>(ShouldReplaceInputDisabler, OnActiveInputDisablerInflunceUpdate, OnActiveInputDisablerControllerUpdate, OnAllInputDisablerCleared);
        private static bool inputsEnabled = true;

        #region Callbacks
        private static bool ShouldReplaceInputDisabler(bool newDisabler, bool oldDisabler)
        {
            return true;
        }
        private static void OnActiveInputDisablerInflunceUpdate(bool influence)
        {
            if (inputsEnabled && influence == true)
                DisableInputs();
        }
        private static void OnActiveInputDisablerControllerUpdate(string controllerID, bool influence)
        {
        }
        private static void OnAllInputDisablerCleared()
        {
            if (!inputsEnabled)
                EnableInputs();
        }
        #endregion

        #region Private Functions
        private static void DisableInputs()
        {
            if (!inputsEnabled)
                return;

            inputsEnabled = false;

            OnInputsDisabled?.Invoke();
        }
        private static void EnableInputs()
        {
            if (inputsEnabled)
                return;

            inputsEnabled = true;

            OnInputsEnabled?.Invoke();

            //Broadcast inputs in all input tracks.
            for (int i = 0; i < inputTrackIds.Count; i++)
            {
                Input input = GetTrackInput(inputTrackIds[i]);

                if (input.Value == null)
                    continue;

                OnTrackReceivedInput?.Invoke(inputTrackIds[i]);
            }

        }
        #endregion

        public static void RegisterInputTrack(string trackId, float inputsLifetime, int maxActiveInputs)
        {
            if(inputTracks.ContainsKey(trackId))
            {
                TafraDebugger.Log("Input Transmitter", "Input was already registered, no need to register again.", TafraDebugger.LogType.Verbose);
                return;
            }

            InputTrack track = new InputTrack(inputsLifetime, maxActiveInputs);

            inputTracks.Add(trackId, track);
            inputTrackIds.Add(trackId);
        }
        public static void RegisterInput(string trackId, object value)
        {
            if(inputTracks.ContainsKey(trackId))
            {
                InputTrack inputTrack = inputTracks[trackId];
                List<Input> trackInputs = inputTrack.ActiveInputs;

                trackInputs.Add(new Input(value, Time.unscaledTime));

                //Always make sure that the track doesn't contain more than one input.
                while(trackInputs.Count > inputTrack.MaxActiveInputs)
                    trackInputs.RemoveAt(0);

                //Only broadcast if the inputs are enabled, otherwise they will be automatically broadcasted when inputs gets enabled back.
                if (inputsEnabled)
                    OnTrackReceivedInput?.Invoke(trackId);
            }
        }

        public static Input ConsumeTrackInput(string trackId)
        {
            if(!inputsEnabled || !inputTracks.ContainsKey(trackId))
                return default;
            
            InputTrack inputTrack = inputTracks[trackId];
            List<Input> trackInputs = inputTrack.ActiveInputs;

            Input input = GetTrackInput(trackId);

            //Consume the input if it exists (regardless if it's life.
            if (trackInputs.Count > 0)
                trackInputs.RemoveAt(0);

            return input;
        }
        /// <summary>
        /// Returns the latest available track input without consuming it.
        /// </summary>
        /// <param name="trackId"></param>
        /// <returns></returns>
        public static Input GetTrackInput(string trackId)
        {
            if(!inputsEnabled || !inputTracks.ContainsKey(trackId))
                return default;
            
            InputTrack inputTrack = inputTracks[trackId];
            List<Input> trackInputs = inputTrack.ActiveInputs;

            if(trackInputs.Count == 0)
                return default;

            Input input = trackInputs[0];

            //If the lifetime of the input has already passed, then this input isn't valid.
            if (input.RegisteredTime + inputTrack.InputsLifeTime < Time.unscaledTime)
            {
                trackInputs.RemoveAt(0);
                return default;
            }
            
            return input;
        }

        public static void ClearTrackInputs(string trackId)
        {
            if(!inputTracks.ContainsKey(trackId))
                return;

            inputTracks[trackId].ActiveInputs.Clear();
        }

        public static void AddInputDisabler(string disablerId)
        {
            inputDisablers.AddInfluence(disablerId, true);
        }
        public static void RemoveInputDisabler(string disablerId)
        {
            inputDisablers.RemoveInfluence(disablerId);
        }
    }
}