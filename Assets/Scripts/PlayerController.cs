using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    public Transform cameraTarget;
    public float movementIntensity = 5f;
    public float jumpIntensity = 5f;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        Vector3 forwardDirection = cameraTarget.forward;
        Vector3 rightDirection = cameraTarget.right;

        // Flatten so we only move on XZ plane
        forwardDirection.y = 0;
        rightDirection.y = 0;

        forwardDirection.Normalize();
        rightDirection.Normalize();

        // Get input
        float moveX = 0f;
        float moveZ = 0f;

        if (Input.GetKey(KeyCode.W)) moveZ += 1f;
        if (Input.GetKey(KeyCode.S)) moveZ -= 1f;
        if (Input.GetKey(KeyCode.D)) moveX += 1f;
        if (Input.GetKey(KeyCode.A)) moveX -= 1f;

        // Combine movement directions
        Vector3 moveDirection = (forwardDirection * moveZ + rightDirection * moveX).normalized;

        // Preserve current Y velocity so gravity still works
        Vector3 velocity = moveDirection * movementIntensity;
        velocity.y = rb.velocity.y;

        rb.velocity = velocity;

        // Handle jump (directly set vertical velocity)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // reset Y first
            rb.velocity += Vector3.up * jumpIntensity; // instant vertical boost
        }
    }
}
