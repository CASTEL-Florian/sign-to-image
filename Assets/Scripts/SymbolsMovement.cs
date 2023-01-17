using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymbolsMovement : MonoBehaviour
{
    public bool _canMove = false;
    private float speed = 0.5f;
    private float minDistance = 0.1f;
    public GameObject target;
    public GameObject centerEyeAnchor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_canMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * speed);
            transform.rotation = centerEyeAnchor.transform.rotation;
            if(Vector3.Distance(transform.position, target.transform.position) < minDistance)
            {
                Destroy(gameObject);
            }
        }
    }
}
