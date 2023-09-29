using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScript : MonoBehaviour
{
    public GameObject fieldPrefab;

    public Cube prototypeCube;

    public Vector3 dimensionAsVector; // only for editing in Unity Editor
    public Dimension3D dimension;

    public Vector3 prototypeCubePosition;
    public Vector3 prototypeCubeRotation; // doesnt work correctly TODO fix

    // Start is called before the first frame update
    void Start()
    {
        dimension = new Dimension3D((int)dimensionAsVector.x, (int)dimensionAsVector.y, (int)dimensionAsVector.z);

        prototypeCube = new Cube(prototypeCubePosition, Quaternion.Euler(prototypeCubeRotation), dimension);

        if (fieldPrefab)
        {
            prototypeCube.instantiateObjects(fieldPrefab);
        }
    }

    // Update is called once per frame
    void Update()
    {
           
    }
}
