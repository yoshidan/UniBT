using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace UniBT.Editor
{
    public class ActionNode : BehaviorTreeNode
    {
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.MenuItems().Add(new BehaviorTreeDropdownMenuAction("Change Behavior", (a) =>
            {
                var provider = new ActionSearchWindowProvider(this);
                SearchWindow.Open(new SearchWindowContext(evt.localMousePosition), provider);
            }));
        }

        protected override bool OnValidate(Stack<BehaviorTreeNode> stack) => true;

        protected override void OnCommit(Stack<BehaviorTreeNode> stack)
        {
        }

        protected override void OnClearStyle()
        {
        }
    }
}