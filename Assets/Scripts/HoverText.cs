using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverText : MonoBehaviour
{
    private GameObject player;

    private void Start()
    {
        player = PlayerSingleton.Instance.gameObject;
    }

    private void Update()
    {
        Vector3 lookDirection = transform.position - player.transform.position;
        lookDirection.y = 0; // Ignore the upward component (set y to 0)

        // Check if the lookDirection is not zero (to avoid division by zero)
        if (lookDirection != Vector3.zero)
        {
            // Create a rotation that points in the look direction
            Quaternion desiredRotation = Quaternion.LookRotation(lookDirection.normalized);

            // Apply the rotation to your object
            transform.rotation = desiredRotation;
        }
    }
}
