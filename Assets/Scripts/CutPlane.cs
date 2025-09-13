using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutPlane : MonoBehaviour
{

    [Header("Frustum Settings")]
    public float xRatio = 1f;
    public float yRatio = 1f;
    public float fov = 60f;
    public float distance = 5f;

    [Header("Audio")]
    public AudioClip cutSound;
    public AudioSource audioSource;
    
    private Vector3 leftTopFrustum, rightTopFrustum, leftBottomFrustum, rightBottomFrustum;

    private Plane leftPlane, rightPlane, topPlane, bottomPlane;


    void Start()
    {
        // audioSource = gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFrustumCorners();
        CreatePlanes();

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryCutObject();
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

    void TryCutObject()
    {
        if (cutSound) audioSource.PlayOneShot(cutSound);

        GameObject[] candidates = GameObject.FindGameObjectsWithTag("Cuttable");

        for (int i = 0; i < candidates.Length; i++)
        {
            GameObject current = candidates[i];

            // Cut with left plane
            GameObject result = Cutter.Cut(current, GetPlaneContactPoint(leftPlane, current), leftPlane.normal);
            if (result != null) current = result;

            // Cut with right plane
            result = Cutter.Cut(current, GetPlaneContactPoint(rightPlane, current), rightPlane.normal);
            if (result != null) current = result;

            // Cut with top plane
            result = Cutter.Cut(current, GetPlaneContactPoint(topPlane, current), topPlane.normal);
            if (result != null) current = result;

            // Cut with bottom plane
            result = Cutter.Cut(current, GetPlaneContactPoint(bottomPlane, current), bottomPlane.normal);
            if (result != null) current = result;
        }
    }

    Vector3 GetPlaneContactPoint(Plane plane, GameObject obj)
    {
        Vector3 center = obj.GetComponent<Renderer>().bounds.center;
        float distanceToPlane = plane.GetDistanceToPoint(center);
        return center - plane.normal * distanceToPlane; // project center onto plane
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
