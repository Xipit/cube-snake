using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePoint 
{
    public CubeFieldCoordinate FieldCoordinate { get; }
    public CubeSideCoordinate SideCoordinate { get; }

    public CubePoint(CubeSideCoordinate sideCoordinate, CubeFieldCoordinate fieldCoordinate)
    {
        FieldCoordinate = fieldCoordinate;
        SideCoordinate = sideCoordinate;
    }

    public bool IsEqual(CubePoint point)
    {
        return point.FieldCoordinate.H == this.FieldCoordinate.H
            && point.FieldCoordinate.V == this.FieldCoordinate.V
            && point.SideCoordinate == this.SideCoordinate;
    }
}
