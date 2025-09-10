using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public Transform playerBody;
    public Transform cameraTarget;  // Reference to Camera object (for vertical rotation)
    public float mouseSensitivity = 3f;
    public float maxLookAngle = 80f;

    private float pitch = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        HandleCameraRotation();
    }
    
    
    void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        playerBody.Rotate(Vector3.up * mouseX);

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

        cameraTarget.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}
