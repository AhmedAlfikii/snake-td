using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit;

namespace TafraKit.Demos
{
    public class DynamicPoolDemo : MonoBehaviour
    {
        [System.Serializable]
        public class DynamicRigidbodyPool : DynamicPool<Rigidbody> { };

        #region Private Serialized Fields
        [SerializeField] private DynamicRigidbodyPool rigidbodyPool;

        [Space()]

        [SerializeField] private Transform spawnPoint;
        #endregion

        #region Private Fields
        private List<Rigidbody> takenUnits = new List<Rigidbody>();
        #endregion

        #region MonoBehaviour Messages
        void Awake()
        {
            rigidbodyPool.Initialize();
        }

        #endregion

        #region Public Functions
        public void TakeUnit()
        {
            Rigidbody unit = rigidbodyPool.RequestUnit();

            if (unit != null)
            {
                takenUnits.Add(unit);
                unit.position = spawnPoint.position;
            }
        }

        public void ReleaseUnit()
        {
            if (takenUnits.Count > 0)
            {
                rigidbodyPool.ReleaseUnit(takenUnits[0]);
                takenUnits.RemoveAt(0);
            }
        }
        #endregion
    }
}