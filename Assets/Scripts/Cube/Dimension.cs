using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data Class used to store the dimension of a cube.
/// Essentially an integer Vector3, where all dimension are >= 0.
/// </summary>
public class Dimension3D 
{
    public int X { get; }
    public int Y { get; }
    public int Z { get; }

    /*
     *  ^ Z
     *  |
     *  |   Y
     *  | /    
     *   /     X
     *   ------> 
     */

    public Dimension3D(int x, int y, int z)
    {
        // ensure non-negative values
        this.X = Mathf.Max(0, x);
        this.Y = Mathf.Max(0, y);
        this.Z = Mathf.Max(0, z);
    }

    public static Dimension3D fromVector(Vector3 vector)
    {
        return new Dimension3D((int)vector.x, (int)vector.y, (int)vector.z);
    }

    public Vector3 ToVector3()
    {
        return new Vector3(X, Y, Z);
    }

    public Vector3 MultiplyBy(Vector3 vector)
    {
        return new Vector3(vector.x * X, vector.y * Y, vector.z * Z);
    }

    public bool IsViableForCube()
    {
        return X > 0 && Y > 0 && Z > 0;
    }
}

/// <summary>
/// Data Class used to store the dimension of a cubeSide.
/// Essentially an integer Vector3, where all dimension are >= 0.
/// </summary>
public class Dimension2D
{
    public int H { get; }
    public int V { get; }

    /*
     *  ^ V (ertical)
     *  |
     *  |   H (orizontal)
     *   ---> 
     */

    public Dimension2D(int a, int b)
    {
        // ensure non-negative values
        this.H = Mathf.Max(0, a);
        this.V = Mathf.Max(0, b);
    }

    public CubeFieldCoordinate GetRandomFieldCoordinate()
    {
        return new CubeFieldCoordinate(Random.Range(0, H), Random.Range(0, V));
    }

    public bool IsPointInDirectionInDimension(CubePoint point, DirectionOnCubeSide direction)
    {
        Vector2 vectorToNextPoint = direction.ToVector2();

        int maxH = this.H;
        int maxV = this.V;

        int H = point.FieldCoordinate.H;
        int V = point.FieldCoordinate.V;

        if (H + vectorToNextPoint.x < maxH &&
            H + vectorToNextPoint.x >= 0 &&
            V + vectorToNextPoint.y < maxV &&
            V + vectorToNextPoint.y >= 0)
        {
            return true;
        }

        return false;
    }

    public CubeFieldCoordinate GetFieldInLeftCenter()
    {
        int centerV = (int)Mathf.Floor((float)this.V / 2);

        return new CubeFieldCoordinate(0, centerV);
    }
}