using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SceneManager : MonoSingleton<SceneManager>
{
    public UnityAction<float> OnProgress = null;
    public UnityAction OnLoad;

    // Use this for initialization

    // Update is called once per frame
    void Update () {
		
	}

    public void LoadScene(string name)
    {
        OnLoad?.Invoke();
        StartCoroutine(LoadLevel(name));
    }

    IEnumerator LoadLevel(string name)
    {
        Debug.LogFormat("LoadLevel: {0}", name);
        AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
        async.allowSceneActivation = true;
        async.completed += LevelLoadCompleted;
        while (!async.isDone)
        {      
            OnProgress?.Invoke(async.progress);
            yield return null;
        }
    }

    private void LevelLoadCompleted(AsyncOperation obj)
    {
        if (OnProgress != null)
            OnProgress(1f);
        Debug.Log("LevelLoadCompleted:" + obj.progress);
    }


}
