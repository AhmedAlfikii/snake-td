using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public interface ITimer
    {
        public string ID { get; }
        public double CurrentTime { get; }
        public bool IsCountingDown { get; }
    }
}