using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public abstract class SettingsModule : ScriptableObject
    {
        public virtual int Priority => 5000;
        public abstract string Name { get;}
        public abstract string Description { get;}
    }
}