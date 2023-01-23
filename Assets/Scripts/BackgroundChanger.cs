using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Background
{
    public string name;
    public Texture2D texture;
}
public class BackgroundChanger : MonoBehaviour
{
    [SerializeField] private List<Background> backgrounds;
    [SerializeField] private MeshRenderer backgroundMeshRenderer;
    [SerializeField] private float fadeInTime = 1f;
    [SerializeField] private float fadeOutTime = 1f;

    private Coroutine changeBackgroundRoutine;
    private string currentBackground = "";
    public void ChangeBackground(string newBackgroundName)
    {
        if (currentBackground == newBackgroundName)
            return;
        int i = 0;
        while (i < backgrounds.Count && backgrounds[i].name != newBackgroundName)
        {
            i++;
        }
        if (i >= backgrounds.Count)
            return;
        if (changeBackgroundRoutine != null)
        {
            StopCoroutine(changeBackgroundRoutine);
        }
        currentBackground = newBackgroundName;
        changeBackgroundRoutine = StartCoroutine(ChangeBackgroundRoutine(i));
    }   

    private IEnumerator ChangeBackgroundRoutine(int backgroundIndex)
    {
        float t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime / fadeOutTime;
            if (t < 0) t = 0;
            backgroundMeshRenderer.material.color = new Color(t, t, t, 1);
            yield return null;
        }
        backgroundMeshRenderer.material.SetTexture("_BaseMap", backgrounds[backgroundIndex].texture);
        backgroundMeshRenderer.material.SetTexture("_EmissionMap", backgrounds[backgroundIndex].texture);
        while (t < 1)
        {
            t += Time.deltaTime / fadeInTime;
            if (t > 1) t = 1;
            backgroundMeshRenderer.material.color = new Color(t, t, t, 1);
            yield return null;
        }
    }
}
