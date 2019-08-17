using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;
using Gemmob.Common;

namespace Gemmob.EditorTools {
    [CustomEditor(typeof(AutoAssignFields))]
    public class AutoAssignFieldsEditor : Editor {
        private MonoBehaviour[] components;
        private string[] options;
        public bool showAssigned;

        void OnEnable() {
            components = ((AutoAssignFields)target).GetComponents<MonoBehaviour>();
            options = components.Select(i => i.GetType().ToString()).ToArray();
        }

        public override void OnInspectorGUI() {
            AutoAssignFields cb = (AutoAssignFields)target;
            int current = System.Array.IndexOf(components, cb.component);

            int selected = EditorGUILayout.Popup("Target Component", current, options);
            if (selected != current && selected >= 0) {
                cb.component = System.Array.Find(components, (MonoBehaviour m) => m.GetType().ToString().Equals(options[selected]));
            }

            if (GUILayout.Button("Assign")) {
                cb.Assign();
            }

            EditorUtils.HorizontalLine();
            showAssigned = EditorGUILayout.Foldout(showAssigned, "Auto Assigned Values", true);
            if (showAssigned)
                ShowAssignments();

            serializedObject.ApplyModifiedProperties();
        }

        private void ShowAssignments() {
            AutoAssignFields cb = (AutoAssignFields)target;
            if (cb.component == null)
                return;

            var fields = cb.component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            foreach (var info in fields) {
                var attributes = info.GetCustomAttributes(typeof(AutoAssignAttribute), false);
                if (attributes.Length > 0) {
                    var value = info.GetValue(cb.component) as UnityEngine.Object;
                    EditorGUILayout.ObjectField(info.Name, value, info.FieldType, false);
                }
            }

            var properties = cb.component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            foreach (var info in properties) {
                var attributes = info.GetCustomAttributes(typeof(AutoAssignAttribute), false);
                if (attributes.Length > 0) {
                    var value = info.GetValue(cb.component, null) as UnityEngine.Object;
                    EditorGUILayout.ObjectField(info.Name, value, info.PropertyType, false);
                }
            }
        }
    }
}