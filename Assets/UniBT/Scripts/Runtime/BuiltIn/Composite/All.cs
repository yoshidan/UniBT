using System.Collections.Generic;
using System.Linq;

namespace UniBT
{
    public class All : Composite
    {

        private List<NodeBehavior> runningNodes;

        protected override void OnAwake()
        {
            runningNodes = new List<NodeBehavior>();
        }

        /// <summary>
        /// Update all nodes.
        /// - any running -> Running
        /// - any failed -> Failure
        /// - else -> Success
        /// </summary>
        protected override Status OnUpdate()
        {
            runningNodes.Clear();
            var anyFailed = Children.Select(c =>
            {
                var result = c.Update();
                if (result == Status.Running)
                {
                    runningNodes.Add(c);
                }
                return result;
            }).Any(r => r == Status.Failure);

            if (runningNodes.Count > 0)
            {
                return Status.Running;
            }

            if (anyFailed)
            {
                runningNodes.ForEach(e => e.Abort());
                return Status.Failure;
            }

            return Status.Success;
        }

        public override void Abort()
        {
            runningNodes.ForEach( e => e.Abort() );
            runningNodes.Clear();
        }

    }
}