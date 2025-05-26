using UnityEngine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.GraphViews
{
    public interface IGraphBlackboardContainer
    {
        GraphBlackboard Blackboard { get; }
    }
}