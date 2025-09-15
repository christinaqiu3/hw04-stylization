using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
public class DynamicFrustumTrigger : MonoBehaviour
{
    private MeshCollider meshCollider;
    private MeshFilter meshFilter;
    private Mesh frustumMesh;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        meshCollider.convex = true;
        meshCollider.isTrigger = true;

        frustumMesh = new Mesh();
        meshFilter.mesh = frustumMesh;
        meshCollider.sharedMesh = frustumMesh;

        // Make sure this object has a Rigidbody (required for trigger events)
        if (!TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    void LateUpdate()
    {
        UpdateFrustumMesh(Camera.main);
        meshCollider.sharedMesh = frustumMesh; // refresh collider each frame
    }

    void UpdateFrustumMesh(Camera cam)
    {
        if (cam == null) return;

        float near = cam.nearClipPlane;
        float far = cam.farClipPlane;

        Vector3[] corners = new Vector3[8];

        // Get frustum corners in world space
        Vector3[] nearCorners = new Vector3[4];
        Vector3[] farCorners = new Vector3[4];
        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), near, Camera.MonoOrStereoscopicEye.Mono, nearCorners);
        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), far, Camera.MonoOrStereoscopicEye.Mono, farCorners);

        for (int i = 0; i < 4; i++)
        {
            corners[i] = cam.transform.TransformPoint(nearCorners[i]);
            corners[i + 4] = cam.transform.TransformPoint(farCorners[i]);
        }

        frustumMesh.Clear();
        frustumMesh.vertices = corners;
        frustumMesh.triangles = new int[]
        {
            // near face
            0, 1, 2, 0, 2, 3,
            // far face
            4, 6, 5, 4, 7, 6,
            // left face
            0, 3, 7, 0, 7, 4,
            // right face
            1, 5, 6, 1, 6, 2,
            // top face
            3, 2, 6, 3, 6, 7,
            // bottom face
            0, 4, 5, 0, 5, 1
        };
        frustumMesh.RecalculateNormals();
    }
}
