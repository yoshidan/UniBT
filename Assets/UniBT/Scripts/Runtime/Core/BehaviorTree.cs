using UnityEngine;

namespace UniBT
{
    public enum UpdateType
    {
       Auto,
       Manual
    }
    public class BehaviorTree : MonoBehaviour
    {
        
        [HideInInspector]
        [SerializeReference]
        private Root root = new Root();

        [SerializeField]
        private UpdateType updateType;

        public Root Root
        {
            get => root;
#if UNITY_EDITOR
            set => root = value;
#endif
        }
        
        private void Awake() {
            root.Run(gameObject);
            root.Awake();
        }

        private void Start()
        {
            root.Start();
        }

        private void Update()
        {
            if (updateType == UpdateType.Auto) Tick();
        }
        
        public void Tick()
        {
            root.PreUpdate();
            root.Update();
            root.PostUpdate();
        }

    }
}