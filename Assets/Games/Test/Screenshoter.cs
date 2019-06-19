using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

#if UNITY_EDITOR
using System.Reflection;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
#endif

public class Screenshoter : MonoBehaviour
{
    public static Resolution[] DefaultResolutions =
    {
        new Resolution("iPhone 4", 640, 960),
        new Resolution("iPhone 5", 640, 1136),
        new Resolution("iPhone 6", 750, 1334),
        new Resolution("iPhone 8+", 1242, 2208),
        new Resolution("iPhone X", 1125, 2436),
        new Resolution("iPhone Xs Max", 1242, 2688),
        new Resolution("iPhone XR ", 828, 1792),
        new Resolution("HD", 1080, 1920),
        new Resolution("iPad Retina", 1536, 2048),
        new Resolution("iPad Pro 10.5", 1668, 2224),
        new Resolution("iPad Pro 12.9", 2048, 2732),
    };

    public bool pauseOnShoot = true;
    public bool landscape;
    public string path;

    public Resolution[] resolutions = DefaultResolutions;

    void Start()
    {
        if (resolutions == null || resolutions.Length == 0)
        {
            resolutions = DefaultResolutions;
        }

#if UNITY_EDITOR
        CreateAllViewSizes();
#endif
    }

    [ContextMenu("Apply Resolution")]
    public void ApplyNewResolution()
    {
        var oo = FindObjectsOfType<SafeArea>();
        foreach (var o in oo)
        {
            o.Apply();
        }
    }

    IEnumerator IEApplyResolution(Resolution r)
    {
#if UNITY_EDITOR
        SetViewSize(r);
#endif
        yield return null;

        ApplyNewResolution();
    }

    public void Shot()
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Select Path ");
            return;
        }

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        if (!Directory.Exists(path))
        {
            Debug.LogError("Invalid Path ");
            return;
        }

        if (Application.isPlaying)
        {
            StartCoroutine(CaptureRoutine());
        }
    }

    IEnumerator CaptureRoutine()
    {
        float s = Time.timeScale;
        if (pauseOnShoot)
        {
            Time.timeScale = 0;    
        }

        int i = 0;
        foreach (var r in resolutions)
        {
            i++;
            Debug.LogFormat("Capture {0}/{1} {2}", i, resolutions.Length, r.name);

#if UNITY_EDITOR
            SetViewSize(r);
#endif
            yield return null;
            ApplyNewResolution();
            yield return null;

            yield return null;
            Capture(r.name);

            yield return new WaitForSecondsRealtime(0.5f);
        }

#if UNITY_EDITOR
        SetViewSize(resolutions[1]);
        yield return null;
        ApplyNewResolution();
#endif

        Time.timeScale = s;
    }

    void DoCapture(Resolution resolution)
    {
        Debug.LogFormat("Capture {0}", resolution.name);

#if UNITY_EDITOR
        SetViewSize(resolution);
#endif
        ApplyNewResolution();

        Capture(resolution.name);
    }

    public void Capture(string namePrefix)
    {
        string fileName = namePrefix + " Screenshot " + DateTime.Now.ToString("yyyy-MM-dd HHmmss", System.Globalization.CultureInfo.InvariantCulture) + ".png";
        UnityEngine.ScreenCapture.CaptureScreenshot(Path.Combine(path, fileName));

        Debug.Log("Screenshot saved to " + Path.Combine(path, fileName));
    }

    [ContextMenu("Load Default Resolutions")]
    public void LoadDefaultResolutions ()
    {
        resolutions = DefaultResolutions;
    }

    [Serializable]
    public struct Resolution
    {
        public string name;
        public int width;
        public int height;

        [NonSerialized] public int widthDp;
        [NonSerialized] public int heightDp;

        public Resolution(string name, int w, int h)
        {
            this.name = name;
            width = widthDp = w;
            height = heightDp = h;
        }

        public Resolution(string name, int w, int h, int wdp, int hdp)
        {
            this.name = name;
            width = w;
            height = h;
            widthDp = wdp;
            heightDp = hdp;
        }

        public override string ToString()
        {
            return string.Format("[{0} ({1}x{2}) ({3}dp x {4}dp)]", name, width, height, widthDp, heightDp);
        }
    }

    #region View Size 
