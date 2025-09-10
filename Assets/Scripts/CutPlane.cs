using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutPlane : MonoBehaviour
{
    public Transform objectToCut;
    public Vector3 cutDirection = Vector3.forward;
    public Vector3 cutPoint = Vector3.zero;
    public List<Vector3> vertices = new List<Vector3>();

    [Header("Frustum Settings")]
    public float xRatio = 1f;
    public float yRatio = 1f;
    public float fov = 60f;
    public float distance = 5f;
    private Vector3 leftTopFrustum, rightTopFrustum, leftBottomFrustum, rightBottomFrustum;

    private Plane leftPlane, rightPlane, topPlane, bottomPlane;


    void Start()
    {
        Plane cutPlane = new Plane(
            objectToCut.InverseTransformDirection(-cutDirection),
            objectToCut.InverseTransformPoint(cutPoint)
        );

        if (vertices.Count > 0)
        {
            bool isOnPositiveSide = cutPlane.GetSide(vertices[0]);
            Debug.Log("First vertex is on positive side? " + isOnPositiveSide);
        }

        if (vertices.Count > 0)
        {
            Vector3 centerPosition = Vector3.zero;
            foreach (var v in vertices)
                centerPosition += v;
            centerPosition /= vertices.Count;

            Debug.Log("Center of vertices: " + centerPosition);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        float aspectRatio = xRatio / yRatio;
        var frustumHeight = 2.0f * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
        var frustumWidth = frustumHeight * aspectRatio;

        Vector3 leftTopFrustum = new Vector3(-frustumWidth * 0.5f, frustumHeight * 0.5f, distance);
        Vector3 rightTopFrustum = new Vector3(frustumWidth * 0.5f, frustumHeight * 0.5f, distance);
        Vector3 leftBottomFrustum = new Vector3(-frustumWidth * 0.5f, -frustumHeight * 0.5f, distance);
        Vector3 rightBottomFrustum = new Vector3(frustumWidth * 0.5f, -frustumHeight * 0.5f, distance);

        // Transform into world space
        leftTopFrustum = transform.TransformPoint(leftTopFrustum);
        rightTopFrustum = transform.TransformPoint(rightTopFrustum);
        leftBottomFrustum = transform.TransformPoint(leftBottomFrustum);
        rightBottomFrustum = transform.TransformPoint(rightBottomFrustum);

        Gizmos.DrawLine(Vector3.zero, rightTopFrustum);
        Gizmos.DrawLine(Vector3.zero, leftTopFrustum);
        Gizmos.DrawLine(Vector3.zero, rightBottomFrustum);
        Gizmos.DrawLine(Vector3.zero, leftBottomFrustum);

        Gizmos.DrawLine(leftTopFrustum, rightTopFrustum);
        Gizmos.DrawLine(leftTopFrustum, leftBottomFrustum);
        Gizmos.DrawLine(rightTopFrustum, rightBottomFrustum);
        Gizmos.DrawLine(leftBottomFrustum, rightBottomFrustum);

        CreatePlane();
    }

    void CreatePlane()
    {
        Vector3 cameraPos = transform.position;
        // leftPlane = new Plane(cameraPos, leftUpFrustum, leftBottomFrustum);
        // rightPlane = new Plane(cameraPos, rightBottomFrustum, rightUpFrustum);
        // topPlane = new Plane(cameraPos, rightUpFrustum, leftUpFrustum);
        // bottomPlane = new Plane(cameraPos, leftBottomFrustum, rightBottomFrustum);
    }
}
