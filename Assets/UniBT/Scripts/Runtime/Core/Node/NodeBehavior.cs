using System;
using UnityEngine;

namespace UniBT
{
        
    public enum Status
    {
        Success,
        Failure,
        Running
    }
   
    [Serializable]
    public abstract class NodeBehavior 
    {

#if UNITY_EDITOR
        [HideInEditorWindow]
        public Rect graphPosition = new Rect();
        
        [HideInEditorWindow]
        public string description;
        
        [HideInEditorWindow]
        [NonSerialized]
        public Action<Status> NotifyEditor;
#endif

        protected GameObject gameObject { private set; get; }

        public void Run(GameObject attachedObject)
        {
            gameObject = attachedObject;
            OnRun();
        }
        
        protected abstract void OnRun();

        public virtual void Awake() { }

        public virtual void Start() {}

        public virtual void PreUpdate() {}

        public Status Update()
        {
            var status = OnUpdate();

#if UNITY_EDITOR
            NotifyEditor?.Invoke(status);
#endif
            return status;
        }

        public virtual void PostUpdate(){}

        protected abstract Status OnUpdate();

        /// <summary>
        ///  abort running node when the condition changed.
        /// </summary>
        public virtual void Abort() {}
        
        public virtual bool CanUpdate() => true;
        
    }
}