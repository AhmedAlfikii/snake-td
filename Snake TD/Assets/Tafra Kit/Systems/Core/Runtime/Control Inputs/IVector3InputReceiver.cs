using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public interface IVector3InputReceiver
    {
        void RecieveInput(Vector3 input);
    }
}