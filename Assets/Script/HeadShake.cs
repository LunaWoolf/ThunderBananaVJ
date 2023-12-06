using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;
using RetroLookPro.Enums;
using LimitlessDev.RetroLookPro;
using UnityEngine;

public class HeadShake : MonoBehaviour
{
    public float shakeSpeed = 120f; // Number of shakes per minute
    public float shakeAngle = 15f; // Angle of head shake
    public float shakeDistance = 0.5f; // Maximum distance to move on the x-axis

    // AnimationCurve parameters for ease-in and ease-out
    public float easeInTime = 2f;
    public float easeOutTime = 2f;

    private float timeBetweenShakes;
    private float nextShakeTime;
    private bool isShakingRight;

    float originalX;
    bool stopped = false;

    void Start()
    {
        CalculateShakeTimings();
        originalX = this.transform.position.x;
    }

    void Update()
    {
        // Check if it's time to shake the head
        if (Time.time >= nextShakeTime && !stopped)
        {
            // Start the head shake coroutine
            StartCoroutine(ShakeCoroutine());

            // Calculate next shake time
            CalculateShakeTimings();
        }
    }

    public void StopAndTurn()
    {
        stopped = true;
        StopAllCoroutines();
        this.transform.rotation = Quaternion.Euler(0f, 71.7f, 0f);
        transform.localPosition = new Vector3(originalX, transform.localPosition.y, transform.localPosition.z);
    }

    IEnumerator ShakeCoroutine()
    {
        // Toggle shaking direction
        isShakingRight = !isShakingRight;

        // Calculate target rotation based on the shaking direction
        float targetRotationY = isShakingRight ? shakeAngle : -shakeAngle;

        // Time taken to reach the target rotation
        float duration = 0.5f; // Adjust as needed

        // Current rotation
        Quaternion startRotation = transform.localRotation;

        // Target rotation
        Quaternion targetRotation = Quaternion.Euler(0f, targetRotationY, 0f);

        // Interpolation curve for more dramatic start and end
        AnimationCurve curve = new AnimationCurve(
            new Keyframe(0, 0, 2 / easeInTime, 2 / easeInTime),
            new Keyframe(1, 1, 2 / easeOutTime, 2 / easeOutTime)
        );

        // Interpolate using the curve
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = curve.Evaluate(elapsedTime / duration);
            transform.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);

            // Calculate x-axis movement
            float xPos = Mathf.Sin(t * Mathf.PI) * shakeDistance;
            transform.localPosition = new Vector3(originalX+ xPos, transform.localPosition.y, transform.localPosition.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final rotation and position are exactly the target rotation and position
        transform.localRotation = targetRotation;
        transform.localPosition = new Vector3(originalX, transform.localPosition.y, transform.localPosition.z);
    }

    void CalculateShakeTimings()
    {
        // Calculate time between shakes based on shake speed
        timeBetweenShakes = 60f / shakeSpeed;

        // Set next shake time
        nextShakeTime = Time.time + timeBetweenShakes;
    }
}
