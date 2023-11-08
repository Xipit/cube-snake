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
    public Vector3 dimension;

    /// <summary>
    /// Specifies if all 6 sides of the cube are unique.
    /// <br/> If [sidesAreUnique] is false, then the style of a random side will be applied to all sides.
    /// </summary>
    public bool sidesAreUnique = true;

    public void Set(GameMode mode)
    {
        dimensionsAreRandom = mode.dimensionsAreRandom;
        cubeIsSquare = mode.cubeIsSquare;
        dimension = mode.dimension;
        sidesAreUnique = mode.sidesAreUnique;
    }
}
