using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;

namespace TafraKit.Internal.AI3
{
    public interface IStateMachine
    {
        public List<State> States { get; }
        public AnyState AnyState { get; }
        public EntryState EntryState { get; }
        public ExitState ExitState { get; }
        public State GetState(string stateGUID);
    }
}
