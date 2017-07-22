using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float turnSpeed = 4.0f;
    public Transform player;

    public Vector3 offset;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Look(float horizontal)
    {
        offset = Quaternion.AngleAxis(horizontal, Vector3.up) * offset;
        transform.position = Vector3.Lerp(transform.position, player.position + offset, turnSpeed);
        transform.LookAt(player.position);
    }
}

