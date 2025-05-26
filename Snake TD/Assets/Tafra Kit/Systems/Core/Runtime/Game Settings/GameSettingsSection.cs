using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Internal
{
    public abstract class GameSettingsSection : MonoBehaviour
    {
        public abstract bool AreConditionsSatisfied();
    }
}