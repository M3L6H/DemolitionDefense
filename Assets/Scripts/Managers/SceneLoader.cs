using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    private LoadingScreen loadingScreen;

    protected void Awake()
    {
        loadingScreen = FindObjectOfType<LoadingScreen>();

        if (loadingScreen == null)
            Debug.LogError($"{name}: unable to find loading screen!");
    }

    /// <summary>
    /// Called to load the subsequent level. Defers to an asynchronous load function.
    /// </summary>
    public void LoadLevel(string levelName)
    {
        List<string> scenesInBuild = new List<string>();
        for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            int lastSlash = scenePath.LastIndexOf("/");
            scenesInBuild.Add(scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1));
        }

        //if (!scenesInBuild.Contains(levelName))
        //{
        //    Debug.LogError($"{name}: Scene does not exist!");
        //    return;
        //}

        loadingScreen.gameObject.SetActive(true);
        StartCoroutine(LoadSceneAsync(levelName));
    }

    private IEnumerator LoadSceneAsync(string levelName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

}
