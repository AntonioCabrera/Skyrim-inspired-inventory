using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLookAt : MonoBehaviour
{

    public float speed = 3;

    Vector2 rotation = new Vector2(0, 0);
    float clampedXRotation;
    [HideInInspector]
    public bool CanLook;

    private void Awake()
    {
        CanLook = true;
    }
    void Update()
    {
        if (CanLook)
        {
            Cursor.visible = false;
            rotation.y += Input.GetAxis("Mouse X");
            rotation.x += -Input.GetAxis("Mouse Y");
            clampedXRotation = Mathf.Clamp(rotation.x, -15, 15);
            rotation.x = clampedXRotation;
            transform.eulerAngles = rotation * speed;
        }
        else
        {
            Cursor.visible = true;
        }
    }
}


