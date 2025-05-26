using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.RPG
{
    public interface IStatValuesList
    {
        List<StatValue> StatValues { get; }

        public void InitializeStatValues();
    }
}