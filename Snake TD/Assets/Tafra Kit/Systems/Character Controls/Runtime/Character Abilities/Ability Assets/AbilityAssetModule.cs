using System;
using UnityEngine;

namespace TafraKit.CharacterControls
{
    [Serializable]
    public abstract class AbilityAssetModule 
    {
        [NonSerialized] protected Ability ability;

        public abstract void LoadAsset(Ability ability);
        public abstract void ReleaseAsset();
    }
}