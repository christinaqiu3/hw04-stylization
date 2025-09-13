using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    public Transform cameraTarget;
    public float movementIntensity = 10f;
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

        if (Input.GetKey(KeyCode.W))
            rb.AddForce(forwardDirection * movementIntensity, ForceMode.Acceleration);
        if (Input.GetKey(KeyCode.S))
            rb.AddForce(-forwardDirection * movementIntensity, ForceMode.Acceleration);
        if (Input.GetKey(KeyCode.D))
            rb.AddForce(rightDirection * movementIntensity, ForceMode.Acceleration);
        if (Input.GetKey(KeyCode.A))
            rb.AddForce(-rightDirection * movementIntensity, ForceMode.Acceleration);
        //if (Input.GetKey(KeyCode.E))

    }
}
