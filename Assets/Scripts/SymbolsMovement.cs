using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymbolsMovement : MonoBehaviour
{
    [SerializeField] private AnimationCurve movementTowardTargetCurve;
    [SerializeField] private float timeToGoToTarget = 5f;
    private bool targetReached = false;
    private bool startedMovingTowardTarget = false;


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
}