#if UNITY_EDITOR

    static object _gameViewSizesInstance;
    static object gameViewSizesInstance
    {
        get
        {
            if (_gameViewSizesInstance == null)
            {
                var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
                var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
                var instanceProp = singleType.GetProperty("instance");
                _gameViewSizesInstance = instanceProp.GetValue(null, null);
            }
            return _gameViewSizesInstance;
        }
    }

    static MethodInfo _getGroupMethodInfo;
    static MethodInfo getGroupMethodInfo
    {
        get
        {
            if (_getGroupMethodInfo == null)
            {
                var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
                _getGroupMethodInfo = sizesType.GetMethod("GetGroup");
            }

            return _getGroupMethodInfo;
        }
    }

    static object GetGroup(GameViewSizeGroupType type)
    {
        return getGroupMethodInfo.Invoke(gameViewSizesInstance, new object[] { (int)type });
    }

    public static void AddCustomViewSize(GameViewSizeGroupType sizeGroupType, GameViewSizeType viewSizeType, int width, int height, string name)
    {
        // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupTyge);
        // group.AddCustomSize(new GameViewSize(viewSizeType, width, height, text);

        var group = GetGroup(sizeGroupType);
        var addCustomSize = getGroupMethodInfo.ReturnType.GetMethod("AddCustomSize"); // or group.GetType().
        var gvsType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
        var ctor = gvsType.GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(string) });
        var newSize = ctor.Invoke(new object[] { (int)viewSizeType, width, height, name });
        addCustomSize.Invoke(group, new object[] { newSize });
    }

    public static Resolution SelectedResolution()
    {
        Resolution r = new Resolution { width = 0, height = 0 };

        int index = SelectedSizeIndex();

        var group = GetGroup(CurrentViewGroup);
        var groupType = group.GetType();
        var getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
        var getCustomCount = groupType.GetMethod("GetCustomCount");
        int sizesCount = (int)getBuiltinCount.Invoke(group, null) + (int)getCustomCount.Invoke(group, null);
        var getGameViewSize = groupType.GetMethod("GetGameViewSize");
        var gvsType = getGameViewSize.ReturnType;
        var widthProp = gvsType.GetProperty("width");
        var heightProp = gvsType.GetProperty("height");
        var textProp = gvsType.GetProperty("baseText");

        var indexValue = new object[1];
        if (index >= 0 && index < sizesCount)
        {
            indexValue[0] = index;
            var size = getGameViewSize.Invoke(group, indexValue);
            r.width = (int)widthProp.GetValue(size, null);
            r.height = (int)heightProp.GetValue(size, null);
            r.name = (string)textProp.GetValue(size, null);
        }

        return r;
    }

    public static int FindViewSize(GameViewSizeGroupType sizeGroupType, int width, int height, string name)
    {
        // goal:
        // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupType);
        // int sizesCount = group.GetBuiltinCount() + group.GetCustomCount();
        // iterate through the sizes via group.GetGameViewSize(int index)


        var group = GetGroup(sizeGroupType);
        var groupType = group.GetType();
        var getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
        var getCustomCount = groupType.GetMethod("GetCustomCount");
        int sizesCount = (int)getBuiltinCount.Invoke(group, null) + (int)getCustomCount.Invoke(group, null);
        var getGameViewSize = groupType.GetMethod("GetGameViewSize");
        var gvsType = getGameViewSize.ReturnType;
        var widthProp = gvsType.GetProperty("width");
        var heightProp = gvsType.GetProperty("height");
        var textProp = gvsType.GetProperty("baseText");

        var indexValue = new object[1];
        for (int i = 0; i < sizesCount; i++)
        {
            indexValue[0] = i;
            var size = getGameViewSize.Invoke(group, indexValue);
            int sizeWidth = (int)widthProp.GetValue(size, null);
            int sizeHeight = (int)heightProp.GetValue(size, null);
            string text = (string)textProp.GetValue(size, null);

            if (sizeWidth == width && sizeHeight == height && text.Equals(name))
                return i;
        }
        return -1;
    }

    public static void SetViewSize(int index)
    {
        var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        var selectedSizeIndexProp = gvWndType.GetProperty("selectedSizeIndex",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        var gvWnd = EditorWindow.GetWindow(gvWndType);
        selectedSizeIndexProp.SetValue(gvWnd, index, null);
    }

    public static int SelectedSizeIndex()
    {
        var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        var selectedSizeIndexProp = gvWndType.GetProperty("selectedSizeIndex",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        var gvWnd = EditorWindow.GetWindow(gvWndType);
        return (int)selectedSizeIndexProp.GetValue(gvWnd, null);
    }

    public int SelectedResolutionIndex()
    {
        var r = SelectedResolution();
        var currentResolution = Array.FindIndex(resolutions, i => i.width == r.width && i.height == r.height);

        if (currentResolution < 0 && r.height > 0)
        {
            currentResolution = Array.FindIndex(resolutions, i => Mathf.Abs((float)i.width / i.height - (float)r.width / r.height) < 0.01f);
        }

        return currentResolution;
    }

    public static GameViewSizeGroupType CurrentViewGroup
    {
        get
        {
#if UNITY_IPHONE
            return GameViewSizeGroupType.iOS;
#endif

#if UNITY_ANDROID
            return GameViewSizeGroupType.Android;
#endif
            return GameViewSizeGroupType.Standalone;
        }
    }

    public void CreateAllViewSizes()
    {
        foreach (var i in resolutions)
        {
            var ii = new Resolution(!landscape ? i.name : i.name + " Landscape",
                                    !landscape ? i.width : i.height,
                                    !landscape ? i.height : i.width);
            int exists = FindViewSize(CurrentViewGroup, ii.width, ii.height, ii.name);
            if (exists < 0)
            {
                Debug.LogFormat("Create new view size {0} ({0}, {1})", ii.name, ii.width, ii.height);
                AddCustomViewSize(CurrentViewGroup,
                                  GameViewSizeType.FixedResolution,
                                  ii.width,
                                  ii.height,
                                  ii.name
                                 );
            }
        }
    }

    public void SetViewSize(Resolution i)
    {
        var ii = new Resolution(!landscape ? i.name : i.name + " Landscape",
                                !landscape ? i.width : i.height,
                                !landscape ? i.height : i.width);

        int index = FindViewSize(CurrentViewGroup, ii.width, ii.height, ii.name);
        if (index >= 0)
        {
            SetViewSize(index);
        }
        else
        {
            Debug.LogErrorFormat("No view size found ({0}, {1})", ii.width, ii.height);
        }

    }

    public void ApplyResolution(int i)
    {
        i = Mathf.Clamp(i, 0, resolutions.Length - 1);
        StartCoroutine(IEApplyResolution(resolutions[i]));
    }

    public enum GameViewSizeType
    {
        AspectRatio, FixedResolution
    }

#endif
    #endregion

}

#if UNITY_EDITOR
[CustomEditor(typeof(Screenshoter))]
public class ScreenshoterEditor : Editor
{
    ReorderableList list;
    int currentResolution;
    Screenshoter t;

    bool GamePaused 
    {
        get { return Math.Abs(Time.timeScale) < 0.0001f; }
    }

    float? lastTimeScale;

    void PauseGame ()
    {
        lastTimeScale = Time.timeScale;
        Time.timeScale = 0;
    }

    void ResumeGame ()
    {
        if (lastTimeScale != null)
        {
            Time.timeScale = lastTimeScale.Value;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    private void OnEnable()
    {
        lastTimeScale = null;
        t = target as Screenshoter;

        list = new ReorderableList(serializedObject, serializedObject.FindProperty("resolutions"), false, false, true, true);
        list.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            DrawElement(list.serializedProperty.GetArrayElementAtIndex(index), rect, index, isActive, isFocused);
        };
        currentResolution = list.index = t.SelectedResolutionIndex();
    }

    void DrawElement(SerializedProperty property, Rect position, int index, bool active, bool focused)
    {
        //EditorGUI.BeginProperty(position, null, property);
        float left = 5;
        float buttonWidth = 20;
        position.height -= 2;

        var buttonRect = new Rect(position.x + left, position.y, buttonWidth, position.height);
        var nameRect = new Rect(buttonRect.x + buttonRect.width, position.y, position.width / 4, position.height);
        var widthRect = new Rect(nameRect.x + nameRect.width, position.y, (position.width - nameRect.width - left - buttonWidth) / 2, position.height);
        var heightRect = new Rect(widthRect.x + widthRect.width, position.y, widthRect.width, position.height);

        if (GUI.Button(buttonRect, ">"))
        {

        }

        EditorGUIUtility.labelWidth = 60f;

        EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), GUIContent.none);
        EditorGUI.PropertyField(widthRect, property.FindPropertyRelative("width"), new GUIContent("Width"));
        EditorGUI.PropertyField(heightRect, property.FindPropertyRelative("height"), new GUIContent("Height"));


        //EditorGUI.EndProperty();
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        {
            t.path = EditorGUILayout.TextField("Save to", t.path);
            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                string path = EditorUtility.OpenFolderPanel("Select Folder", t.path ?? Application.dataPath, "");
                if (!string.IsNullOrEmpty(path))
                {
                    t.path = path;
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            t.pauseOnShoot = EditorGUILayout.Toggle("Pause on Shoot", t.pauseOnShoot);
            if (Application.isPlaying)
            {
                if (GUILayout.Button(GamePaused ? "Unpause" : "Pause"))
                {
                    if (GamePaused)
                    {
                        ResumeGame();
                    }
                    else
                    {
                        PauseGame();
                    }
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        t.landscape = EditorGUILayout.Toggle("Landscape", t.landscape);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("resolutions"), true);
        //list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();

        if (Application.isPlaying)
        {
            EditorGUILayout.BeginHorizontal();
            {
                float w = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 80f;
                EditorGUILayout.LabelField("Switch resolution");
                EditorGUIUtility.labelWidth = w;
                if (GUILayout.Button("<<<"))
                {
                    currentResolution = Mathf.Clamp(currentResolution - 1, 0, t.resolutions.Length - 1);
                    t.ApplyResolution(currentResolution);
                }

                if (GUILayout.Button(">>>"))
                {
                    currentResolution = Mathf.Clamp(currentResolution + 1, 0, t.resolutions.Length - 1);
                    t.ApplyResolution(currentResolution);
                }
            }
            EditorGUILayout.EndHorizontal();
        }


        if (Application.isPlaying && GUILayout.Button("Shoot All Resolutions"))
        {
            t.Shot();
        }

        if (GUILayout.Button("Shoot One"))
        {
            t.Capture("");
        }
    }

}

[CustomPropertyDrawer(typeof(Screenshoter.Resolution))]
public class ResolutionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var nameRect = new Rect(position.x, position.y, position.width / 3, position.height);
        var widthRect = new Rect(position.x + nameRect.width, position.y, position.width / 3, position.height);
        var heightRect = new Rect(position.x + nameRect.width + widthRect.width, position.y, position.width / 3, position.height);

        EditorGUIUtility.labelWidth = 60f;

        EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), GUIContent.none);
        EditorGUI.PropertyField(widthRect, property.FindPropertyRelative("width"), new GUIContent("Width"));
        EditorGUI.PropertyField(heightRect, property.FindPropertyRelative("height"), new GUIContent("Height"));

        EditorGUI.EndProperty();
    }
}
#endif
