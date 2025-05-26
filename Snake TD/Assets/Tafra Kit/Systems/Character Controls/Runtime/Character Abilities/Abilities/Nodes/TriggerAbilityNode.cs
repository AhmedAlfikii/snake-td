using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.GraphViews;
using TafraKit.Internal.GraphViews;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    public abstract class TriggerAbilityNode : AbilityNode
    {
        public class NodesBranch
        {
            public AbilityNode branchRoot;
            public SecondaryBlackboard branchBlackboard;
            public int id;
        }

        [SerializeField] protected bool restartOnRetrigger;

        /// <summary>
        /// The child that will be used to clone new branches out of (will never be played).
        /// </summary>
        [NonSerialized] protected AbilityNode child;
        /// <summary>
        /// The current pool of cloned branches that are available for use.
        /// </summary>
        [NonSerialized] private List<NodesBranch> branchesPool = new List<NodesBranch>();
        /// <summary>
        /// The branches that are taken from the pool and are currnetly active (will be put back into the pool once finished).
        /// </summary>
        [NonSerialized] private List<NodesBranch> takenBranches = new List<NodesBranch>();
        [NonSerialized] private bool hasChildren;
        [NonSerialized] private List<IEnumerator> runningBranchesEnums = new List<IEnumerator>();
        [NonSerialized] private SecondaryBlackboard originalBranchBlackboard;
        [NonSerialized] private IEnumerator originalBranchIEnumerator;

        public override bool HasInputPort => true;
        public override bool HasOutputPort => true;
        public override bool MultiInputs => false;
        public override bool MultiOutputs => false;
        public abstract bool UseTriggerBlackboard { get; }

        protected override void OnInitialize()
        {
            hasChildren = children.Count > 0;

            if(!hasChildren)
                return;

            child = children[0] as AbilityNode;
        }
        public override void OnLateInitialize()
        {
            if (!hasChildren)
                return;

            //Creating the branches on late initialize to make sure that all the child nodes got a chance to initialize first, since we don't re-initialize the nodes whan a branch is created.
            branchesPool.Add(CreateNewBranch());
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Running;
        }
        protected override void OnEnd()
        {
            for (int i = 0; i < runningBranchesEnums.Count; i++)
            {
                characterAbilities.StopCoroutine(runningBranchesEnums[i]);
            }
            runningBranchesEnums.Clear();

            for (int i = 0; i < takenBranches.Count; i++)
            {
                var branch = takenBranches[i];
                branch.branchRoot.Reset();
                branchesPool.Add(branch);
            }
            takenBranches.Clear();
        }
        protected void TriggerInvoked(Action<SecondaryBlackboard> onShouldFillTriggerBlackboard = null)
        {
            if(!hasChildren)
                return;

            if(restartOnRetrigger)
            {
                if(originalBranchBlackboard == null)
                {
                    originalBranchBlackboard = new SecondaryBlackboard();
                    child.SetTriggerBlackboard(originalBranchBlackboard);
                }

                onShouldFillTriggerBlackboard?.Invoke(originalBranchBlackboard);

                if(originalBranchIEnumerator != null)
                {
                    characterAbilities.StopCoroutine(originalBranchIEnumerator);
                    child.Reset();
                }

                originalBranchIEnumerator = RunOriginalBranch(null);

                characterAbilities.StartCoroutine(originalBranchIEnumerator);
            }
            else
            {
                NodesBranch branch;

                int poolCount = branchesPool.Count;

                if(poolCount > 0)
                {
                    branch = branchesPool[poolCount - 1];
                    branchesPool.RemoveAt(poolCount - 1);
                    takenBranches.Add(branch);
                }
                else
                {
                    branch = CreateNewBranch();
                    takenBranches.Add(branch);
                }

                onShouldFillTriggerBlackboard?.Invoke(branch.branchBlackboard);

                IEnumerator enumerator = null;
                enumerator = RunClonedBranch(branch, () => {
                    runningBranchesEnums.Remove(enumerator);
                });
                runningBranchesEnums.Add(enumerator);

                characterAbilities.StartCoroutine(enumerator);
            }
        }
        private NodesBranch CreateNewBranch()
        {
            NodesBranch newBranch = new NodesBranch();
            AbilityNode branchRoot = child.CloneNode() as AbilityNode;

            newBranch.branchRoot = branchRoot;
            if(UseTriggerBlackboard)
            {
                SecondaryBlackboard brachBlackboard = new SecondaryBlackboard();
                newBranch.branchBlackboard = brachBlackboard;

                branchRoot.SetTriggerBlackboard(brachBlackboard);
            }

            return newBranch;
        }
        private IEnumerator RunClonedBranch(NodesBranch branch, Action onEnd)
        {
            AbilityNode rootNode = branch.branchRoot;

            BTNodeState state = BTNodeState.Running;
            while(state == BTNodeState.Running)
            {
                state = rootNode.Update();
                yield return null;
            }

            rootNode.Reset();
            takenBranches.Remove(branch);
            branchesPool.Add(branch);

            onEnd?.Invoke();
        }
        private IEnumerator RunOriginalBranch(Action onEnd)
        {
            AbilityNode rootNode = child;

            BTNodeState state = BTNodeState.Running;
            while(state == BTNodeState.Running)
            {
                state = rootNode.Update();
                yield return null;
            }

            rootNode.Reset();

            onEnd?.Invoke();
            originalBranchIEnumerator = null;
        }
    }
}