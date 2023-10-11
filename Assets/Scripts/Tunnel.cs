using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tunnel 
{
    CubePoint PointA;
    CubePoint PointB;

    public Tunnel (CubePoint pointA, CubePoint pointB)
    {
        PointA = pointA;
        PointB = pointB;
    }

    public bool HasPoint(CubePoint point)
    {
        return point.IsEqual(PointA) || point.IsEqual(PointB);
    }
}
