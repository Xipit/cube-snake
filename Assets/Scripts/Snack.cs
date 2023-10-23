using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snack 
{
    public CubePoint Position;

    public GameObject[] Prefabs;

    private Cube Cube;

    public Snack(GameObject[] prefabs, Cube cube, CubePoint[] snakePoints)
    {
        Prefabs = prefabs;
        Cube = cube;

        AssignNewPosition(snakePoints);
    }

    // TODO should be called when snake eats a snack
    public void AssignNewPosition(CubePoint[] snakePoints)
    {
        Position = Cube.GetRandomPoint(Cube.Dimension, snakePoints);
        InstantiateContent();
    }

    private void InstantiateContent()
    {
        // TODO instantiate random prefab on CubePoint position
    }
}
