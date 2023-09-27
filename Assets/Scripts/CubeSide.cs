using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents 1 side of a cube
/// <br />
/// </summary>
public class CubeSide 
{
    /*
     *  ^ B
     *  |
     *  |   A
     *   ---> 
     */

    private Dimension2D dimension;
    public CubeField[,] fields;


    // neighbour sides
    private CubeSide northSide;
    private CubeSide eastide;
    private CubeSide southSide;
    private CubeSide westSide;


    public CubeSide(Dimension2D dimension)
    {
        this.dimension = dimension;
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

    public void instantiateMeshes(Vector3 referencePosition, Quaternion referenceRotation, GameObject fieldPrefab)
    {
        for (int a = 0; a < dimension.A; a++)
        {
            for (int b = 0; b < dimension.B; b++)
            {
                if(fields[a,b].isTunnel == true)
                {
                    break;
                }

                /*
                 *  Vector3 pointA;
                    Vector3 pointB;
                    Quaternion rotationA;
                    Vector3 result = pointA + (pointB * rotationA);
                 */

                Vector3 position = referencePosition + (referenceRotation * new Vector3(a, 0, b));

                Debug.Log(position);
                Debug.Log(InstantiateManager.Instance);


                InstantiateManager.Instance.InstantiateGameObject(position, fieldPrefab);
            }
        }
    }
}
