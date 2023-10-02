using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents 1 side of a cube
/// <br />
/// </summary>
public class CubeSide 
{
    private Dimension2D dimension;
    public CubeField[,] fields;

    public Vector3 positionInCube;
    public Quaternion rotationInCube;

    // neighbour sides
    private CubeSide northSide;
    private CubeSide eastide;
    private CubeSide southSide;
    private CubeSide westSide;


    public CubeSide(Dimension2D dimension, Vector3 positionInCube, Quaternion rotationInCube)
    {
        this.dimension = dimension;
        this.positionInCube = positionInCube;
        this.rotationInCube = rotationInCube;

        fields = CreateFields(dimension);
    }

    private CubeField[,] CreateFields (Dimension2D dimension)
    {
        CubeField[,] fields = new CubeField[dimension.A, dimension.B];

        for (int a = 0; a < dimension.A; a++)
        {
            for (int b = 0; b < dimension.B; b++)
            {
                fields[a, b] = new CubeField(a, b);
            }
        }

        return fields;
    }

    public GameObject instantiateObjects(Vector3 positionInWorld, Quaternion rotationInWorld, float size, GameObject fieldPrefab)
    {
        Vector3 sidePosition = positionInWorld + positionInCube;
        Quaternion sideRotation = rotationInWorld * rotationInCube;

        GameObject side = new GameObject("Side (" + dimension.A + ", " + dimension.B + ")");
        side.transform.position = sidePosition;
        side.transform.rotation = sideRotation;
            
        for (int a = 0; a < dimension.A; a++)
        {
            for (int b = 0; b < dimension.B; b++)
            {
                if(fields[a,b].isTunnel == true)
                {
                    break;
                }

                Vector3 positionInSide = new Vector3(
                    a + size / 2,
                    0,
                    b + size / 2);

                Vector3 fieldPosition = positionInWorld + (rotationInWorld * (positionInCube + (rotationInCube * positionInSide)));

                GameObject field = fields[a, b].instantiateObject(fieldPosition, sideRotation, fieldPrefab, side);

                field.name = "Field " + "[" + a + ", " + b + "]";
            }
        }

        return side;
    }
}
