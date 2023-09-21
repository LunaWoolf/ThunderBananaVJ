
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomFloatMovement : MonoBehaviour
{
    public float movementRange = 1.0f;
    public float movementSpeed = 1.0f;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    private void Start()
    {
        startPosition = transform.position;
        GenerateRandomTargetPosition();
    }

    private void Update()
    {
        // Move the object towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

        // Check if the object has reached the target position
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            GenerateRandomTargetPosition();
        }
    }

    private void GenerateRandomTargetPosition()
    {
        // Generate random offsets within the specified range for each axis
        float randomX = Random.Range(-movementRange, movementRange);
        float randomY = Random.Range(-movementRange, movementRange);
        float randomZ = Random.Range(-movementRange, movementRange);

        // Calculate the new target position
        targetPosition = startPosition + new Vector3(randomX, randomY, randomZ);
    }
}
