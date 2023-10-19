using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SpawnSnake : MonoBehaviour
{
    public static SpawnSnake Instance { get; private set; }

    private CubePoint startPoint;

    public void SpawnSnakeOnCube(Cube cube, SplineContainer splinePath, CubeSideCoordinate startSide)
    {
        if (cube == null)
        {
            return;
        }
        
        Vector3 positionInCube = startSide.GetPositionInCube(cube.Dimension, 1f); //TODO cubeScale -> variable
        Quaternion rotationInCube = startSide.GetRotationInCube();

        CubeFieldCoordinate startFieldCoordinate = startSide.GetDimension2D(cube.Dimension).GetRandomFieldCoordinate();
        Vector3 positionInSide = startFieldCoordinate.GetPositionInCubeSide(1f); // TODO change cubescale to variable

        Vector3 startPositionOfSnake = positionInCube + rotationInCube * positionInSide;

        BezierKnot startKnot = new BezierKnot();
        startKnot.Position = startPositionOfSnake;
        startKnot.Rotation = rotationInCube;
        startKnot.TangentIn = new float3(0, 0, -0.33f);
        startKnot.TangentOut = new float3(0, 0, 0.33f);

        splinePath.Spline.Add(startKnot);


        startPoint = new CubePoint(startSide, startFieldCoordinate);
    }

    public CubePoint GetStartPoint()
    {
        return startPoint;
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
