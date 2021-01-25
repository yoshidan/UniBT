using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace UniBT.Editor
{
    public class ConditionalNode : BehaviorTreeNode
    {
        private Port childPort;

        public Port Child => childPort;

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Change Behavior", (a) =>
            {
                var provider = new ConditionalSearchWindowProvider(this);
                SearchWindow.Open(new SearchWindowContext(evt.localMousePosition), provider);
            }));
        }

        public ConditionalNode()
        {
            childPort = CreateChildPort();
            outputContainer.Add(childPort);
        }

        protected override bool OnValidate(Stack<BehaviorTreeNode> stack)
        {
            var edges = childPort.connections.ToList();
            if (edges.Count == 0)
            {
                return true;
            }
            stack.Push(edges.First().input.node as BehaviorTreeNode);
            return true;
        }

        protected override void OnCommit(Stack<BehaviorTreeNode> stack)
        {
            var edges = childPort.connections.ToList();
            if (edges.Count == 0)
            {
                (NodeBehavior as Conditional).Child = null;
                return;
            }
            var child = edges.First().input.node as BehaviorTreeNode;
            (NodeBehavior as Conditional).Child = child.ReplaceBehavior();
            stack.Push(child);
        }

        protected override void OnClearStyle()
        {
            var edges = childPort.connections.ToList();
            if (edges.Count == 0)
            {
                return;
            }
            var child = edges.First().input.node as BehaviorTreeNode;
            child.ClearStyle();
        }
    }
}