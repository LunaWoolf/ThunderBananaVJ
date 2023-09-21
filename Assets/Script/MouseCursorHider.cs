using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursorHider : MonoBehaviour
{
    private bool isCursorVisible = true;

    private void Update()
    {
        // Check for a mouse click (e.g., left mouse button)
        if (Input.GetMouseButtonDown(0))
        {
            ToggleMouseCursorVisibility();
        }
    }

    private void ToggleMouseCursorVisibility()
    {
        isCursorVisible = !isCursorVisible;

        if (isCursorVisible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
        }
    }
}
