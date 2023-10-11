using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data Class to store and type the coordinate of a CubeField. Functionally similar to Dimension2D.
/// </summary>
public class CubeFieldCoordinate
{
    public int H { get; }
    public int V { get; }

    public CubeFieldCoordinate(int h, int v)
    {
        this.H = h;
        this.V = v;
    }
}

// the following methods are invoked by e.g. CubeFieldCoordinate.GetPositionInCubeSide(cubeScale);
static class CubeFieldCoordinateMethods
{
    public static Vector3 GetPositionInCubeSide(this CubeFieldCoordinate coordinate, float cubeScale)
    {
        return new Vector3(
            coordinate.H + cubeScale / 2,
            0,
            coordinate.V + cubeScale / 2);
    }
}