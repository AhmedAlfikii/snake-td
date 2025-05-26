using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;

namespace TafraKit.Internal.AI3
{
    [RemoveFromSearchMenu]
    public class ExitState : MandatoryState
    {
        protected override void OnPlay()
        {
            Complete();
        }
    }
}
