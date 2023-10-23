using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SpawnSnake : MonoBehaviour
{
    public static SpawnSnake Instance { get; private set; }

    private List<CubePoint> startPoints;
    private DirectionOnCubeSide startDirection;

    public void SpawnSnakeOnCube(Cube cube, SplineContainer splinePath, CubeSideCoordinate startSide)
    {
        if (cube == null)
        {
            return;
        }

        startPoints = new List<CubePoint>();
        
        Vector3 positionInCube = startSide.GetPositionInCube(cube.Dimension, cube.Scale);
        Quaternion rotationInCube = startSide.GetRotationInCube();

        CubeFieldCoordinate startFieldCoordinate = GetStartFieldCoordinate(startSide.GetDimension2D(cube.Dimension).V);

        // First Knot
        splinePath.Spline.Add(AddKnotToPath(startFieldCoordinate, rotationInCube, positionInCube, cube.Scale, true));
        
        for (int i = 0; i < 3; i++)
        {
            CubeFieldCoordinate fieldCoordinate =
                new CubeFieldCoordinate(startFieldCoordinate.H + i, startFieldCoordinate.V);
            
            splinePath.Spline.Add(AddKnotToPath(fieldCoordinate, rotationInCube, positionInCube, cube.Scale));
            
            startPoints.Add(new CubePoint(startSide, fieldCoordinate));
        }
    }

    public List<CubePoint> GetStartPoints()
    {
        return startPoints;
    }

    private static CubeFieldCoordinate GetStartFieldCoordinate(int maxV)
    {
        int centerV = (int)Math.Floor((float)maxV / 2);
        return new CubeFieldCoordinate(0, centerV);
    }

    private static BezierKnot AddKnotToPath(CubeFieldCoordinate fieldCoordinate, Quaternion rotationInCube, Vector3 positionInCube, float scaleFactor, bool isFirstKnot = false)
    {
        Vector3 positionInSide = fieldCoordinate.GetPositionInCubeSide(scaleFactor);

        Vector3 startPositionOfSnake = isFirstKnot switch
        {
            true => positionInCube + rotationInCube * (positionInSide - new Vector3(0.5f * scaleFactor, 0, 0)),
            false => positionInCube + rotationInCube * (positionInSide + new Vector3(0.5f * scaleFactor, 0, 0))
        };

        Vector3 rotation = new Vector3(0, 90, 270);
        
        BezierKnot startKnot = new BezierKnot();
        startKnot.Position = startPositionOfSnake;
        startKnot.Rotation = Quaternion.Euler(rotation);
        startKnot.TangentIn = new float3(0, 0, -0.33f);
        startKnot.TangentOut = new float3(0, 0, 0.33f);

        return startKnot;
    }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
}
