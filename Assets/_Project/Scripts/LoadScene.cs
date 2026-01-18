using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public bool loadAtStart = false;
    public string sceneName;

    IEnumerator Start()
    {
        if(loadAtStart == false) yield break;

        yield return new WaitForEndOfFrame();
        yield return null;

        LoadSceneByName();
    }

    public void LoadSceneByName()
    {
        SceneManager.LoadScene(sceneName);
    }
}