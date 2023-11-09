using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject (aka Data Class), which includes every game mode parameter.
/// <br/> To create a new GameMode, Right-Click in Project Window and select Create -> ScriptableObjects -> GameMode.
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameMode", order = 1)]
public class GameMode : ScriptableObject
{
    /// <summary>
    /// Specifies if dimensions should be randomly generated.
    /// <br/> [dimension] specifies the maximum possible dimension.
    /// </summary>
    public bool dimensionsAreRandom = false;

    /// <summary>
    /// Specifies if all 3 dimensions of the cube should be the same.
    /// E.g. 4x4x4
    /// <br/> If the specified [dimension] is not "square", then the a random dimension will be applied to all. E.g. 5x7x4 will turn into 5x5x5, 7x7x7 or 4x4x4.
    /// </summary>
    public bool cubeIsSquare = false;

    /// <summary>
    /// Specifies dimension of the cube. Floats will be casted to integers.
    /// <br/> If [dimensionsAreRandom] is true, then [dimension] specifies the maximum possible dimension. 
    /// </summary>
    public Vector3 dimension = new Vector3(Dimension3D.MIN, Dimension3D.MIN, Dimension3D.MIN);

    /// <summary>
    /// Specifies how fast the snake should move
    /// </summary>
    public float speedFactor = 1;

    public void Set(GameMode mode)
    {
        dimensionsAreRandom = mode.dimensionsAreRandom;
        cubeIsSquare = mode.cubeIsSquare;
        dimension = mode.dimension;
        speedFactor = mode.speedFactor;
    }

    public static GameMode CreateRandomGameMode()
    {
        GameMode mode = ScriptableObject.CreateInstance<GameMode>();

        mode.cubeIsSquare = Random.Range(0, 1) > 0.5 ? true : false;
        mode.dimensionsAreRandom = Random.Range(0, 1) > 0.5 ? true : false;

        // restrict to half the maximum, as that is barely playable and only for very brave people
        int x = Random.Range(Dimension3D.MIN, Dimension3D.MAX / 2);
        int y = Random.Range(Dimension3D.MIN, Dimension3D.MAX / 2);
        int z = Random.Range(Dimension3D.MIN, Dimension3D.MAX / 2);
        mode.dimension = new Vector3(x, y, z);

        mode.speedFactor = Random.Range(0.5F, 1);

        return mode;
    }
}
