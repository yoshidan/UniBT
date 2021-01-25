using UnityEngine;

namespace UniBT.Examples.Scripts.Behavior
{
    public class IsHateGt : Conditional
    {
        
        [SerializeField] 
        private float threshold = 10;
        
        private Enemy enemy;
        
        protected override void OnAwake()
        {
            enemy = gameObject.GetComponent<Enemy>();
        }

        protected override bool IsUpdatable()
        {
            return enemy.Hate > threshold;
        }
    }
}