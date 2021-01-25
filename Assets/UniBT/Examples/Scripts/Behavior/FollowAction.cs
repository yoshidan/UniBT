using UnityEngine;
using UnityEngine.AI;

namespace UniBT.Examples.Scripts.Behavior
{
    public class FollowAction : Action
    {

        private static readonly int Walking = Animator.StringToHash("Walking");
        
        [SerializeField] 
        private Transform target;

        [SerializeField] 
        private float speed;

        [SerializeField] 
        private float stoppingDistance;

        private Animator animator;

        private NavMeshAgent navMeshAgent;

        public override void Awake()
        {
            navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
            animator = gameObject.GetComponent<Animator>();
        }

        protected override Status OnUpdate()
        {
            navMeshAgent.enabled = true;
            navMeshAgent.isStopped = false;
            navMeshAgent.speed = speed;
            navMeshAgent.stoppingDistance = stoppingDistance;
            navMeshAgent.SetDestination(target.position);
            if (IsDone)
            {
                SetWalking(false);
                return Status.Success;
            }

            SetWalking(true);
            return Status.Running;
        }

        public override void Abort()
        {
            SetWalking(false);
        }

        private void SetWalking(bool walking)
        {
            if (animator != null)
            {
                animator.SetBool(Walking, walking);
            }

            navMeshAgent.isStopped = !walking;
        }

        private bool IsDone => !navMeshAgent.pathPending &&
                               (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance ||
                                Mathf.Approximately(navMeshAgent.remainingDistance, navMeshAgent.stoppingDistance));
    }
}