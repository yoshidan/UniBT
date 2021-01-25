using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniBT.Editor
{
    public class CompositeNode : BehaviorTreeNode
    {
        public readonly List<Port> ChildPorts = new List<Port>();

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Change Behavior", (a) =>
            {
                var provider = new CompositeSearchWindowProvider(this);
                SearchWindow.Open(new SearchWindowContext(evt.localMousePosition), provider);
            }));
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Add Child", (a) => AddChild()));
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Remove Unnecessary Children", (a) => RemoveUnnecessaryChildren()));
        }

        public CompositeNode() 
        {
            AddChild();
        }

        public void AddChild()
        {
            var child = CreateChildPort();
            ChildPorts.Add(child);
            outputContainer.Add(child);
        }

        private void RemoveUnnecessaryChildren()
        {
            var unnecessary = ChildPorts.Where(p => p.connections.ToList().Count == 0).ToList();
            unnecessary.ForEach(e =>
            {
                ChildPorts.Remove(e);
                outputContainer.Remove(e);
            });
        }

        protected override bool OnValidate(Stack<BehaviorTreeNode> stack)
        {
            if (ChildPorts.Count <= 0) return false;

            foreach (var port in ChildPorts)
            {
                var edges = port.connections.ToList();
                if (edges.Count == 0)
                {
                    style.backgroundColor = Color.red;
                    return false;
                }
                stack.Push(edges.First().input.node as BehaviorTreeNode);
            }
            style.backgroundColor = new StyleColor(StyleKeyword.Null);
            return true;
        }

        protected override void OnCommit(Stack<BehaviorTreeNode> stack)
        {
            foreach (var port in ChildPorts)
            {
                var edges = port.connections.ToList();
                var child = edges.First().input.node as BehaviorTreeNode;
                (this.NodeBehavior as Composite).AddChild(child.ReplaceBehavior());
                stack.Push(child);
            }
        }

        protected override void OnClearStyle()
        {
            foreach (var port in ChildPorts)
            {
                var edges = port.connections.ToList();
                (edges.First().input.node as BehaviorTreeNode).ClearStyle();
            }
        }
    }
}