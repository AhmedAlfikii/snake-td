using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public interface ITafraPlayable
    {
        public bool IsPlaying { get; protected set; }
        public bool ShouldBePlaying { get; protected set; }
        public bool IsPaused { get; protected set; }

        protected HashSet<string> Pausers { get; }
        /// <summary>
        /// The playable was asked to play while it was paused. Once it resumes it should automatically play.
        /// </summary>
        protected bool IsWaitingToBePlayed { get; set; }

        public void Play()
        {
            if(IsPlaying)
                return;

            if(IsPaused)
            {
                IsWaitingToBePlayed = true;
                return;
            }
            
            IsPlaying = true;
            ShouldBePlaying = true;

            OnPlay();
        }
        public void Stop()
        {
            if(!IsPlaying)
                return;

            IsPlaying = false;
            IsPaused = false;
            ShouldBePlaying = false;
            IsWaitingToBePlayed = false;

            OnStop();
        }
        public void Pause(string pauserID)
        {
            if (!Pausers.Contains(pauserID))
                Pausers.Add(pauserID);

            if(!IsPaused)
            {
                IsPaused = true;
                IsPlaying = false;
                OnPause();
            }
        }
        public void Resume(string pauserID)
        {
            if(!Pausers.Contains(pauserID))
                return;

            Pausers.Remove(pauserID);

            if(IsPaused && Pausers.Count == 0)
            {
                IsPaused = false;

                if(IsWaitingToBePlayed)
                {
                    IsWaitingToBePlayed = false;

                    Play();
                    OnResumeIntoPlay();
                }
                else if(ShouldBePlaying)
                {
                    IsPlaying = true;
                    OnResume();
                }
            }
        }

        protected void OnPlay();
        protected void OnStop();
        protected void OnPause();
        protected void OnResume();
        /// <summary>
        /// Gets called once the playable is automatically played after resuming when a play request was fired while paused.
        /// OnPlay will still be called but OnResume will not.
        /// </summary>
        protected void OnResumeIntoPlay();
    }
}