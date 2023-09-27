using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data Class used to store the dimension of a cube.
/// Essentially an integer Vector3.
/// </summary>
public class Dimension3D 
{
    public int X { get; }
    public int Y { get; }
    public int Z { get; }


    public Dimension3D(int x, int y, int z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }
}

/// <summary>
/// Data Class used to store the dimension of a cubeSide.
/// Essentially an integer Vector2.
/// </summary>
public class Dimension2D
{
    public int A { get; }
    public int B { get; }
    
    public Dimension2D(int a, int b)
    {
        this.A = a;
        this.B = b;
    }
}