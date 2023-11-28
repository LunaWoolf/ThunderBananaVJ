using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameAnimation : MonoBehaviour
{
    public float speed = 1.0f;
    private Transform[] children;
    private int currentIndex = 0;

    private void Start()
    {
        // Get all the children of the GameObject and disable them.
        children = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
            children[i].gameObject.SetActive(false);
        }

        // Start the coroutine to enable and disable children in order.
        StartCoroutine(EnableDisableChildrenCoroutine());
    }

    private IEnumerator EnableDisableChildrenCoroutine()
    {
        while (true)
        {
            // Enable the current child.
            children[currentIndex].gameObject.SetActive(true);

            // Wait for the specified time (speed) before disabling the current child.
            yield return new WaitForSeconds(speed);

            // Disable the current child.
            children[currentIndex].gameObject.SetActive(false);

            // Move to the next child, and loop back to the first child if necessary.
            currentIndex = (currentIndex + 1) % children.Length;
        }
    }
}
