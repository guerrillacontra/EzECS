using Ez.Scripts;
using UnityEditor;
using UnityEngine;

namespace Ez.Editor
{
    [CustomEditor(typeof(EzEntitySpace))]
    public class EzEntitySpaceEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var space = (EzEntitySpace) target;

            if (GUILayout.Button("Prioritise Systems"))
            {
                space.PrioritiseSystems();
            }
        }
    }
}