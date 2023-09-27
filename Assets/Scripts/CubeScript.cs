using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScript : MonoBehaviour
{
    public GameObject fieldPrefab;

    public Cube prototypeCube;

    public Vector3 prototypeCubePosition;
    public Vector3 prototypeCubeRotation;

    // Start is called before the first frame update
    void Start()
    {
        prototypeCube = new Cube(prototypeCubePosition, Quaternion.Euler(prototypeCubeRotation));

        if (fieldPrefab)
        {
            prototypeCube.instantiateMeshes(fieldPrefab);
        }
    }

    // Update is called once per frame
    void Update()
    {
           
    }
}
