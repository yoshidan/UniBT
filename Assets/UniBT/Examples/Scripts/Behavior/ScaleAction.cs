using UnityEngine;

namespace UniBT.Examples.Scripts.Behavior
{
    public class ScaleAction: Action
    {
        
        private Transform transform;

        private float initialScale = 0;
        
        public override void Awake()
        {
            transform = gameObject.transform;
            initialScale = transform.localScale.x;
        }

        protected override Status OnUpdate()
        {
            var curScale = transform.localScale;
            curScale.x = 1.0f + 0.5f * Mathf.Sin(Time.realtimeSinceStartup);
            transform.localScale = curScale;
            return Status.Running;
        }

        public override void Abort()
        {
            var scale = transform.localScale;
            scale.x = initialScale;
            transform.localScale = scale;
        }
    }
}