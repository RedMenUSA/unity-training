using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugSceneMenu : MonoBehaviour
{
    [Header("Menu Settings")]
    public KeyCode toggleKey = KeyCode.F2;
    public bool showMenu = true;
    private Vector2 scrollPosition;

    private void Update()
    {
        // Toggle menu visibility
        if (Input.GetKeyDown(toggleKey)) 
            showMenu = !showMenu;
    }

    private void OnGUI()
    {
        if (!showMenu) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 500), "Debug Scene Menu", GUI.skin.window);
        GUILayout.Label("Available Scenes:");

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        var sceneCount = SceneManager.sceneCountInBuildSettings;
        for (var i = 0; i < sceneCount; i++)
        {
            var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            var sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (GUILayout.Button(sceneName, GUILayout.Height(30)))
                SceneManager.LoadScene(i);
        }

        GUILayout.EndScrollView();
        GUILayout.Space(10);
        GUILayout.Label("Press F1 to toggle menu");
        GUILayout.EndArea();
    }
}