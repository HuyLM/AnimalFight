#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Gemmob.EditorTools {
    public static class EditorUtils {
        #region Inspector
        public static void HorizontalLine(float height = 2) {
            GUIStyle styleHR = new GUIStyle(GUI.skin.box);
            styleHR.stretchWidth = true;
            styleHR.fixedHeight = height;
            GUILayout.Box("", styleHR);
        }

        public static void VerticalLine(float width = 2) {
            GUIStyle styleHR = new GUIStyle(GUI.skin.horizontalScrollbar);
            styleHR.stretchHeight = true;
            styleHR.fixedWidth = width;
            GUILayout.Box("", styleHR);
        }

        public static void BeginHorizontal(System.Action draw, string labelName = "", int labelWidth = -1) {
            if (draw == null) return;
            GUILayout.BeginHorizontal();
            {
                if (!string.IsNullOrEmpty(labelName)) {
                    if (labelWidth > 0) GUILayout.Label(labelName, GUILayout.Width(labelWidth));
                    else GUILayout.Label(labelName);
                }
                draw.Invoke();
                GUILayout.EndHorizontal();
            }
        }

        public static void BeginVertical(System.Action draw, string labelName = "", int labelWidth = -1) {
            if (draw == null) return;
            GUILayout.BeginVertical();
            {
                if (!string.IsNullOrEmpty(labelName)) {
                    if (labelWidth > 0) GUILayout.Label(labelName, GUILayout.Width(labelWidth));
                    else GUILayout.Label(labelName);
                }
                draw.Invoke();
                GUILayout.EndVertical();
            }
        }

        public static void BeginScrollView(ref Vector3 scrollPos, System.Action draw) {
            if (draw == null) return;
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            {
                draw.Invoke();
                GUILayout.EndScrollView();
            }
        }

        public static void BeginHorizontal(this EditorWindow editor, System.Action draw) {
            BeginHorizontal(draw);
        }

        public static void BeginHorizontal(this EditorWindow editor, string labelName, System.Action draw) {
            BeginHorizontal(draw, labelName);
        }

        public static void BeginHorizontal(this EditorWindow editor, string labelName, int labelWidth, System.Action draw) {
            BeginHorizontal(draw, labelName, labelWidth);
        }

        public static void BeginVertical(this EditorWindow editor, System.Action draw) {
            BeginVertical(draw);
        }

        public static void BeginVertical(this EditorWindow editor, string labelName, System.Action draw) {
            BeginVertical(draw, labelName);
        }

        public static void BeginVertical(this EditorWindow editor, string labelName, int labelWidth, System.Action draw) {
            BeginVertical(draw, labelName, labelWidth);
        }

        public static void BeginScrollView(this EditorWindow editor, ref Vector3 scrollPos, System.Action draw) {
            BeginScrollView(ref scrollPos, draw);
        }

        public static void DrawProperty<T>(this T obj, string label, float labelWidth = 120) where T : Object {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(120));
            obj = (T)EditorGUILayout.ObjectField(obj, typeof(T), true);
            EditorGUILayout.EndHorizontal();
        }

        public static void DrawField<T>(this T obj, string label, float labelWidth = 120) where T : Object {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(120));
            obj = (T)EditorGUILayout.ObjectField(obj, typeof(T), true);
            EditorGUILayout.EndHorizontal();
        }

        public static void DrawField(string label, MonoBehaviour script, System.Action action) {
            EditorGUILayout.BeginHorizontal();
            //var result = (System.Action)
            EditorGUILayout.LabelField(label);
            script = (MonoBehaviour)EditorGUILayout.ObjectField(script, typeof(MonoBehaviour), true);
            if (script != null) {
            }
            EditorGUILayout.EndHorizontal();
            //return result;
        }

        public static object DrawField(string label, object obj, float labelWidth = 120) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(120));

            object result = obj;
            if (obj is Color) {
                result = EditorGUILayout.ColorField((Color)obj);
            }
            else if (obj is bool) {
                result = EditorGUILayout.Toggle((bool)obj);
            }
            else if (obj is int) {
                result = EditorGUILayout.IntField((int)obj);
            }
            else if (obj is float) {
                result = EditorGUILayout.FloatField((float)obj);
            }
            else if (obj is System.Enum) {
                System.Enum o = (System.Enum)obj;
                result = EditorGUILayout.EnumPopup(o);
            }
            else {
                result = EditorGUILayout.PropertyField((SerializedProperty)obj);
            }

            EditorGUILayout.EndHorizontal();
            return result;
        }
        #endregion
    }

    #region Window
    public static class EditorWindowExtension {
        public static void ShowNotify(this EditorWindow window, string content) {
            window.ShowNotification(new GUIContent { text = content });
        }
    }
    #endregion

    #region Attribute 

    #endregion
}
#endif