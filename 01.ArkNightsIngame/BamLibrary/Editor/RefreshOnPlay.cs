using UnityEngine;
using UnityEditor;

[InitializeOnLoadAttribute]
public class RefreshOnPlay
{
    static RefreshOnPlay()
    {
        EditorApplication.playModeStateChanged += PlayRefresh;
    }

    private static void PlayRefresh(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode && !EditorPrefs.GetBool("kAutoRefresh"))
        {
            Debug.LogTry("스크립트 새로고침 중..");
            AssetDatabase.Refresh();
        }
    }
}
