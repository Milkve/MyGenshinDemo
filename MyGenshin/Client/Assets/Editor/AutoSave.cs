using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.SceneManagement;
public class AutoSave : EditorWindow
{
    const string AUTO_SAVE_INTERVAL_TIME = "AutoSave_Interval_Time(sec)";
    static int interval; //实际使用的
    static int tempInterval;//用于显示的

    const string SHOW_MESSAGE = "AutoSave_ShowMessage";
    static bool isShowMessage;
    static bool tempIsShowMessage;

    const string AUTO_SAVE = "AutoSave_AutoSave";
    static bool isAutoSave;
    static bool tempIsAutoSave;
    static bool IsAutoSave
    {
        get => isAutoSave; set
        {

            if (isAutoSave != value)
            {
                EditorApplication.update -= AppUpdate;
                if (value)
                {
                    EditorApplication.update += AppUpdate;
                }
            }
            isAutoSave = value;

        }
    }

    static DateTime lastSaveTimeScene;
    static string projectPath;
    static string scenePath;

    [InitializeOnLoadMethod]
    public static void OnEditorLoad()
    {
        interval = EditorPrefs.GetInt(AUTO_SAVE_INTERVAL_TIME, 60);
        IsAutoSave = EditorPrefs.GetBool(AUTO_SAVE, false);
        isShowMessage = EditorPrefs.GetBool(SHOW_MESSAGE, true);
        lastSaveTimeScene = DateTime.Now;
        projectPath = Application.dataPath;
    }
    public void OnEnable()
    {
        tempIsAutoSave = IsAutoSave;
        tempIsShowMessage = isShowMessage;
        tempInterval = interval;
    }
    public void OnDisable()
    {
        EditorPrefs.SetInt(AUTO_SAVE_INTERVAL_TIME, interval);
        EditorPrefs.SetBool(AUTO_SAVE, IsAutoSave);
        EditorPrefs.SetBool(SHOW_MESSAGE, isShowMessage);
    }

    [MenuItem("Window/AutoSave")]
    static void Init()
    {
        GetWindow<AutoSave>("自动保存").Show();
    }

    private void OnGUI()
    {

        tempIsAutoSave = EditorGUILayout.Toggle("是否开启", tempIsAutoSave);
        tempIsShowMessage = EditorGUILayout.Toggle("显示信息", tempIsShowMessage);
        tempInterval = EditorGUILayout.IntField("保存间隔(s)", tempInterval);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("确认"))
        {
            IsAutoSave = tempIsAutoSave;
            isShowMessage = tempIsShowMessage;
            interval = tempInterval;
            Close();
        }
        if (GUILayout.Button("应用"))
        {
            IsAutoSave = tempIsAutoSave;
            isShowMessage = tempIsShowMessage;
            interval = tempInterval;
        }
        if (GUILayout.Button("取消"))
        {
            Close();
        }
        GUILayout.EndHorizontal();

    }
    static void AppUpdate()
    {
        if ((DateTime.Now - lastSaveTimeScene).TotalSeconds < interval | EditorApplication.isPlayingOrWillChangePlaymode) return;
        lastSaveTimeScene = DateTime.Now;
        scenePath = EditorSceneManager.GetActiveScene().path;
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), scenePath);
        if (isShowMessage)
        {
            Debug.Log("自动保存路径: " + scenePath + " 保存时间： " + lastSaveTimeScene);
        }
    }
}