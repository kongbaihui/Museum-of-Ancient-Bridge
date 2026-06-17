using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraRotate : MonoBehaviour
{
    public float sensX = 2.56f;
    public float sensY = 2.56f;
    public float lookSmooth = 20f;

    public Transform orientation;

    float yRotation;
    float xRotation;
    float currentYRotation;
    float currentXRotation;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 initialEuler = transform.rotation.eulerAngles;
        currentXRotation = initialEuler.x > 180f ? initialEuler.x - 360f : initialEuler.x;
        currentYRotation = initialEuler.y;
        xRotation = currentXRotation;
        yRotation = currentYRotation;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        float blend = 1f - Mathf.Exp(-lookSmooth * Time.deltaTime);
        currentXRotation = Mathf.Lerp(currentXRotation, xRotation, blend);
        currentYRotation = Mathf.Lerp(currentYRotation, yRotation, blend);

        transform.rotation = Quaternion.Euler(currentXRotation, currentYRotation, 0);
        orientation.rotation = Quaternion.Euler(0, currentYRotation, 0);
    }
}
