using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyManager : MonoBehaviour
{
    public float skySpeed;
    public Material skyboxMaterial;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        skyboxMaterial.SetFloat("_Rotation", Time.time * skySpeed);
    }
}
