using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.RPG
{
    public interface IStatsContainerProvider
    {
        public StatsContainer StatsContainer { get; }
    }
}