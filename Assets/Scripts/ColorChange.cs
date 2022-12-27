using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;

    private Color originalColor;

    [SerializeField] Color newColor;

    public float transitionDuration = 0.3f;

    private bool isChangingColor = false;

    void Start()
    {
        originalColor = skinnedMeshRenderer.material.color;
    }
    public void Pulse()
    {
        if (!isChangingColor)
        {
            StartCoroutine(PulseCoroutine());
        }
    }

    IEnumerator PulseCoroutine()
    {
        isChangingColor = true;

        skinnedMeshRenderer.material.color = newColor;

        yield return new WaitForSeconds(transitionDuration);

        float t = 0;
        while (t < transitionDuration)
        {
            t += Time.deltaTime;
            skinnedMeshRenderer.material.color = Color.Lerp(newColor, originalColor, t / transitionDuration);
            yield return null;
        }
        skinnedMeshRenderer.material.color = originalColor;

        isChangingColor = false;
    }

    public void ChangeColor(Color col)
    {
        originalColor = col;
        if (!isChangingColor)
            skinnedMeshRenderer.material.color = col;

    }
}
