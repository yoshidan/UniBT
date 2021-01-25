using UnityEditor;
using UnityEngine;

namespace UniBT.Editor
{
    [CustomEditor(typeof(BehaviorTree))]
    public class BehaviorTreeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI ();

            if (GUILayout.Button("Open Behavior Tree"))
            {
                var bt = target as BehaviorTree;
                GraphEditorWindow.Show(bt);
            }
        }
    }

}