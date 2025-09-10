using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutPlane : MonoBehaviour
{
    public Transform objectToCut;

    [Header("Frustum Settings")]
    public float xRatio = 1f;
    public float yRatio = 1f;
    public float fov = 60f;
    public float distance = 5f;
    public AudioClip cutSound;
    private AudioSource audioSource;
    
    private Vector3 leftTopFrustum, rightTopFrustum, leftBottomFrustum, rightBottomFrustum;

    private Plane leftPlane, rightPlane, topPlane, bottomPlane;


    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        // Plane cutPlane = new Plane(
        //     objectToCut.InverseTransformDirection(-cutDirection),
        //     objectToCut.InverseTransformPoint(cutPoint)
        // );

        // if (vertices.Count > 0)
        // {
        //     bool isOnPositiveSide = cutPlane.GetSide(vertices[0]);
        //     Debug.Log("First vertex is on positive side? " + isOnPositiveSide);
        // }

        // if (vertices.Count > 0)
        // {
        //     Vector3 centerPosition = Vector3.zero;
        //     foreach (var v in vertices)
        //         centerPosition += v;
        //     centerPosition /= vertices.Count;

        //     Debug.Log("Center of vertices: " + centerPosition);
        // }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFrustumCorners();
        CreatePlanes();

        if (Input.GetKeyDown(KeyCode.E))
        {
            CutObject();
        }
    }

    void UpdateFrustumCorners()
    {
        float aspectRatio = xRatio / yRatio;
        float frustumHeight = 2.0f * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad) * distance;
        float frustumWidth = frustumHeight * aspectRatio;

        // Transformed into world space
        leftTopFrustum = transform.TransformPoint(new Vector3(-frustumWidth * 0.5f, frustumHeight * 0.5f, distance));
        rightTopFrustum = transform.TransformPoint(new Vector3(frustumWidth * 0.5f, frustumHeight * 0.5f, distance));
        leftBottomFrustum = transform.TransformPoint(new Vector3(-frustumWidth * 0.5f, -frustumHeight * 0.5f, distance));
        rightBottomFrustum = transform.TransformPoint(new Vector3(frustumWidth * 0.5f, -frustumHeight * 0.5f, distance));
    }

    void CreatePlanes()
    {
        Vector3 camPos = transform.position;
        leftPlane = new Plane(camPos, leftTopFrustum, leftBottomFrustum);
        rightPlane = new Plane(camPos, rightBottomFrustum, rightTopFrustum);
        topPlane = new Plane(camPos, rightTopFrustum, leftTopFrustum);
        bottomPlane = new Plane(camPos, leftBottomFrustum, rightBottomFrustum);
    }

    void CutObject()
    {
        if (cutSound) audioSource.PlayOneShot(cutSound);
        if (!objectToCut) return;

        MeshFilter mf = objectToCut.GetComponent<MeshFilter>();
        if (!mf) return;

        Mesh originalMesh = mf.mesh;
        GeneratedMesh insideMesh = new GeneratedMesh();
        GeneratedMesh outsideMesh = new GeneratedMesh();

        // --- Pseudocode ---
        // For each triangle in original mesh:
        // - Get vertices
        // - Test if inside all planes
        // - If inside → add to insideMesh
        // - Else → add to outsideMesh
        // (TODO triangle clipping logic)

        // For now, I just demonstrate separation by duplicating mesh
        mf.mesh = insideMesh.GetGeneratedMesh();

        GameObject outsideObject = new GameObject(objectToCut.name + "_CutPiece");
        outsideObject.transform.position = objectToCut.position;
        outsideObject.transform.rotation = objectToCut.rotation;

        MeshFilter newMF = outsideObject.AddComponent<MeshFilter>();
        newMF.mesh = outsideMesh.GetGeneratedMesh();

        MeshRenderer newMR = outsideObject.AddComponent<MeshRenderer>();
        newMR.sharedMaterial = objectToCut.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void OnDrawGizmos()
    {
        UpdateFrustumCorners();
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(transform.position, rightTopFrustum);
        Gizmos.DrawLine(transform.position, leftTopFrustum);
        Gizmos.DrawLine(transform.position, rightBottomFrustum);
        Gizmos.DrawLine(transform.position, leftBottomFrustum);

        Gizmos.DrawLine(leftTopFrustum, rightTopFrustum);
        Gizmos.DrawLine(leftTopFrustum, leftBottomFrustum);
        Gizmos.DrawLine(rightTopFrustum, rightBottomFrustum);
        Gizmos.DrawLine(leftBottomFrustum, rightBottomFrustum);

        CreatePlanes();
    }
}
