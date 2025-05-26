using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.ModularSystem
{
    public abstract class InternallyModularComponent<TInternalModule> : MonoBehaviour, ITafraPlayable where TInternalModule : InternalModule
    {
        [SerializeField] protected bool playOnEnable = true;

        protected abstract List<TInternalModule> InternalModules { get; }

        protected int modulesCount;
        private bool cachedModules;
        protected Dictionary<Type, List<TInternalModule>> modulesByType = new Dictionary<Type, List<TInternalModule>>();
        protected List<TInternalModule> allModules;
        protected List<TInternalModule> updateModules;
        protected List<TInternalModule> lateUpdateModules;
        protected List<TInternalModule> fixedUpdateModules;
        protected bool isPlaying;
        protected bool isPaused;
        protected bool shouldBePlaying;
        protected bool isWaitingToBePlayed;
        protected HashSet<string> pausers = new HashSet<string>();

        #region ITafraPlayable Properties
        public bool IsPlaying => isPlaying;
        public bool IsPaused => isPaused;
        public bool IsWaitingToBePlayed => isWaitingToBePlayed;
        public ITafraPlayable Playable => this;
        public bool CanBePlayed => true;
        bool ITafraPlayable.IsPlaying { get => isPlaying; set => isPlaying = value; }
        bool ITafraPlayable.IsPaused { get => isPaused; set => isPaused = value; }
        bool ITafraPlayable.ShouldBePlaying { get => shouldBePlaying; set => shouldBePlaying = value; }
        bool ITafraPlayable.IsWaitingToBePlayed { get => isWaitingToBePlayed; set => isWaitingToBePlayed = value; }
        HashSet<string> ITafraPlayable.Pausers => pausers;
        #endregion

        protected virtual void Awake()
        {
            CacheModules();
        }
        protected virtual void Start()
        {
            for (int i = 0; i < modulesCount; i++)
            {
                var module = allModules[i];

                if (module == null)
                    continue;

                module.OnControllerStart();
            }
        }
        protected virtual void OnEnable()
        {
            if(playOnEnable)
                Playable.Play();
        }
        protected virtual void OnDisable()
        {
            Playable.Stop();
        }
        protected virtual void Update()
        {
            if(!isPlaying)
                return;

            if (updateModules != null)
            {
                for (int i = 0; i < updateModules.Count; i++)
                {
                    updateModules[i].Update();
                }
            }
        }
        protected virtual void LateUpdate()
        {
            if(!isPlaying)
                return;

            if(lateUpdateModules != null)
            {
                for (int i = 0; i < lateUpdateModules.Count; i++)
                {
                    lateUpdateModules[i].LateUpdate();
                }
            }
        }
        protected virtual void FixedUpdate()
        {
            if(!isPlaying)
                return;

            if(fixedUpdateModules != null)
            {
                for (int i = 0; i < fixedUpdateModules.Count; i++)
                {
                    fixedUpdateModules[i].FixedUpdate();
                }
            }
        }
        protected virtual void OnDestroy()
        {
            for(int i = 0; i < modulesCount; i++)
            {
                var module = allModules[i];

                module.OnDestroy();
            }
        }
        protected virtual void OnDrawGizmos()
        {
            if(!isPlaying)
                return;

            for(int i = 0; i < modulesCount; i++)
            {
                var module = allModules[i];

                if (module == null)
                    continue;

                module.OnDrawGizmos();
            }
        }
        protected virtual void OnDrawGizmosSelected()
        {
            if(!isPlaying)
                return;

            for(int i = 0; i < modulesCount; i++)
            {
                var module = allModules[i];

                if (module == null)
                    continue;

                module.OnDrawGizmosSelected();
            }
        }

        protected void CacheModules()
        {
            if (cachedModules)
                return;

            allModules = InternalModules;

            modulesCount = allModules.Count;

            for (int i = 0; i < modulesCount; i++)
            {
                var module = allModules[i];

                if (module == null)
                    continue;

                Type type = module.GetType();
                do
                {
                    if (modulesByType.TryGetValue(type, out var existingModules))
                        existingModules.Add(module);
                    else
                        modulesByType.Add(type, new List<TInternalModule> { module });

                    type = type.BaseType;

                } while (type != typeof(TInternalModule));

                if (module.UseUpdate)
                {
                    if (updateModules == null)
                        updateModules = new List<TInternalModule>();

                    updateModules.Add(module);
                }

                if (module.UseLateUpdate)
                {
                    if (lateUpdateModules == null)
                        lateUpdateModules = new List<TInternalModule>();

                    lateUpdateModules.Add(module);
                }

                if (module.UseFixedUpdate)
                {
                    if (fixedUpdateModules == null)
                        fixedUpdateModules = new List<TInternalModule>();

                    fixedUpdateModules.Add(module);
                }
            }

            cachedModules = true;
        }

        /// <summary>
        /// Gets a list of all the modules of the given type. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<TInternalModule> GetModules<T>() where T : InternalModule
        {
            if (!cachedModules)
                CacheModules();
            
            modulesByType.TryGetValue(typeof(T), out var modules);

            return modules;
        }
        /// <summary>
        /// Gets the first module of the given type. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetModule<T>() where T : InternalModule
        {
            if (!cachedModules)
                CacheModules();
            
            modulesByType.TryGetValue(typeof(T), out var modules);

            if (modules == null)
                return default;
            
            return modules[0] as T;
        }

        #region Callbacks
        protected virtual void OnPlay() { }
        protected virtual void OnStop() { }
        protected virtual void OnPause() { }
        protected virtual void OnResume() { }
        protected virtual void OnResumeIntoPlay() { }

        void ITafraPlayable.OnPlay()
        {
            for(int i = 0; i < modulesCount; i++)
            {
                var module = allModules[i];

                if(module == null)
                    continue;

                module.Enable();
            }

            OnPlay();
        }

        void ITafraPlayable.OnStop()
        {
            for(int i = 0; i < modulesCount; i++)
            {
                var module = allModules[i];

                if(module == null)
                    continue;

                module.Disable();
            }

            OnStop();
        }

        void ITafraPlayable.OnPause()
        {
            for(int i = 0; i < modulesCount; i++)
            {
                var module = allModules[i];

                if(module == null)
                    continue;

                module.Disable();
            }

            OnPause();
        }

        void ITafraPlayable.OnResume()
        {
            for(int i = 0; i < modulesCount; i++)
            {
                var module = allModules[i];

                if(module == null)
                    continue;

                module.Enable();
            }

            OnResume();
        }

        void ITafraPlayable.OnResumeIntoPlay()
        {
            for(int i = 0; i < modulesCount; i++)
            {
                var module = allModules[i];

                if(module == null)
                    continue;

                module.Enable();
            }

            OnResumeIntoPlay();
        }
        #endregion
    }
}