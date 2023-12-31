using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
#nullable enable

public class CubeSpawner : MonoBehaviour
{
    public GameObject CubePreset;
    [Header("Field Prefabs")]
    [Tooltip("0: Front, 1: Back, 2: Right, 3: Left, 4: Up, 5: Right")]
    public GameObject[] FieldPrefabs = new GameObject[6];
    public GameObject TunnelPrefab;
    [Tooltip("0: Front, 1: Back, 2: Right, 3: Left, 4: Up, 5: Right")]
    public GameObject[] DecorationPrefabs = new GameObject[6];

    [Header("Options")]
    [Range(0, 100)]
    public int DecorationPercentage = 0;

    [Header("Snack")]
    public GameObject[] snackPrefabs;

    public Snake.Snake snake;

    private Dimension3D Dimension;

    public static CubeSpawner Instance { get; private set; }

    public Cube? SpawnCube(GameMode mode)
    {
        DetermineValuesFromGameMode(mode);

        if (!IsReadyForSpawn(Dimension))
        {
            return null;
        }

        Cube cube = new Cube(Dimension);

        GameObject cubeGameObject = InstantiateManager.Instance.InstantiateGameObject(new Vector3(0, 0, 0), Quaternion.identity, CubePreset);
        cubeGameObject.transform.parent = RotationManager.Instance.transform;

        CubeDirector director = cubeGameObject.GetComponent<CubeDirector>();
        
        director.AssignCubeAndPrefabs(cube, FieldPrefabs, TunnelPrefab, DecorationPrefabs, DecorationPercentage);
        director.InstantiateCubeContent();

        Snack snack = new Snack(snackPrefabs, cube);

        if(!snake){
            Debug.LogError("Snake reference has not been set. Cant start snake");
            return null;
        }

        RotationManager.Instance.transform.localScale = DetermineCubeObjectScaleToFitInCamera(Dimension);

        snake.StartSnake(cube, CubeSideCoordinate.Front, snack, mode);

        return cube;
    }

    private void DetermineValuesFromGameMode(GameMode mode)
    {
        if (mode.dimensionsAreRandom)
        {
            Dimension3D randomDimension = new Dimension3D(
                Random.Range(3, mode.dimension.x),
                Random.Range(3, mode.dimension.y),
                Random.Range(3, mode.dimension.z));

            Dimension = randomDimension;
        }else
        {
            Dimension = Dimension3D.fromVector(mode.dimension);
        }

        if (mode.cubeIsSquare)
        {
            Dimension = new Dimension3D(Dimension.GetRandomDimension());
        }
    }

    private Vector3 DetermineCubeObjectScaleToFitInCamera(Dimension3D dimension)
    {
        float highestDimension = dimension.GetHighestDimension();

        float scale = Mathf.Min(1, (float)(6 / highestDimension) + 0.12F);

        Debug.Log("Scale for rotationreference: " + scale);

        return new Vector3(scale, scale, scale);
    }

    private bool IsReadyForSpawn(Dimension3D dimension)
    {
        for (int i = 0; i < 6; i++)
        {
            if (FieldPrefabs[i] == null)
            {
                Debug.LogError("FieldPrefabs have not been filled on CubeSpawner, cant spawn cube!");
                return false;
            }
            if (DecorationPrefabs[i] == null)
            {
                Debug.LogError("DecorationPrefabs have not been filled on CubeSpawner, cant spawn cube!");
                return false;
            }
        }


        if (!CubePreset || FieldPrefabs.Length != 6 || DecorationPrefabs.Length != 6 || !TunnelPrefab)
        {
            Debug.LogError("Prefabs have not been set on Cubespawner or Prefab Arrays arent sized to 6, cant spawn cube!");
            return false;
        }

        if (this.CubePreset.TryGetComponent<CubeDirector>(out CubeDirector director)
            && (director == null || (director != null && !director.enabled)))
        {
            Debug.LogError("CubePreset Prefab does not have necessary CubeDirector Script attached or enabled!");
            return false;
        }

        if (!dimension.IsViableForCube())
        {
            Debug.LogError("Specified DimensionAsVector is not suitable to create a Cube. All dimensions need to be > 3.");
            return false;
        }

        if(snackPrefabs.Length < 1)
        {
            Debug.LogError("No snack prefab specified. Assign at least 1 snack prefab!");
            return false;
        }

        return true;
    }


    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
}
