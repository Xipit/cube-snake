#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

namespace Snake
{
    public class SnakeSpline : MonoBehaviour
    {
        // public GameObject CubePreset;
        // public CubeSpawner CubeSpawner;
        // private CubeDirector cubeDirector;
        
        public CubeSideCoordinate startSide;
        private List<CubePoint> pointsOfSnake;
        private Cube cube;
        
        public SplineContainer splinePath;

        public float stepInterval;
    
        private MovementDirection stepDirection;
        public MovementDirection inputDirection;

        private DirectionOnCubeSide currentInputUpDirection;

        public void InitializeDataForSnake(Cube cube)
        {
            this.cube = cube;

            SpawnSnake.Instance.SpawnSnakeOnCube(cube, splinePath, startSide);
            pointsOfSnake = SpawnSnake.Instance.GetStartPoints();

            stepDirection = inputDirection;
            // it is posVert because the startSide is the front side
            currentInputUpDirection = DirectionOnCubeSide.posVert;

            InvokeRepeating(nameof(DetermineNextStepDirection), stepInterval * 0.75f, stepInterval);
            InvokeRepeating(nameof(UpdateSpline), stepInterval, stepInterval);
        }
    
        private void Update()
        {
            inputDirection = InputManager.Instance.GetPlayerInput(stepDirection) ?? inputDirection;
        }

        /// <summary>
        /// Set position of next spline point, according to current inputDirection.
        /// </summary>
        private void DetermineNextStepDirection()
        {
            if (stepDirection == inputDirection)
            {
                return;
            }
            
            stepDirection = inputDirection;
            
            CubePoint lastPoint = pointsOfSnake.Last();

            splinePath.Spline.SetKnot(splinePath.Spline.Count - 1, GetBezierKnot(lastPoint));
        }

        /// <summary>
        /// Delete tail of splineArray and spawn new head, according to stepDirection
        /// </summary>
        private void UpdateSpline()
        {
            // add a new knot to the spline
            // head == end of splineArray
            // tail == start of splineArray

            CubePoint nextPoint = GetNextPointOnCube();
 
            splinePath.Spline.Add(GetBezierKnot(nextPoint));
            splinePath.Spline.RemoveAt(0);
            
            pointsOfSnake.Add(nextPoint);
            pointsOfSnake.RemoveAt(0);
        }

        private BezierKnot GetBezierKnot(CubePoint cubePoint)
        {
            BezierKnot bezierKnot = new BezierKnot();

            bezierKnot.Position = GetNextPositionVector(cubePoint);
            bezierKnot.Rotation = GetNextRotation(cubePoint);
            bezierKnot.TangentIn = new float3(0, 0, -0.33f);
            bezierKnot.TangentOut = new float3(0, 0, 0.33f);

            return bezierKnot;
        }

        private Vector3 GetNextPositionVector(CubePoint nextPoint)
        {
            Vector3 positionInCube = nextPoint.SideCoordinate.GetPositionInCube(cube.Dimension, cube.Scale); 
            Quaternion rotationInCube = nextPoint.SideCoordinate.GetRotationInCube();
            
            // position in the center of a field
            Vector3 positionInSide = nextPoint.FieldCoordinate.GetPositionInCubeSide(cube.Scale);

            DirectionOnCubeSide directionOnCubeSide =
                CubeSnakeHolderManager.Instance.TranslateInputDirectionToDirectionOnSide(stepDirection,
                    currentInputUpDirection);
            
            // set position to the edge of a field
            positionInSide += directionOnCubeSide switch
            {
                DirectionOnCubeSide.negHor => new Vector3(-0.5f * cube.Scale, 0, 0),
                DirectionOnCubeSide.posHor => new Vector3(0.5f * cube.Scale, 0, 0),
                DirectionOnCubeSide.negVert => new Vector3(0, 0, -0.5f * cube.Scale),
                DirectionOnCubeSide.posVert => new Vector3(0, 0, 0.5f * cube.Scale),
                _ => new Vector3(0, 0, 0)
            };

            return positionInCube + rotationInCube * positionInSide;
        }

