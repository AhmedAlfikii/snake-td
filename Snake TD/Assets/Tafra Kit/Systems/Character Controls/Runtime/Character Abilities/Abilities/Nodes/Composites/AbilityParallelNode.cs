using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;
using System;
using TafraKit.CharacterControls;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Composites/Parallel"), GraphNodeName("Parallel", "Parallel")]
    public class ParallelNode : AbilityCompositeNode
    {
        [Tooltip("If enabled, the rest of the nodes will only run if the first node is running. And the first nod will control the state of the parallel node.")]
        [SerializeField] private bool firstNodeIsController;
        [Tooltip("If a node finished running, should we run it again or ignore it? If false, this parallel node will complete if no more child nodes are running.")]
        [SerializeField] private bool restartCompletedNodes = false;

        [NonSerialized] private List<BTNode> activeChildren = new List<BTNode>();

        public ParallelNode(ParallelNode otherParallelNode) : base(otherParallelNode)
        {
            firstNodeIsController = otherParallelNode.firstNodeIsController;
            restartCompletedNodes = otherParallelNode.restartCompletedNodes;
        }
        public ParallelNode()
        {

        }

        protected override void OnStart()
        {
            activeChildren.Clear();

            for(int i = 0; i < children.Count; i++)
            {
                activeChildren.Add(children[i]);
            }
        }
        protected override BTNodeState OnUpdate()
        {
            int childrenCount = activeChildren.Count;

            if(childrenCount == 0)
                return BTNodeState.Success;

            if(!firstNodeIsController)
            {
                int sucesses = 0;
                int failures = 0;

                for(int i = 0; i < childrenCount; i++)
                {
                    //This could happen if the node got reset while iterating through its children, in this case, we should not update the rest of them.
                    if(!isStarted)
                        break;

                    BTNodeState childState = activeChildren[i].Update();

                    if(childState == BTNodeState.Success)
                        sucesses++;
                    else if(childState == BTNodeState.Failure)
                        failures++;

                    if(!restartCompletedNodes && childState != BTNodeState.Running)
                    {
                        activeChildren.RemoveAt(i);
                        i--;
                        childrenCount--;
                    }

                }

                if(sucesses == childrenCount)
                    return BTNodeState.Success;
                else if(failures > 0)
                {
                    //If some of the children are still updating, reset them.
                    for(int i = 0; i < childrenCount; i++)
                    {
                        var child = activeChildren[i];

                        if(child.IsStarted)
                            child.Reset();
                    }

                    return BTNodeState.Failure;
                }
            }
            else
            {
                BTNodeState mainNodeState = activeChildren[0].Update();

                if(mainNodeState != BTNodeState.Running)
                {
                    if(mainNodeState == BTNodeState.Success)
                    {
                        for(int i = 1; i < childrenCount; i++)
                        {
                            //This could happen if the node got reset while iterating through its children, in this case, we should not update the rest of them.
                            if(!isStarted)
                                break;

                            activeChildren[i].Update();
                        }
                    }

                    for(int i = 1; i < childrenCount; i++)
                    {
                        activeChildren[i].Reset();
                    }

                    return mainNodeState;
                }
                //If the main node is still running.
                else
                {
                    for(int i = 1; i < childrenCount; i++)
                    {
                        //This could happen if the node got reset while iterating through its children, in this case, we should not update the rest of them.
                        if(!isStarted)
                            break;

                        BTNodeState childState = activeChildren[i].Update();

                        if(!restartCompletedNodes && childState != BTNodeState.Running)
                        {
                            activeChildren.RemoveAt(i);
                            i--;
                            childrenCount--;
                        }
                    }
                }
            }

            if(childrenCount == 0)
                return BTNodeState.Success;
            else
                return BTNodeState.Running;
        }
        protected override BTNode CloneContent()
        {
            ParallelNode clonedNode = new ParallelNode(this);

            return clonedNode;
        }
    }
}