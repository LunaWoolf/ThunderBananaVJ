using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EyeballController : MonoBehaviour
{
    public float rotationSpeed_key = 20.0f;
    public float rotationSpeed_mouse = 5.0f;
    private bool isMouseControlMode = true;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Toggle between mouse control and WASD movement mode
            isMouseControlMode = !isMouseControlMode;
        }

        if (isMouseControlMode)
        {
            // Mouse control mode
            RotateTowardsMouse();
        }
        else
        {
            // WASD movement mode
            RotateWithWASD();
        }
    }

    private void RotateTowardsMouse()
    {
        // Get the mouse position in screen coordinates
        Vector3 mousePosition = Input.mousePosition;

        // Calculate the rotation angles based on the mouse position
        float horizontalRotationAngle = (mousePosition.x / Screen.width - 0.5f) * 360f;
        float verticalRotationAngle = (mousePosition.y / Screen.height - 0.5f) * 360f;

        // Clamp the vertical rotation angle between -60 and 60 degrees
        verticalRotationAngle = Mathf.Clamp(verticalRotationAngle, -20f, 20f);
        horizontalRotationAngle = Mathf.Clamp(horizontalRotationAngle, -20f, 20f);

        // Apply th
        Quaternion targetRotation = Quaternion.Euler(0f, -horizontalRotationAngle, verticalRotationAngle);

        // Smoothly interpolate towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed_mouse * Time.deltaTime);
    }

    private void RotateWithWASD()
    {
        // Get input for rotation based on A and D keys
        float horizontalRotation = Input.GetAxis("Horizontal") * -rotationSpeed_key * Time.deltaTime;

        // Get input for rotation based on W and S keys
        float verticalRotation = Input.GetAxis("Vertical") * rotationSpeed_key * Time.deltaTime;

        // Rotate the eyeball accordingly
        transform.Rotate(Vector3.up, horizontalRotation);
        transform.Rotate(Vector3.forward, verticalRotation);
    }
}