        private Quaternion GetNextRotation(CubePoint nextPoint)
        {
            Quaternion sideRotation = nextPoint.SideCoordinate.GetRotationInCube();

            /// TODO get rotation in stepDirection
            
            return sideRotation;
        }

        private CubePoint GetNextPointOnCube()
        {
            CubePoint currentPoint = pointsOfSnake.Last();
            CubePoint nextPoint;

            DirectionOnCubeSide inputDirectionOnCubeSide = CubeSnakeHolderManager.Instance.TranslateInputDirectionToDirectionOnSide(stepDirection, currentInputUpDirection);

            nextPoint = inputDirectionOnCubeSide switch
            {
                DirectionOnCubeSide.negHor => GetNextPoint(currentPoint, new Vector2(-1, 0), inputDirectionOnCubeSide),
                DirectionOnCubeSide.posHor => GetNextPoint(currentPoint, new Vector2(1, 0), inputDirectionOnCubeSide),
                DirectionOnCubeSide.negVert => GetNextPoint(currentPoint, new Vector2(0, -1), inputDirectionOnCubeSide),
                DirectionOnCubeSide.posVert => GetNextPoint(currentPoint, new Vector2(0, 1), inputDirectionOnCubeSide),
                _ => throw new ArgumentOutOfRangeException()
            };

            return nextPoint;
        }

        private bool PointIsOnSameSide(CubePoint currentPoint, Vector2 vectorToNextPoint)
        {
            int maxH = cube.Sides[(int)currentPoint.SideCoordinate].Dimension.H;
            int maxV = cube.Sides[(int)currentPoint.SideCoordinate].Dimension.V;

            int H = currentPoint.FieldCoordinate.H;
            int V = currentPoint.FieldCoordinate.V;

            if (H + vectorToNextPoint.x < maxH &&
                H + vectorToNextPoint.x >= 0 &&
                V + vectorToNextPoint.y < maxV &&
                V + vectorToNextPoint.y >= 0)
            {
                return true;
            }

            return false;
        }

        private CubePoint GetNextPoint(CubePoint currentPoint, Vector2 vectorToNextPoint, DirectionOnCubeSide currentDirectionOnCubeSide)
        {
            if (PointIsOnSameSide(currentPoint, vectorToNextPoint))
            {
                return GetPointOnSide(currentPoint, vectorToNextPoint);
            }
            else
            {
                return GetPointOnNextSide(currentPoint, currentDirectionOnCubeSide);
            }
        }

        private CubePoint GetPointOnSide(CubePoint currentPoint, Vector2 vectorToNextPoint)
        {
            CubeFieldCoordinate nextFieldCoordinate =
                new CubeFieldCoordinate(currentPoint.FieldCoordinate.H + (int)vectorToNextPoint.x, 
                                        currentPoint.FieldCoordinate.V + (int)vectorToNextPoint.y);

            return new CubePoint(currentPoint.SideCoordinate, nextFieldCoordinate);
        }

        private CubePoint GetPointOnNextSide(CubePoint currentPoint, DirectionOnCubeSide currentDirectionOnCubeSide)
        {
            (CubeSideCoordinate neighborCoordinate, DirectionOnCubeSide neighborDirection) nextSide =
                currentPoint.SideCoordinate.GetNeighborWithDirection(currentDirectionOnCubeSide);

            CubePoint nextPoint = currentPoint.GetPointOnNeighbour(currentDirectionOnCubeSide, cube);
                
            currentInputUpDirection =
                CubeSnakeHolderManager.Instance.GetInputUpAsDirectionOnCubeSide(stepDirection, nextSide.neighborDirection);

            CubeSnakeHolderManager.Instance.RotateCubeSnakeHolder(stepDirection);

            return nextPoint;
        }
    }
}
