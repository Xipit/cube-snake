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

    public Vector3 GetPositionInCubeSide(float cubeScale)
    {
        return new Vector3(
            H + cubeScale / 2,
            0,
            V + cubeScale / 2);
    }

    public override string ToString()
    {
        return "[H: " + H + ", V: " + V + "]";
    }
}