using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingSymbol : MonoBehaviour
{
    [SerializeField] private AnimationCurve movementTowardTargetCurve;
    [SerializeField] private float timeToGoToTarget = 5f;
    [SerializeField] private Transform imageTransform;
    [SerializeField] private float upDownMovementMagnetude = 0.01f;
    [SerializeField] private float upDownMovementTime = 2f;
    [SerializeField] private Image symbolImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private float fadeTime = 0.5f;
    private bool hidden = false;
    private float alpha = 0;
    private bool targetReached = false;
    private bool startedMovingTowardTarget = false;
    private float currentTime = 0;
    private string type;
    private bool fadeOutAndDestroy = false;

    private void UpdateColor()
    {
        symbolImage.color = new Color(1, 1, 1, alpha);
        Color backgroundColor = backgroundImage.color;
        backgroundColor.a = alpha;
        backgroundImage.color = backgroundColor;
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        imageTransform.transform.localPosition = new Vector3(0, upDownMovementMagnetude * Mathf.Sin(2 * Mathf.PI * currentTime / upDownMovementTime), 0);
        if (alpha < 1 && !hidden)
        {
            alpha += Time.deltaTime / fadeTime;
            if (alpha > 1) alpha = 1;
            UpdateColor();
        } else if (alpha > 0 && hidden){
            alpha -= Time.deltaTime / fadeTime;
            if (alpha < 0) alpha = 0;
            UpdateColor();
        }
        if (alpha <= 0 && fadeOutAndDestroy)
        {
            Destroy(gameObject);
        }
    }
    public void GoToTarget(Vector3 target)
    {
        if (startedMovingTowardTarget) return;
        startedMovingTowardTarget = true;
        StartCoroutine(GoToTargetRoutine(target));
    }

    public IEnumerator GoToTargetRoutine(Vector3 target)
    {
        Vector3 initialPos = transform.position;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / timeToGoToTarget;
            if (t > 1) t = 1;
            transform.position = Vector3.Lerp(initialPos, target, t);
            yield return null;
        }
        targetReached = true;
    }

    public bool TargetReached()
    {
        return targetReached;
    }

    public void SetSymbolImage(Sprite sprite)
    {
        symbolImage.sprite = sprite;
    }

    public void SetBackgroundColor(Color col)
    {
        backgroundImage.color = col;
    }

    public void SetType(string t)
    {
        type = t;
    }
    public string GetSymbolType()
    {
        return type;
    }
    public void FadeOutAndDestroy()
    {
        hidden = true;
        fadeOutAndDestroy = true;
    }
}
