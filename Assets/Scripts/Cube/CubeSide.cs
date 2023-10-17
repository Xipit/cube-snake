using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents 1 side of a cube
/// <br />
/// </summary>
public class CubeSide 
{
    public Dimension2D Dimension { get; }
    public CubeField[,] Fields { get; }

    private CubeSideCoordinate CubeSideCoordinate { get; }

    public CubeSide(CubeSideCoordinate cubeSideCoordinate, Dimension3D cubeDimension, Tunnel[] tunnels)
    {
        this.CubeSideCoordinate = cubeSideCoordinate;
        this.Dimension = cubeSideCoordinate.GetDimension2D(cubeDimension);

        Fields = CreateFields(Dimension, tunnels);
    }

    private CubeField[,] CreateFields (Dimension2D dimension, Tunnel[] tunnels)
    {
        CubeField[,] fields = new CubeField[dimension.H, dimension.V];

        for (int h = 0; h < dimension.H; h++)
        {
            for (int v = 0; v < dimension.V; v++)
            {
                fields[h, v] = new CubeField(new CubeFieldCoordinate(h, v), this.CubeSideCoordinate, tunnels);
            }
        }

        return fields;
    }

    public GameObject InstantiateObjects(CubeDirector director, Dimension3D cubeDimension, float cubeScale)
    {
        Vector3 positionInCube = this.CubeSideCoordinate.GetPositionInCube(cubeDimension, cubeScale);
        Quaternion rotationInCube = this.CubeSideCoordinate.GetRotationInCube();

        GameObject sideObject = new GameObject("Side (" + Dimension.H + ", " + Dimension.V + ") " + this.CubeSideCoordinate);
        sideObject.transform.parent = director.transform;
        sideObject.transform.position = positionInCube;
        sideObject.transform.rotation = rotationInCube;
            
        for (int h = 0; h < Dimension.H; h++)
        {
            for (int v = 0; v < Dimension.V; v++)
            {
                GameObject prefab = Fields[h, v].Tunnel != null
                    ? director.TunnelPrefab
                    : director.FieldPrefab;


                Vector3 positionInSide = new CubeFieldCoordinate(h, v).GetPositionInCubeSide(cubeScale);

                Vector3 position = (positionInCube + (rotationInCube * positionInSide));

                GameObject field = Fields[h, v].InstantiateObject(sideObject, position, rotationInCube, prefab);
                field.name = "Field " + "[" + h + ", " + v + "]";
            }
        }

        return sideObject;
    }
}
