using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Consumables
{
    public class BarFetcherObject : MonoBehaviour
    {
        [SerializeField] private Consumable[] consumablesToFetch;
        [Tooltip("The id that will be used to fetch and abandon the bars. If empty, the game object's name will be used.")]
        [SerializeField] private string fetcherId;

        [Space()]

        [SerializeField] private bool fetchOnEnable = true;
        [SerializeField] private bool abandonOnDisable = true;

        private string id;

        private void Awake()
        {
            if(!string.IsNullOrEmpty(fetcherId))
                id = fetcherId;
            else
                id = gameObject.name;
        }
        private void OnEnable()
        {
            if(fetchOnEnable)
                Fetch();
        }
        private void OnDisable()
        {
            if(abandonOnDisable)
                Abandon();
        }

        public void Fetch()
        {
            for(int i = 0; i < consumablesToFetch.Length; i++)
            {
                ConsumablesBarFetcher.Fetch(consumablesToFetch[i], id);
            }
        }
        public void Abandon()
        {
            for(int i = 0; i < consumablesToFetch.Length; i++)
            {
                ConsumablesBarFetcher.Abandon(consumablesToFetch[i], id);
            }
        }
    }
}
