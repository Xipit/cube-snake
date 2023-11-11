using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Data Class used to store the dimension of a cube.
/// Essentially an integer Vector3, where all dimension are >= 0.
/// </summary>
public class Dimension3D 
{
    public static int MAX = 50;
    public static int MIN = 3;

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
        // ensure all dimensions are above min
        this.X = Mathf.Clamp(x, MIN, MAX);
        this.Y = Mathf.Clamp(y, MIN, MAX);
        this.Z = Mathf.Clamp(z, MIN, MAX);
    }

    public Dimension3D(float x, float y, float z) : this((int)x, (int)y, (int)z) { }

    public Dimension3D(int xyz) : this(xyz, xyz, xyz) { }

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
        return X >= 3 && Y >= 3 && Z >= 3;
    }

    public int GetRandomDimension()
    {
        int random = Random.Range(0, 3);

        switch (random)
        {
            case 0:
                return X;

            case 1:
                return Y;

            case 2:
            default:
                return Z;
        }
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

    public CubeFieldCoordinate GetRandomFieldCoordinate(bool canTouchEdge = true)
    {
        if (canTouchEdge) return new CubeFieldCoordinate(Random.Range(0, H), Random.Range(0, V));
        else return GetRandomFieldCoordinateNotOnEdge();
    }

    public CubeFieldCoordinate GetRandomFieldCoordinateNotOnEdge()
    {
        return new CubeFieldCoordinate(Random.Range(1, (H - 1)), Random.Range(1, (V - 1)));
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