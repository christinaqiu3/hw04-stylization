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
    public Collider frustumCollider;

    [Header("Audio")]
    public AudioClip cutSound;
    public AudioClip pasteSound;
    public AudioSource audioSource;
    
    private Vector3 leftTopFrustum, rightTopFrustum, leftBottomFrustum, rightBottomFrustum;

    private Plane leftPlane, rightPlane, topPlane, bottomPlane;

    // Stores cut geometry for later placement
    private List<GameObject> storedInsidePieces = new List<GameObject>();
    
    public List<GameObject> objectsInsideFrustum = new List<GameObject>();


    void Start()
    {
        // audioSource = gameObject.AddComponent<AudioSource>();

        CollisionChecker[] checkers = GetComponentsInChildren<CollisionChecker>();
        foreach (var checker in checkers)
        {
            checker.cutPlane = this; // so OnTriggerEnter will have a reference
        }
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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PasteStoredGeometry();
        }
    }
    public void AddObjectToFrustum(GameObject obj)
    {
        if (!objectsInsideFrustum.Contains(obj))
            objectsInsideFrustum.Add(obj);
    }

    public void RemoveObjectFromFrustum(GameObject obj)
    {
        objectsInsideFrustum.Remove(obj);
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

        storedInsidePieces.Clear(); // Clear any previous stored selection

        // GameObject[] candidates = GameObject.FindGameObjectsWithTag("Cuttable");

        // Collect objects overlapping frustumCollider
        List<GameObject> candidates = new List<GameObject>();
        Collider[] overlaps = Physics.OverlapBox(
            frustumCollider.bounds.center,
            frustumCollider.bounds.extents,
            frustumCollider.transform.rotation,
            LayerMask.GetMask("Cuttable")
        );

        foreach (Collider col in overlaps)
        {
            GameObject current = col.gameObject;
            if (!candidates.Contains(current))
                candidates.Add(current);
        }

        foreach (GameObject current in objectsInsideFrustum)
        {
            GameObject insideMesh = current;

            // Check if object is hit by the collider attached to the camera

            // Make a duplicate of the object before cutting
            GameObject originalCopy = Instantiate(current);
            originalCopy.transform.SetPositionAndRotation(current.transform.position, current.transform.rotation);
            originalCopy.transform.localScale = current.transform.localScale;

            // === IDK IF THIS IS RIGHT END === 

            // Cut with left plane
            GameObject rightPiece = Cutter.Cut(insideMesh, GetPlaneContactPoint(leftPlane, insideMesh), leftPlane.normal, true);
            // Cut with right plane
            rightPiece = Cutter.Cut(insideMesh, GetPlaneContactPoint(rightPlane, insideMesh), rightPlane.normal, true);
            // Cut with top plane
            rightPiece = Cutter.Cut(insideMesh, GetPlaneContactPoint(topPlane, insideMesh), topPlane.normal, true);
            // Cut with bottom plane
            rightPiece = Cutter.Cut(insideMesh, GetPlaneContactPoint(bottomPlane, insideMesh), bottomPlane.normal, true);

            // === IDK IF THIS IS RIGHT === 
            Destroy(current); // Remove cut version from scene
            GameObject restored = Instantiate(originalCopy); // Restore original
            restored.SetActive(true);
            restored.tag = "Cuttable";
            Destroy(originalCopy);
            // === IDK IF THIS IS RIGHT END === 

            if (insideMesh != null)
            {
                // Disable the copy in scene (we just store it for later)
                // insideMesh.SetActive(false);
                // storedInsidePieces.Add(insideMesh);
                GameObject copy = Instantiate(insideMesh);
                copy.name = insideMesh.name + "_StoredCopy";
                copy.transform.SetPositionAndRotation(insideMesh.transform.position, insideMesh.transform.rotation);
                storedInsidePieces.Add(copy);
                copy.SetActive(false);
            }
        }
        GameObject[] cutResults = GameObject.FindGameObjectsWithTag("Cuttable");
        foreach (GameObject obj in cutResults)
        {
            if (obj.name.StartsWith("[TO_DELETE]_"))
            {
                Destroy(obj);
            }
        }
    }
    void PasteStoredGeometry()
    {
        if (pasteSound) audioSource.PlayOneShot(pasteSound);
        GameObject[] candidates = GameObject.FindGameObjectsWithTag("Cuttable");

        foreach (GameObject current in objectsInsideFrustum)
        {
            if (current == null) continue;
            Renderer rend = current.GetComponent<Renderer>();
            GameObject insideMesh = current;

            // Cut with left plane
            insideMesh = Cutter.Cut(insideMesh, GetPlaneContactPoint(leftPlane, insideMesh), leftPlane.normal);
            // Cut with right plane
            insideMesh = Cutter.Cut(insideMesh, GetPlaneContactPoint(rightPlane, insideMesh), rightPlane.normal);
            // Cut with top plane
            insideMesh = Cutter.Cut(insideMesh, GetPlaneContactPoint(topPlane, insideMesh), topPlane.normal);
            // Cut with bottom plane
            insideMesh = Cutter.Cut(insideMesh, GetPlaneContactPoint(bottomPlane, insideMesh), bottomPlane.normal);

            if (insideMesh != null)
            {
                // Store a copy BEFORE destroying
                GameObject copy = Instantiate(insideMesh);
                copy.SetActive(false);
                storedInsidePieces.Add(copy);
            }
            Destroy(current);
        }

        foreach (GameObject piece in storedInsidePieces)
        {
            if (piece == null) continue;
            piece.SetActive(true);

            piece.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2f;
            piece.transform.rotation = Camera.main.transform.rotation;
        }
    }

    Vector3 GetPlaneContactPoint(Plane plane, GameObject obj)
    {
        if (obj == null) 
        {
            Debug.LogWarning("GetPlaneContactPoint: Called with null object!");
            return Vector3.zero;
        }

        Renderer rend = obj.GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogWarning("GetPlaneContactPoint: Object has no Renderer!");
            return obj.transform.position; // fallback
        }

        return rend.bounds.center; 
        // Vector3 center = obj.GetComponent<Renderer>().bounds.center;
        // float distanceToPlane = plane.GetDistanceToPoint(center);
        // return center - plane.normal * distanceToPlane; // project center onto plane
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
