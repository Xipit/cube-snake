using System.Collections;
using System.Collections.Generic;
using UnityEngine;
# nullable enable

/// <summary>
/// Every Cube GameObject has this script, which contains an instance of Cube.
/// </summary>
public class CubeDirector : MonoBehaviour
{
    public Cube? Cube;

    public GameObject[]? FieldPrefabs;
    public GameObject? TunnelPrefab;
    public GameObject[]? DecorationPrefabs;

    public int DecorationPercentage;

    public void AssignCubeAndPrefabs(Cube cube, GameObject[] fieldPrefabs, GameObject tunnelPrefab, GameObject[] decorationPrefabs, int decorationPercentage)
    {
        this.Cube = cube;
        this.FieldPrefabs = fieldPrefabs;
        this.TunnelPrefab = tunnelPrefab;
        this.DecorationPrefabs = decorationPrefabs;
        this.DecorationPercentage = decorationPercentage;

        this.name = "Cube (" + cube.Dimension.X + ", " + cube.Dimension.Y + ", " + cube.Dimension.Z + ")";
    }

    public void InstantiateCubeContent()
    {
        // ensure Cube hasn't been instantiated already
        if(this.transform.childCount > 0)
        {
            Debug.LogWarning("Tried to instantiateCubeContent() with already existing CubeContent.");
            return;
        }
        
        if (Cube == null || FieldPrefabs == null || FieldPrefabs.Length != 6 || DecorationPrefabs == null || DecorationPrefabs.Length != 6 || !TunnelPrefab)
        {
            Debug.LogError("Tried to instantiateCubeContent() before assigning cube or prefabs!");
            return;
        }


        Cube.InstantiateObjects(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
