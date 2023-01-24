using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigPainting : MonoBehaviour
{
    [SerializeField] private float scaleMultiplier = 5f;
    [SerializeField] private float positionOffset = 10f;
    [SerializeField] private float timeToTransition = 1f;
    [SerializeField] private float activationDistance = 1f;
    [SerializeField] private List<GameObject> objectsToHide;
    [SerializeField] private List<GameObject> objectsToShow;
    [SerializeField] private List<MeshRenderer> rendererToHide;
    [SerializeField] private OVRScreenFade centerEyeAnchorFader;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private Transform centerEyeAnchor;


    private bool activated = false;
    private bool transitioning = false;
    private float currentTime = 0;
    private Vector3 initialPos;

    private void Start()
    {
        initialPos = transform.position;
    }

    private void Update()
    {
        if (transitioning)
            return;
        if (currentTime > timeToTransition)
        {
            if (activated)
                Desactivate();
            else
                Activate();
            currentTime = 0;
        }
        else if (activated)
        {
            if (DistanceXZ(centerEyeAnchor.position, initialPos) > activationDistance)
            {
                currentTime += Time.deltaTime;
            }
            else
            {
                currentTime = 0;
            }
        } else
        {
            if (DistanceXZ(centerEyeAnchor.position, transform.position) < activationDistance)
            {
                currentTime += Time.deltaTime;
            }
            else
            {
                currentTime = 0;
            }
        }
    }

    public void Activate()
    {
        if (transitioning || activated)
            return;
        StartCoroutine(ActivationRoutine());
    }

    private IEnumerator ActivationRoutine()
    {
        transitioning = true;
        centerEyeAnchorFader.FadeOut();
        yield return new WaitForSeconds(centerEyeAnchorFader.fadeTime);
        foreach (var item in objectsToHide)
        {
            item.SetActive(false);
        }
        foreach (var item in objectsToShow)
        {
            item.SetActive(true);
        }
        foreach (var rend in rendererToHide)
        {
            rend.enabled = false;
        }
        player.Teleport(new Vector3(initialPos.x, player.transform.position.y, initialPos.z));
        transform.localScale = transform.localScale * scaleMultiplier;
        transform.Translate(-transform.forward * positionOffset);
        centerEyeAnchorFader.FadeIn();
        yield return new WaitForSeconds(centerEyeAnchorFader.fadeTime);
        transitioning = false;
        activated = true;
    }

    public void Desactivate()
    {
        if (transitioning || !activated)
            return;
        StartCoroutine(DesactivationRoutine());
    }

    private IEnumerator DesactivationRoutine()
    {
        transitioning = true;
        centerEyeAnchorFader.FadeOut();
        yield return new WaitForSeconds(centerEyeAnchorFader.fadeTime);
        foreach (var item in objectsToHide)
        {
            item.SetActive(true);
        }
        foreach (var item in objectsToShow)
        {
            item.SetActive(false);
        }
        foreach (var rend in rendererToHide)
        {
            rend.enabled = true;
        }
        transform.localScale = transform.localScale / scaleMultiplier;
        transform.position = initialPos;
        player.ResetPos();
        centerEyeAnchorFader.FadeIn();
        yield return new WaitForSeconds(centerEyeAnchorFader.fadeTime);
        transitioning = false;
        activated = false;
    }

    private float DistanceXZ(Vector3 a, Vector3 b)
    {
        Vector3 c = new Vector3(a.x - b.x, 0, a.z - b.z);
        return c.magnitude;
    }
}
