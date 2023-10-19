using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SpawnSnake : MonoBehaviour
{
    public static SpawnSnake Instance { get; private set; }

    public void SpawnSnakeOnCube(CubeSpawner cube, SplineContainer splinePath, CubeSideCoordinate startSide)
    {
        Vector3 positionInCube = startSide.GetPositionInCube(cube.Dimension, 1f); //TODO cubeScale -> variable
        Quaternion rotationInCube = startSide.GetRotationInCube();

        CubeFieldCoordinate startField = startSide.GetDimension2D(cube.Dimension).GetRandomFieldCoordinate();
        Vector3 positionInSide = startField.GetPositionInCubeSide(1f); // TODO change cubescale to variable

        Vector3 startPositionOfSnake = positionInCube + rotationInCube * positionInSide;
        
        
        BezierKnot startKnot = new BezierKnot();
        startKnot.Position = startPositionOfSnake;
        startKnot.Rotation = rotationInCube;
        startKnot.TangentIn = new float3(0, 0, -0.33f);
        startKnot.TangentOut = new float3(0, 0, 0.33f);

        splinePath.Spline.Add(startKnot);


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
