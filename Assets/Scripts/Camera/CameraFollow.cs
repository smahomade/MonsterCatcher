using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    // Start is called before the first frame update
    private void LateUpdate()
    {
        Follow();
    }

    // Update is called once per frame
    void Follow()
    {
        Vector3 targetPosition = target.position + offset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position,targetPosition,5*Time.fixedDeltaTime);
        transform.position = targetPosition;
    }
}
