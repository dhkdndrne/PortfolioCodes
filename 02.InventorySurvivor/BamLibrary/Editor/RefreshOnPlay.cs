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
            Debug.Log("Refresh on play..");
            AssetDatabase.Refresh();
        }
    }
}
