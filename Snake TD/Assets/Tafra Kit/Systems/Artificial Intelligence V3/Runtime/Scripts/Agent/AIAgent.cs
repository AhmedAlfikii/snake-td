using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.Internal.AI3;
using TafraKit.GraphViews;
using System;
using TafraKit.ContentManagement;

namespace TafraKit.AI3
{
    public class AIAgent : TafraActor
    {
        [Tooltip("The brain this agent would use to behave.")]
        [SerializeField] private Brain brain;
        [SerializeField] private ExposedBrainBlackboard exposedBlackboard;
        [Tooltip("The group of external blackboards that the brain nodes would need access to.")]
        [SerializeField] private List<TafraAsset<ExternalBlackboard>> externalBlackboards;

        private Brain activeBrain;
        private BrainOperator brainOperator;
        private BlackboardCollection blackboardCollection;
        private bool canPlayOnEnable;

        public Brain Brain
        {
            get 
            { 
                if (Application.isPlaying)
                    return activeBrain;
                else
                    return brain;
            }
            set 
            {
                if (value == brain)
                    return;

                AssignBrain(value); 
            }
        }
        public BlackboardCollection BlackboardCollection => blackboardCollection;
        public ExposedBrainBlackboard ExposedBlackboard => exposedBlackboard;

        protected override void Awake()
        {
            if(brain == null)
                return;

            base.Awake();

            exposedBlackboard.Initialize(null);

            blackboardCollection = new BlackboardCollection();
            for(int i = 0; i < externalBlackboards.Count; i++)
            {
                blackboardCollection.AddExternalBlackboard(externalBlackboards[i].Load().Blackboard);
            }
            AssignBrain(brain);
        }
        protected override void Start()
        {
            base.Start();

            if(playOnEnable)
                Playable.Play();

            canPlayOnEnable = true;
        }
        protected override void OnEnable()
        {
            base.OnEnable();

            //"canPlayOnEnable" is here because in case this enemy was already present in the scene on awake...
            //...then don't enable it here because we want to give external blackboard a chance to find the player.
            if(playOnEnable && canPlayOnEnable)
                Playable.Play();
        }
        protected override void OnDisable()
        {
            base.OnDisable();

            if(isPlaying)
                StopBrain();
        }
        protected override void Update()
        {
            base.Update();
           
            if(isPlaying && brainOperator != null)
                brainOperator.Update();
        }
        protected override void LateUpdate()
        {
            base.LateUpdate();
          
            if(isPlaying && brainOperator != null)
                brainOperator.LateUpdate();
        }
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
         
            if(isPlaying && brainOperator != null)
                brainOperator.FixedUpdate();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            for(int i = 0; i < externalBlackboards.Count; i++)
            {
                externalBlackboards[i].Release();
            }

            Destroy(activeBrain);
        }

        #if UNITY_EDITOR
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if(brainOperator != null)
                brainOperator.OnDrawGizmos();
        }
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            if(brainOperator != null)
                brainOperator.OnDrawGizmosSelected();
        }
        #endif

        private void AssignBrain(Brain brain)
        {
            if(brain == null)
            {
                if(brainOperator != null)
                {
                    //Send a null brain to the operator so that it cleans itself up.
                    brainOperator.SwitchBrain(null);

                    brainOperator = null;
                }

                Destroy(activeBrain);
                activeBrain = null;

                return;
            }

            activeBrain = brain.Clone();

            activeBrain.Blackboard.Initialize(exposedBlackboard);
            blackboardCollection.SetDependencies(this);
            blackboardCollection.SetInternalBlackboard(activeBrain.Blackboard);

            if(brainOperator == null)
            {
                brainOperator = new BrainOperator(activeBrain, this);
                brainOperator.Initialize();
            }
            else
                brainOperator.SwitchBrain(activeBrain);
        }

        public void Play()
        {
            Playable.Play();
        }
        public void Stop()
        {
            Playable.Stop();
        }
        public void Pause(string pauserID)
        {
            Playable.Pause(pauserID);
        }
        public void Resume(string pauserID)
        {
            Playable.Resume(pauserID);
        }
        /// <summary>
        /// Meant to be called during edit-time to sync the exposed properties with the assigned brain's blackboard.
        /// </summary>
        public bool RefreshExposedProperties()
        {
            if(brain == null)
            {
                exposedBlackboard.RemoveAllProperties();
                return false;
            }

            return BlackboardUtilities.RefreshExposedProperties(brain.Blackboard, exposedBlackboard);
        }
        private void PlayBrain()
        {
            if(brainOperator != null)
                brainOperator.RaisePlayFlag();
        }
        private void StopBrain()
        {
            if(brainOperator != null)
                brainOperator.RaiseStopFlag();
        }
        protected override void OnPlay()
        {
            base.OnPlay();
            PlayBrain();
        }

        protected override void OnStop()
        {
            base.OnStop();
            StopBrain();
        }

        protected override void OnPause()
        {
            base.OnPause();
            StopBrain();
        }

        protected override void OnResume()
        {
            base.OnResume();
            PlayBrain();
        }
    }
}
