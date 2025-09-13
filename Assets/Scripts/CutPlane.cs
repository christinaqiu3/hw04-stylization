using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutPlane : MonoBehaviour
{
    public Transform objectToCut;
    public Vector3 cutPoint = Vector3.zero;
    public Vector3 cutDirection = Vector3.forward;

    [Header("Frustum Settings")]
    public float xRatio = 1f;
    public float yRatio = 1f;
    public float fov = 60f;
    public float distance = 5f;

    [Header("Audio")]
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
            Debug.Log("cut");

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

        Plane cutPlane = new Plane(
            objectToCut.InverseTransformDirection(-cutDirection),
            objectToCut.InverseTransformPoint(cutPoint)
        );

        GeneratedMesh insideMesh = new GeneratedMesh();
        GeneratedMesh outsideMesh = new GeneratedMesh();

        Vector3[] vertices = originalMesh.vertices;
        Vector3[] normals = originalMesh.normals;
        Vector2[] uvs = originalMesh.uv;

        int subMeshCount = originalMesh.subMeshCount;

        for (int subMesh = 0; subMesh < subMeshCount; subMesh++) {

            int[] indices = originalMesh.GetTriangles(subMesh);

            for (int i = 0; i < indices.Length; i += 3)
            {

                Vector3 v0 = objectToCut.TransformPoint(vertices[indices[i]]);
                Vector3 v1 = objectToCut.TransformPoint(vertices[indices[i + 1]]);
                Vector3 v2 = objectToCut.TransformPoint(vertices[indices[i + 2]]);

                Vector3 n0 = normals[indices[i]];
                Vector3 n1 = normals[indices[i + 1]];
                Vector3 n2 = normals[indices[i + 2]];

                Vector2 uv0 = uvs[indices[i]];
                Vector2 uv1 = uvs[indices[i + 1]];
                Vector2 uv2 = uvs[indices[i + 2]];

                MeshTriangle tri = new MeshTriangle(
                    new Vector3[] { v0, v1, v2 },
                    new Vector3[] { n0, n1, n2 },
                    new Vector2[] { uv0, uv1, uv2 },
                    subMesh
                );

                // Classify by triangle center
                Vector3 center = (v0 + v1 + v2) / 3f;
                if (cutPlane.GetSide(center))
                    insideMesh.AddTriangle(tri);
                else
                    outsideMesh.AddTriangle(tri);
            }
        }
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
