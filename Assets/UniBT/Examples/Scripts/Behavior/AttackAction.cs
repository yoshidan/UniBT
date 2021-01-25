using UnityEngine;

namespace UniBT.Examples.Scripts.Behavior
{
    public class AttackAction : Action
    {
        [SerializeField]
        private int force;

        private Enemy enemy;

        private bool triggered = false;

        public override void Awake()
        {
            enemy = gameObject.GetComponent<Enemy>();
        }
        
        protected override Status OnUpdate()
        {
            if (triggered)
            {
                if (enemy.Attacking)
                {
                    return Status.Running;
                }
                triggered = false;
                return Status.Success;
            }
            enemy.Attack(force);
            triggered = true;
            return Status.Running;
        }

        public override void Abort()
        {
            enemy.CancelAttack();
            triggered = false;
        }

    }
}