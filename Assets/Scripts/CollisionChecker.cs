using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
    [HideInInspector]
    // CHANGE
    public CutPlane cutPlane;
    [HideInInspector]
    public int side;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Cuttable") || other.gameObject.tag == "Cuttable")
        {
            cutPlane.AddObjectToFrustum(other.gameObject);
            Debug.Log("ENTERED");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Cuttable") || other.gameObject.tag == "Cuttable")
            cutPlane.RemoveObjectFromFrustum(other.gameObject);
    }
}