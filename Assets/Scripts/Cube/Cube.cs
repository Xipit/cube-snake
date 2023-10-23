using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

/// <summary>
/// Represents a Cube and all its needed data and its CubeSides and CubeFields.
/// </summary>
public class Cube
{
    public float Scale;

    public Dimension3D Dimension { get; }

    public Tunnel[] Tunnels;
    public CubeSide[] Sides = new CubeSide[6];

    public Cube(Dimension3D dimension, float scale = 1f)
    {
        this.Scale = scale;

        this.Dimension = dimension;

        Tunnels = CreateTunnels(dimension);
        Sides = CreateSides(dimension, Tunnels);
    }

    // Note for the future:
    // when implementing generating multiple tunnels, you need to make sure that tunnels dont interfere with each other
    // i.e. have the same point
    private Tunnel[] CreateTunnels(Dimension3D dimension)
    {
        CubePoint pointA = GetRandomPoint(dimension);

        CubeSideCoordinate opposingSide = pointA.SideCoordinate.GetOpposingCubeSideCoordinate();
        CubeFieldCoordinate randomFieldOnOpposingSide = opposingSide.GetDimension2D(dimension).GetRandomFieldCoordinate();

        CubePoint pointB = new CubePoint(opposingSide, randomFieldOnOpposingSide);

        Tunnel tunnel = new Tunnel(pointA, pointB);


        return new Tunnel[] { tunnel };
    }

    private CubeSide[] CreateSides(Dimension3D dimension, Tunnel[] tunnels)
    {
        CubeSide[] sidesTemporary = new CubeSide[6];

        sidesTemporary[((int)CubeSideCoordinate.Front)] =    new CubeSide(CubeSideCoordinate.Front,   dimension, tunnels);
        sidesTemporary[((int)CubeSideCoordinate.Back)] =     new CubeSide(CubeSideCoordinate.Back,    dimension, tunnels);
        sidesTemporary[((int)CubeSideCoordinate.Right)] =    new CubeSide(CubeSideCoordinate.Right,   dimension, tunnels);
        sidesTemporary[((int)CubeSideCoordinate.Left)] =     new CubeSide(CubeSideCoordinate.Left,    dimension, tunnels);
        sidesTemporary[((int)CubeSideCoordinate.Up)] =       new CubeSide(CubeSideCoordinate.Up,      dimension, tunnels);
        sidesTemporary[((int)CubeSideCoordinate.Down)] =     new CubeSide(CubeSideCoordinate.Down,    dimension, tunnels);

        return sidesTemporary;
    }

    /// <summary>
    /// instantiates all needed 3D Meshes to represent this cube in 3D.
    /// </summary>
    public void InstantiateObjects(CubeDirector director)
    { 
        for (int i = 0; i < Sides.Length; i++)
        {
            if(Sides[i] != null)
            {
                Sides[i].InstantiateObjects(director, Dimension, Scale);
            }

        }
    }

    /// <summary>
    /// Generate a random point on the cube.
    /// </summary>
    public CubePoint GetRandomPoint(Dimension3D dimension)
    {
        CubeSideCoordinate randomSideCoordinate = (CubeSideCoordinate)Random.Range(0, 6);
        Dimension2D dimensionOfSide = randomSideCoordinate.GetDimension2D(dimension);

        return new CubePoint(randomSideCoordinate, dimensionOfSide.GetRandomFieldCoordinate());
    }

    /// <summary>
    /// Generate a random point on the cube, but avoiding certain points.
    /// </summary>
    /// <param name="pointsToAvoid">Array of CubePoints, which are not allowed to be generated.</param>
    public CubePoint GetRandomPoint(Dimension3D dimension, CubePoint[] pointsToAvoid)
    {
        bool shouldGenerateNewPoint = true;
        CubePoint randomPoint;

        do
        {
            shouldGenerateNewPoint = false;
            randomPoint = GetRandomPoint(dimension);

            foreach (CubePoint pointToAvoid in pointsToAvoid)
            {
                if (randomPoint.IsEqual(pointToAvoid))
                {
                    shouldGenerateNewPoint = true;
                }
            }

        } while (shouldGenerateNewPoint);

        return randomPoint;
    }
}