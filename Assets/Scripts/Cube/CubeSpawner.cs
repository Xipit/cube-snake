using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public GameObject CubePreset;
    public GameObject FieldPrefab;
    public GameObject TunnelPrefab;

    public Vector3 DimensionAsVector; // only for editing in Unity Editor
    public Dimension3D Dimension;

    // Start is called before the first frame update
    void Start()
    {
        if(!CubePreset || !FieldPrefab || !TunnelPrefab)
        {
            Debug.LogError("Prefabs have not been set on Cubespawner, cant spawn cube!");
            return;
        }

        if (this.CubePreset.TryGetComponent<CubeDirector>(out CubeDirector director)
            && (director == null || (director != null && !director.enabled)))
        {
            Debug.LogError("CubePreset Prefab does not have necessary CubeDirector Script attached or enabled!");
            return;
        }

        Dimension = Dimension3D.fromVector(DimensionAsVector);

        if(!Dimension.IsViableForCube())
        {
            Debug.LogWarning("Specified DimensionAsVector is not suitable to create a Cube. All dimensions need to be > 0.");
        }

        SpawnCube(Dimension, FieldPrefab, TunnelPrefab); 
    }


    GameObject SpawnCube(Dimension3D dimension, GameObject fieldPrefab, GameObject tunnelPrefab)
    {
        Cube cube = new Cube(dimension);

        GameObject cubeGameObject = InstantiateManager.Instance.InstantiateGameObject(new Vector3(0,0,0), Quaternion.identity, CubePreset);

        CubeDirector director = cubeGameObject.GetComponent<CubeDirector>();
        director.AssignCubeAndPrefabs(cube, fieldPrefab, tunnelPrefab); 
        director.InstantiateCubeContent();

        return cubeGameObject;
    }
}
