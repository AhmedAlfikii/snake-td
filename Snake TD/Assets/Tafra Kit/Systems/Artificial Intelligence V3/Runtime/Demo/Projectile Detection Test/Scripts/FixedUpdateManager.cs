using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.AI3.Demo
{
    public class FixedUpdateManager : MonoBehaviour
    {
        [SerializeField] private FixedUpdateReceiver[] recievers;

        void FixedUpdate()
        {
            for (int i = 0; i < recievers.Length; i++)
            {
                recievers[i].FixedTick();
            }
        }
    }
}