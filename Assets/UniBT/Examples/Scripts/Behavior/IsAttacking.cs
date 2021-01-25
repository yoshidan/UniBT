namespace UniBT.Examples.Scripts.Behavior
{
    public class IsAttacking : Conditional
    {

        private Enemy enemy;

        protected override void OnAwake()
        {
            enemy = gameObject.GetComponent<Enemy>();
        }

        protected override bool IsUpdatable()
        {
            return enemy.Attacking;
        }
    }
}