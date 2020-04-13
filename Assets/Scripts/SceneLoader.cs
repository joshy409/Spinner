using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    private string sceneNameToBeLoaded;

    public void LoadScene(string sceneName)
    {
        sceneNameToBeLoaded = sceneName;

        StartCoroutine(InitializeSceneLoading());
    }

    IEnumerator InitializeSceneLoading()
    {
        yield return SceneManager.LoadSceneAsync("Scene_Loading");

        StartCoroutine(LoadAcutalScene());

    }

    IEnumerator LoadAcutalScene()
    {
       var asyncSceneLoading = SceneManager.LoadSceneAsync(sceneNameToBeLoaded);

        //this stops the scen efrom displaying when it is still loading...
        asyncSceneLoading.allowSceneActivation = false;

        while(!asyncSceneLoading.isDone)
        {
            Debug.Log(asyncSceneLoading.progress);
            if(asyncSceneLoading.progress >= .0f)
            {
                //show the scene
                asyncSceneLoading.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
