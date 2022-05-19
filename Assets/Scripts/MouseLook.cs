using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody;
    public float mouseSensitivity = 300;
    private float xRotation = 0f;
    public bool mouselook = true;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown("b")){
            mouselook = !mouselook;
            if(mouselook){ Cursor.lockState = CursorLockMode.Locked; }
            else { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
        }

        if(mouselook){
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }

    }
}