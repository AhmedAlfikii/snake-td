using UnityEngine;
using TafraKit;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

namespace TafraKit
{
    [SearchMenuItem("General/Object References")]
    public class ActorObjectReferencesModule : TafraActorModule
    {
        [System.Serializable]
        public class KeyObjectPair
        {
            public string key;
            public UnityEngine.Object value;
        }

        [SerializeField] private List<KeyObjectPair> references;

        private Dictionary<string, KeyObjectPair> referenceByKey = new Dictionary<string, KeyObjectPair>();

        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            for (int i = 0; i < references.Count; i++)
            {
                var reference = references[i];

                referenceByKey.Add(reference.key, reference);
            }
        }

        public T GetObject<T>(string key) where T : UnityEngine.Object
        { 
            if (referenceByKey.TryGetValue(key, out var target))
            {
                if(target == null)
                    return default;
                else
                    return target.value as T;
            }

            return default;
        }
    }
}