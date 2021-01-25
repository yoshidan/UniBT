using UnityEngine;

namespace UniBT
{
    /// <summary>
    ///  update the children in order.
    ///  update only one child per frame.
    /// </summary>
    public class Rotator : Composite
    {
        [SerializeField]
        private bool resetOnAbort;

        private int targetIndex;

        private NodeBehavior runningNode;

        protected override Status OnUpdate()
        {
            // update running node if previous status is Running.
            if (runningNode != null)
            {
                return HandleStatus(runningNode.Update(), runningNode);
            }

            var target = Children[targetIndex];
            return HandleStatus(target.Update(), target);
        }

        private void SetNext()
        {
            targetIndex++;
            if (targetIndex >= Children.Count)
            {
                targetIndex = 0;
            }
        }

        private Status HandleStatus(Status status, NodeBehavior updated)
        {
            if (status == Status.Running)
            {
                runningNode = updated;
            }
            else
            {
                runningNode = null;
                SetNext();
            }
            return status;
        }

        public override void Abort()
        {
            if (runningNode != null)
            {
                runningNode.Abort();
                runningNode = null;
            }

            if (resetOnAbort)
            {
                targetIndex = 0;
            }
        }
    }
}