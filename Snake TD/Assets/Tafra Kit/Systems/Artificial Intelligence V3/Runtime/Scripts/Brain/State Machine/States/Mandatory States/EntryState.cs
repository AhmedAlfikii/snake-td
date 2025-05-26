using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;

namespace TafraKit.Internal.AI3
{
    [RemoveFromSearchMenu, System.Serializable]
    public class EntryState : MandatoryState
    {
        protected override void OnPlay()
        {
            Complete();
        }
    }
}
