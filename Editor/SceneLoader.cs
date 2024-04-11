using System;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class SceneLoader : EditorWindow
{
    private string[] scenePaths= Array.Empty<string>();
    private List<string> favoriteScenes= new List<string>();
    private Vector2 scrollPosition;

    private GUIStyle favoritedStarStyle;
    private GUIStyle unfavoritedStarStyle;
    private GUIStyle playButtonStyle;

    [MenuItem("Partisan Programmer/Scene Loader")]
    public static void ShowWindow()
    {
        GetWindow<SceneLoader>("Scene Loader");
    }

    private void OnEnable()
    {
        favoriteScenes = new List<string>();
        favoritedStarStyle = new GUIStyle();
        unfavoritedStarStyle = new GUIStyle();
        playButtonStyle = new GUIStyle();
        RefreshEditor();
        LoadFavoriteScenes();
    }

    private void InitializeStyles()
    {
        favoritedStarStyle = new GUIStyle(GUI.skin.button);
        favoritedStarStyle.normal.textColor = Color.yellow; 

        unfavoritedStarStyle = new GUIStyle(GUI.skin.button);

        playButtonStyle = new GUIStyle(GUI.skin.button);
        playButtonStyle.fontStyle = FontStyle.Bold; 
    }

    private void OnGUI()
    {
        GUILayout.Label("Select a scene to load and play:", EditorStyles.boldLabel);
        InitializeStyles();
        DrawFavoriteScenes();
        DrawSceneList();
        DrawRefreshButton();
    }

    private void DrawFavoriteScenes()
    {
        if (favoriteScenes != null)
        {
            GUILayout.Label("Favorite Scenes:", EditorStyles.boldLabel);
            foreach (string scenePath in favoriteScenes)
            {
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("★", favoritedStarStyle, GUILayout.Width(20)))
                {
                    UnfavoriteScene(scenePath);
                }

                if (GUILayout.Button(sceneName))
                {
                    EditorSceneManager.OpenScene(scenePath);
                }

                if (GUILayout.Button("▶", playButtonStyle, GUILayout.Width(20)))
                {
                    LoadAndPlayScene(scenePath);
                }

                GUILayout.EndHorizontal();
            }
        }
    }

    private void DrawSceneList()
    {
        GUILayout.Label("All Scenes:", EditorStyles.boldLabel);
        GUILayout.BeginVertical();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        if (scenePaths == null) return;

        foreach (string scenePath in scenePaths)
        {
            if (!favoriteScenes.Contains(scenePath))
            {
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("☆", unfavoritedStarStyle, GUILayout.Width(20)))
                {
                    FavoriteScene(scenePath);
                }

                if (GUILayout.Button(sceneName))
                {
                    EditorSceneManager.OpenScene(scenePath);
                }

                if (GUILayout.Button("▶", playButtonStyle, GUILayout.Width(20)))
                {
                    LoadAndPlayScene(scenePath);
                }

                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void DrawRefreshButton()
    {
        if (GUILayout.Button("Refresh", GUILayout.Height(30)))
        {
            RefreshEditor();
        }
    }

    private void FavoriteScene(string scenePath)
    {
        if (!favoriteScenes.Contains(scenePath))
        {
            favoriteScenes.Add(scenePath);

            // Save the updated list of favorite scenes to PlayerPrefs
            SaveFavoriteScenes();
        }
    }

    private void UnfavoriteScene(string scenePath)
    {
        if (favoriteScenes.Contains(scenePath))
        {
            favoriteScenes.Remove(scenePath);

            // Save the updated list of favorite scenes to PlayerPrefs
            SaveFavoriteScenes();
        }
    }

    private void LoadAndPlayScene(string scenePath)
    {
        if (EditorSceneManager.GetActiveScene().path == scenePath){
            EditorApplication.isPlaying = !EditorApplication.isPlaying;
            return;
        }

        if (EditorApplication.isPlaying) EditorApplication.isPlaying = false;

        EditorSceneManager.OpenScene(scenePath);
        EditorApplication.isPlaying = true;
    }


    private void RefreshEditor()
    {
        // Get the list of scenes from the build settings.
        scenePaths = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();
    }

    private void LoadFavoriteScenes()
    {
        // Load the list of favorite scenes from PlayerPrefs
        string serializedFavoriteScenes = PlayerPrefs.GetString("FavoriteScenes", "");
        favoriteScenes = new List<string>(serializedFavoriteScenes.Split(new[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries));
    }

    private void SaveFavoriteScenes()
    {
        // Save the list of favorite scenes to PlayerPrefs
        string serializedFavoriteScenes = string.Join(";", favoriteScenes);
        PlayerPrefs.SetString("FavoriteScenes", serializedFavoriteScenes);
        PlayerPrefs.Save();
    }
}
