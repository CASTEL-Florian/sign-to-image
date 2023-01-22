using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StyleSelectionUI : MonoBehaviour
{
    [SerializeField] private Transform centerEyeAnchor;
    [SerializeField] private PromptStyliser promptStyliser;
    [SerializeField] private AudioHandler audioHandler;
    [SerializeField] private List<Image> images;
    [SerializeField] private float height = 1.5f;
    [SerializeField] private float radius = 1f;
    [SerializeField] private float rotationSpeed = 0.5f;
    [SerializeField] private float lerpSpeed = 0.1f;
    [SerializeField] private float fadeInTime = 0.5f;
    [SerializeField] private float fadeOutTime = 0.5f;
    private float currentAngle = 0;
    private bool hidden = true;
    float alpha = 0;

    private void Start()
    {
        foreach (Image image in images)
        {
            image.color = new Color(1, 1, 1, alpha);
        }
    }
    void Update()
    {
        UpdateAlpha();
        if (alpha <= 0) {
            return;
        }
        Vector3 centerPos = centerEyeAnchor.transform.position;
        centerPos.y = 0; 
        transform.position = Vector3.Lerp(transform.position, centerPos + new Vector3(0, height, 0), lerpSpeed);
        float angle = currentAngle;
        for (int i = 0; i < images.Count; i++)
        {
            images[i].transform.position = transform.position + (radius * new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)));
            angle += 2 * Mathf.PI / images.Count;
            images[i].transform.LookAt(centerEyeAnchor.transform.position);
        }
        currentAngle += rotationSpeed * Time.deltaTime;
    }

    private void UpdateAlpha()
    {
        if (alpha < 1 && !hidden)
        {
            alpha += Time.deltaTime / fadeOutTime;
            if (alpha > 1) alpha = 1;
            foreach (Image image in images)
            {
                image.color = new Color(1, 1, 1, alpha);
            }
        }
        if (alpha > 0 && hidden)
        {
            alpha -= Time.deltaTime / fadeOutTime;
            if (alpha < 0) alpha = 0;
            foreach (Image image in images)
            {
                image.color = new Color(1, 1, 1, alpha);
            }
        }
    }

    public void Toggle()
    {
        hidden = !hidden;
        if (!hidden)
            transform.position = centerEyeAnchor.transform.position + new Vector3(0, height, 0);
    }

    public void ChangeStyle(int id)
    {
        if (alpha <= 0) return;
        promptStyliser.ChangeStyle(id);
        audioHandler.PlaySwitchStyleSound();
    }
}
