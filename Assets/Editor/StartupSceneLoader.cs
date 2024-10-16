using UnityEditor;
using UnityEditor.SceneManagement;

// Script to make sure that we load the bootstrap scene first when we're testing so we don't have to remember to play from that scene every time
[InitializeOnLoad]
public static class StartupSceneLoader
{
    static StartupSceneLoader()
    {
        EditorApplication.playModeStateChanged += LoadStartupScene;
    }

    private static void LoadStartupScene(PlayModeStateChange newState)
    {
        if (newState == PlayModeStateChange.ExitingEditMode)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }
        if (newState == PlayModeStateChange.EnteredPlayMode)
        {
            if (EditorSceneManager.GetActiveScene().buildIndex != 0)
            {
                EditorSceneManager.LoadScene(0);
            }
        }
    }
}
