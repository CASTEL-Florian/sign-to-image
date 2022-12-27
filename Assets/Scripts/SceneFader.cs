using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneFader : MonoBehaviour
{
    [SerializeField] private OVRScreenFade centerEyeAnchorFader;
    bool transitioning = false;

    public void LoadScene(int sceneIndex)
    {
        if (!transitioning)
            StartCoroutine(LoadSceneRoutine(sceneIndex));
    }

    private IEnumerator LoadSceneRoutine(int sceneIndex)
    {
        transitioning = true;
        centerEyeAnchorFader.FadeOut();
        yield return new WaitForSeconds(centerEyeAnchorFader.fadeTime);
        SceneManager.LoadScene(sceneIndex);
    }
    
}
