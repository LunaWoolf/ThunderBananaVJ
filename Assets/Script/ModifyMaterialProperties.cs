using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyMaterialProperties : MonoBehaviour
{
    public Material targetMaterial; // Assign the material in the Inspector
    public Material skyMaterial; // Assign the material in the Inspector
    public float speedIncrement = 0.1f;
    public float colorIncrement = 0.1f;

    float moveDirection = 1;
    public float emissionIncrement = 0.1f;
    public float minEmission = 0.0f; // Minimum emission value
    public float maxEmission = 1.0f; // Maximum emission value

    private float[] speedValues = { 0.5f, 1.0f, 1.5f, 2.0f, 3f, 4f, 5f, 7f, 10f };
    private float currentSpeedIndex = 1;
    private void Update()
    {
        for (int i = 1; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                SetMaterialSpeed(speedValues[i - 1]);
            }
        }

        // Increase speed when the up arrow key is pressed or held
        if (Input.GetKey(KeyCode.UpArrow))
        {
            ModifyMaterialSpeed(speedIncrement);
        }

        // Decrease speed when the down arrow key is pressed
        if (Input.GetKey(KeyCode.DownArrow))
        {

            ModifyMaterialSpeed(-speedIncrement);
        }

        // Set speed to a negative value when the spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            moveDirection = moveDirection * -1;
            SetMaterialSpeed(targetMaterial.GetFloat("_speed"));
           
        }

        if (Input.GetKey(KeyCode.Alpha0))
        {

            SetMaterialSpeed(0.1f);
        }

        // Adjust the color with wrapping when the "Q" and "W" keys are pressed
        if (Input.GetKey(KeyCode.Q))
        {
            ModifyMaterialColorR(colorIncrement);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            ModifyMaterialColorR(-colorIncrement);
        }

        // Adjust the color with wrapping when the "A" and "S" keys are pressed
        if (Input.GetKey(KeyCode.A))
        {
            ModifyMaterialColorG(colorIncrement);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            ModifyMaterialColorG(-colorIncrement);
        }

        // Adjust the color with wrapping when the "Z" and "X" keys are pressed
        if (Input.GetKey(KeyCode.Z))
        {
            ModifyMaterialColorB(colorIncrement);
        }
        else if (Input.GetKey(KeyCode.X))
        {
            ModifyMaterialColorB(-colorIncrement);
        }

        // Increase or decrease emission within the specified range when the "P" and "O" keys are pressed
        if (Input.GetKey(KeyCode.P))
        {
            ModifyMaterialEmission(emissionIncrement);
        }
        else if (Input.GetKey(KeyCode.O))
        {
            ModifyMaterialEmission(-emissionIncrement);
        }


        //___________________________________________________________________________________


        // Adjust the color with wrapping when the "A" and "S" keys are pressed
        if (Input.GetKey(KeyCode.E))
        {
            ModifySkyMaterialColorR(colorIncrement);
        }
        else if (Input.GetKey(KeyCode.R))
        {
            ModifySkyMaterialColorR(-colorIncrement);
        }

        // Adjust the color with wrapping when the "Z" and "X" keys are pressed
        if (Input.GetKey(KeyCode.D))
        {
            ModifySkyMaterialColorG(colorIncrement);
        }
        else if (Input.GetKey(KeyCode.F))
        {
            ModifySkyMaterialColorG(-colorIncrement);
        }

        // Increase or decrease emission within the specified range when the "P" and "O" keys are pressed
        if (Input.GetKey(KeyCode.C))
        {
            ModifySkyMaterialColorB(colorIncrement);
        }
        else if (Input.GetKey(KeyCode.V))
        {
            ModifySkyMaterialColorB(colorIncrement);
        }
    }

    private void ModifyMaterialSpeed(float increment)
    {
        targetMaterial.SetFloat("_speed", targetMaterial.GetFloat("_speed") + increment);
    }

    private void SetMaterialSpeed(float value)
    {
        targetMaterial.SetFloat("_speed", value * moveDirection);
    }

    private void ModifyMaterialColorR(float increment)
    {
        Color color = targetMaterial.GetColor("_Color");
        color.r = Mathf.Repeat(color.r + increment, 1f);
        targetMaterial.SetColor("_Color", color);
    }

    private void ModifyMaterialColorG(float increment)
    {
        Color color = targetMaterial.GetColor("_Color");
        color.g = Mathf.Repeat(color.g + increment, 1f);
        targetMaterial.SetColor("_Color", color);
    }

    private void ModifyMaterialColorB(float increment)
    {
        Color color = targetMaterial.GetColor("_Color");
        color.b = Mathf.Repeat(color.b + increment, 1f);
        targetMaterial.SetColor("_Color", color);
    }

    private void ModifyMaterialEmission(float increment)
    {
        float emission = Mathf.Clamp(targetMaterial.GetFloat("_emission") + increment, minEmission, maxEmission);
        targetMaterial.SetFloat("_emission", emission);
    }



    private void ModifySkyMaterialColorR(float increment)
    {
        Color color = skyMaterial.GetColor("_BottomColor");
        color.r = Mathf.Repeat(color.r + increment, 1f);
        skyMaterial.SetColor("_BottomColor", color);
    }

    private void ModifySkyMaterialColorG(float increment)
    {
        Color color = skyMaterial.GetColor("_BottomColor");
        color.g = Mathf.Repeat(color.g + increment, 1f);
        skyMaterial.SetColor("_BottomColor", color);
    }

    private void ModifySkyMaterialColorB(float increment)
    {
        Color color = skyMaterial.GetColor("_BottomColor");
        color.b = Mathf.Repeat(color.b + increment, 1f);
        skyMaterial.SetColor("_BottomColor", color);
    }
}
