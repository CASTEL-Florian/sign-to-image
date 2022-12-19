using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public TMP_InputField debugLog;

    public void PlayerMove(Vector3 direction)
    {
        transform.position += direction * Time.deltaTime * speed;
        debugLog.text = "isMoving";
    }
}
