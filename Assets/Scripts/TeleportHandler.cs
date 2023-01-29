using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TeleportHandler : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private GameObject teleportationIndicatorPrefab;
    [SerializeField] private LineRenderer lineRendererPrefab;
    [SerializeField] private Transform indexFingerPos1;
    [SerializeField] private Transform indexFingerPos2;
    [SerializeField] private float fadeOutTime = 1f;
    [SerializeField] private float fadeInTime = 0.3f;
    [SerializeField] private float floorHeight = 0;
    private Transform x1;
    private Transform x_1;
    private Transform z1;
    private Transform z_1;
    private bool hidden = true;
    private float alpha = 0;
    Gradient lineRendererGradient;
    private Trigger currentTrigger = null;
    private MeshRenderer teleportationIndicatorMeshRenderer;
    private Transform teleportationIndicator;
    private LineRenderer lineRenderer;


    private void Start()
    {
        teleportationIndicator = Instantiate(teleportationIndicatorPrefab).transform;
        teleportationIndicatorMeshRenderer = teleportationIndicator.GetComponentInChildren<MeshRenderer>();
        lineRenderer = Instantiate(lineRendererPrefab);
        lineRendererGradient = lineRenderer.colorGradient;
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[lineRendererGradient.alphaKeys.Length];
        for (int i = 0; i < alphaKeys.Length; i++)
        {
            alphaKeys[i] = new GradientAlphaKey(lineRendererGradient.alphaKeys[i].alpha * alpha, lineRendererGradient.alphaKeys[i].time);
        }
        Gradient newLineRendererGradient = new Gradient();
        newLineRendererGradient.SetKeys
        (
            lineRenderer.colorGradient.colorKeys,
            alphaKeys
        );
        lineRenderer.colorGradient = newLineRendererGradient;
        Color col = teleportationIndicatorMeshRenderer.material.color;
        col.a = alpha;
        teleportationIndicatorMeshRenderer.material.color = col;
        x1 = teleportationIndicator.Find("x1");
        x_1 = teleportationIndicator.Find("x-1");
        z1 = teleportationIndicator.Find("z1");
        z_1 = teleportationIndicator.Find("z-1");
    }
    public void ShootRay()
    {
        RaycastHit hit;
        Vector3 origin = indexFingerPos2.position;
        Vector3 dir = indexFingerPos2.position - indexFingerPos1.position;

        if (Physics.Raycast(origin, dir, out hit, Mathf.Infinity, whatIsGround))
        {
            Vector3 hitPos = hit.point;
            if (hit.collider.tag == "Trigger")
                currentTrigger = hit.collider.GetComponent<Trigger>();
            else
            {
                currentTrigger = null;
                hitPos.y = 0;
            }
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, hitPos + new Vector3(0, floorHeight, 0));
            teleportationIndicator.position = hitPos + new Vector3(0, floorHeight, 0);
        }
    }

    public Vector3 GetTpPosition()
    {
        Vector3 tpPos = teleportationIndicator.position;
        Collider[] colliders = Physics.OverlapSphere(x1.position, 0.01f, whatIsGround);
        if (colliders.Length != 0) tpPos.x -= 1;
        colliders = Physics.OverlapSphere(x_1.position, 0.01f, whatIsGround);
        if (colliders.Length != 0) tpPos.x += 1;
        colliders = Physics.OverlapSphere(z1.position, 0.01f, whatIsGround);
        if (colliders.Length != 0) tpPos.z -= 1;
        colliders = Physics.OverlapSphere(z_1.position, 0.01f, whatIsGround);
        if (colliders.Length != 0) tpPos.z += 1;

        return teleportationIndicator.position;
    }

    public Trigger GetCurrentTrigger()
    {
        return currentTrigger;
    }

    public void Show()
    {
        hidden = false;
    }

    public void Hide()
    {
        hidden = true;
    }

    private void Update()
    {
        if (hidden && alpha > 0)
        {
            alpha -= Time.deltaTime / fadeOutTime;
            if (alpha < 0)
                alpha = 0;
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[lineRendererGradient.alphaKeys.Length];
            for (int i = 0; i < alphaKeys.Length; i++)
            {
                alphaKeys[i] = new GradientAlphaKey(lineRendererGradient.alphaKeys[i].alpha * alpha, lineRendererGradient.alphaKeys[i].time);
            }
            Gradient newLineRendererGradient = new Gradient();
            newLineRendererGradient.SetKeys
            (
                lineRenderer.colorGradient.colorKeys,
                alphaKeys
            );
            lineRenderer.colorGradient = newLineRendererGradient;
            Color col = teleportationIndicatorMeshRenderer.material.color;
            col.a = alpha;
            teleportationIndicatorMeshRenderer.material.color = col;
        }
        if (!hidden && alpha < 1)
        {
            alpha += Time.deltaTime / fadeInTime;
            if (alpha > 1)
                alpha = 1;
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[lineRendererGradient.alphaKeys.Length];
            for (int i = 0; i < alphaKeys.Length; i++)
            {
                alphaKeys[i] = new GradientAlphaKey(lineRendererGradient.alphaKeys[i].alpha * alpha, lineRendererGradient.alphaKeys[i].time);
            }
            Gradient newLineRendererGradient = new Gradient();
            newLineRendererGradient.SetKeys
            (
                lineRenderer.colorGradient.colorKeys,
                alphaKeys
            );
            lineRenderer.colorGradient = newLineRendererGradient;
            Color col = teleportationIndicatorMeshRenderer.material.color;
            col.a = alpha;
            teleportationIndicatorMeshRenderer.material.color = col;
        }
    }
}
