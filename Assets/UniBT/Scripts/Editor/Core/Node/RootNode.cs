using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;

namespace UniBT.Editor
{
    public sealed class RootNode : BehaviorTreeNode
    {
        public readonly Port Child;

        public RootNode() 
        {
            SetBehavior(typeof(Root));
            title = "Root";
            Child = CreateChildPort();
            outputContainer.Add(Child);
        }

        protected override void AddParent()
        {
        }

        protected override void AddDescription()
        {
        }

        protected override void OnRestore()
        {
            (NodeBehavior as Root).UpdateEditor = ClearStyle;
        }

        protected override bool OnValidate(Stack<BehaviorTreeNode> stack)
        {
            var edges = Child.connections.ToList();
            if (edges.Count == 0)
            {
                return false;
            }
            stack.Push(edges.First().input.node as BehaviorTreeNode);
            return true;
        }
        protected override void OnCommit(Stack<BehaviorTreeNode> stack)
        {
            var edges = Child.connections.ToList();
            var child = edges.First().input.node as BehaviorTreeNode;

            var newRoot = new Root();
            newRoot.Child = child.ReplaceBehavior();
            newRoot.UpdateEditor = ClearStyle;
            NodeBehavior = newRoot;
            stack.Push(child);
        }

        public void PostCommit(BehaviorTree tree)
        {
            tree.Root = (NodeBehavior as Root); 
        }

        protected override void OnClearStyle()
        {
            var edges = Child.connections.ToList();
            var child = edges.First().input.node as BehaviorTreeNode;
            child.ClearStyle();
        }
    }
}