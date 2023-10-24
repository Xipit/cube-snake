using System.Collections;
using System.Collections.Generic;
using Snake;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public GameObject CubePreset;
    public GameObject FieldPrefab;
    public GameObject TunnelPrefab;

    public GameObject CubeSnakeHolder;

    public Vector3 DimensionAsVector; // only for editing in Unity Editor
    public Dimension3D Dimension;

    private CubeDirector director;

    public Snake.Snake snake;

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

        Cube cube = SpawnCube(Dimension, FieldPrefab, TunnelPrefab);

        if (!snake)
        {
            Debug.LogError("Snake reference has not been set. Cant start snake!");
            return;
        }

        snake.StartSnake(cube, CubeSideCoordinate.Front);
    }


    Cube SpawnCube(Dimension3D dimension, GameObject fieldPrefab, GameObject tunnelPrefab)
    {
        Cube cube = new Cube(dimension);

        GameObject cubeGameObject = InstantiateManager.Instance.InstantiateGameObjectAsChild(new Vector3(0,0,0), Quaternion.identity, CubePreset, CubeSnakeHolder.transform);

        director = cubeGameObject.GetComponent<CubeDirector>();
        director.AssignCubeAndPrefabs(cube, fieldPrefab, tunnelPrefab); 
        director.InstantiateCubeContent();

        return cube;
    }
}
