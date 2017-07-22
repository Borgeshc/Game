using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float turnSpeed = 4.0f;
    public Transform player;

    public Vector3 offset;

    void LateUpdate()
    {
        if(Input.GetMouseButton(1))
        {
            offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * offset;
        }
        transform.position = player.position + offset;
        transform.LookAt(player.position);
    }
}

