#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tunnel 
{
    public CubePoint PointA;
    public CubePoint PointB;
    public CubePoint? Entry = null; // could be either PointA or PointB --> each time a snake moves on a tunnel, this should be refreshed

    public bool HeadGoesThroughTunnel = false;

    public Tunnel (CubePoint pointA, CubePoint pointB)
    {
        PointA = pointA;
        PointB = pointB;
    }

    public bool HasPoint(CubePoint point)
    {
        return point.IsEqual(PointA) || point.IsEqual(PointB);
    }

    public CubePoint GetOtherCubePoint(CubePoint entry)
    {
        if (entry.IsEqual(PointA)) return PointB;
        if (entry.IsEqual(PointB)) return PointA;

        return entry;
    }
}
