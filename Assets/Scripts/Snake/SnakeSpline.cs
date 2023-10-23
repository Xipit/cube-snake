#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

namespace Snake
{
    public class SnakeSpline : MonoBehaviour
    {
        private CubeSideCoordinate startSide;
        private List<CubePoint> pointsOfSnake;
        private Cube cube;
        
        public SplineContainer splinePath;

        public float stepInterval;
    
        private MovementDirection stepDirection;
        public MovementDirection inputDirection;

        private DirectionOnCubeSide inputUpDirection;

        public void InitializeDataForSnake(Cube cube)
        {
            this.cube = cube;

            startSide = CubeSideCoordinate.Front;

            SpawnSnake.Instance.SpawnSnakeOnCube(cube, splinePath, startSide);
            pointsOfSnake = SpawnSnake.Instance.GetStartPoints();

            stepDirection = inputDirection;
            // it is posVert because the startSide is the front side
            inputUpDirection = DirectionOnCubeSide.posVert;

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
            
            // position of the center of a field
            Vector3 positionInSide = nextPoint.FieldCoordinate.GetPositionInCubeSide(cube.Scale);

            DirectionOnCubeSide stepDirectionOnCubeSide = stepDirection.ToLocalDirectionOnCubeSide(inputUpDirection);
            
            // set the position to the edge of a field to which the snake moves
            positionInSide += stepDirectionOnCubeSide switch
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

            Vector3 rotationOnSide;
            DirectionOnCubeSide stepDirectionOnCubeSide = stepDirection.ToLocalDirectionOnCubeSide(inputUpDirection);
            
            switch (nextPoint.SideCoordinate)
            {
                case CubeSideCoordinate.Front:
                case CubeSideCoordinate.Right:
                case CubeSideCoordinate.Back:
                case CubeSideCoordinate.Left:
                    rotationOnSide = stepDirectionOnCubeSide switch
                    {
                        DirectionOnCubeSide.negHor => new Vector3(-90, 90, 270),
                        DirectionOnCubeSide.posHor => new Vector3(90, 90, 90),
                        DirectionOnCubeSide.negVert => new Vector3(-180, 0, 0),
                        DirectionOnCubeSide.posVert => new Vector3(0, 0, 0),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;
                case CubeSideCoordinate.Up:
                    rotationOnSide = stepDirectionOnCubeSide switch
                    {
                        DirectionOnCubeSide.negHor => new Vector3(0, -90, 0),
                        DirectionOnCubeSide.posHor => new Vector3(0, 90, 0),
                        DirectionOnCubeSide.negVert => new Vector3(-180, 0, 0),
                        DirectionOnCubeSide.posVert => new Vector3(0, 0, 0),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;
                case CubeSideCoordinate.Down:
                    rotationOnSide = stepDirectionOnCubeSide switch
                    {
                        DirectionOnCubeSide.negHor => new Vector3(0, 90, 0),
                        DirectionOnCubeSide.posHor => new Vector3(0, -90, 0),
                        DirectionOnCubeSide.negVert => new Vector3(-180, 0, 0),
                        DirectionOnCubeSide.posVert => new Vector3(0, 0, 0),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;
                default:
                    rotationOnSide = Vector3.zero;
                    break;
            }

            Vector3 rotation = sideRotation.eulerAngles + rotationOnSide;

            return Quaternion.Euler(rotation);
        }

        private CubePoint GetNextPointOnCube()
        {
            CubePoint lastPoint = pointsOfSnake.Last();

            DirectionOnCubeSide stepDirectionOnCubeSide = stepDirection.ToLocalDirectionOnCubeSide(inputUpDirection);

            CubePoint nextPoint = stepDirectionOnCubeSide switch
            {
                DirectionOnCubeSide.negHor => GetNextPoint(lastPoint, new Vector2(-1, 0), stepDirectionOnCubeSide),
                DirectionOnCubeSide.posHor => GetNextPoint(lastPoint, new Vector2(1, 0), stepDirectionOnCubeSide),
                DirectionOnCubeSide.negVert => GetNextPoint(lastPoint, new Vector2(0, -1), stepDirectionOnCubeSide),
                DirectionOnCubeSide.posVert => GetNextPoint(lastPoint, new Vector2(0, 1), stepDirectionOnCubeSide),
                _ => new CubePoint(CubeSideCoordinate.Front, new CubeFieldCoordinate(0, 0))
            };

            return nextPoint;
        }

        private bool PointIsOnSameSide(CubePoint lastPoint, Vector2 vectorToNextPoint)
        {
            int maxH = cube.Sides[(int)lastPoint.SideCoordinate].Dimension.H;
            int maxV = cube.Sides[(int)lastPoint.SideCoordinate].Dimension.V;

            int H = lastPoint.FieldCoordinate.H;
            int V = lastPoint.FieldCoordinate.V;

            if (H + vectorToNextPoint.x < maxH &&
                H + vectorToNextPoint.x >= 0 &&
                V + vectorToNextPoint.y < maxV &&
                V + vectorToNextPoint.y >= 0)
            {
                return true;
            }

            return false;
        }

        private CubePoint GetNextPoint(CubePoint lastPoint, Vector2 vectorToNextPoint, DirectionOnCubeSide lastDirectionOnCubeSide)
        {
            if (PointIsOnSameSide(lastPoint, vectorToNextPoint))
            {
                return GetPointOnSide(lastPoint, vectorToNextPoint);
            }
            else
            {
                return GetPointOnNextSide(lastPoint, lastDirectionOnCubeSide);
            }
        }

        private CubePoint GetPointOnSide(CubePoint lastPoint, Vector2 vectorToNextPoint)
        {
            CubeFieldCoordinate nextFieldCoordinate =
                new CubeFieldCoordinate(lastPoint.FieldCoordinate.H + (int)vectorToNextPoint.x, 
                                        lastPoint.FieldCoordinate.V + (int)vectorToNextPoint.y);

            return new CubePoint(lastPoint.SideCoordinate, nextFieldCoordinate);
        }

        private CubePoint GetPointOnNextSide(CubePoint lastPoint, DirectionOnCubeSide lastDirectionOnCubeSide)
        {
            (CubeSideCoordinate neighborCoordinate, DirectionOnCubeSide neighborDirection) nextSide =
                lastPoint.SideCoordinate.GetNeighborWithDirection(lastDirectionOnCubeSide);

            CubePoint nextPoint = lastPoint.GetPointOnNeighbour(lastDirectionOnCubeSide, cube);
                
            inputUpDirection = stepDirection.GetInputUpAsDirectionOnCubeSide(nextSide.neighborDirection);

            CubeSnakeHolderManager.Instance.RotateCubeSnakeHolder(stepDirection);

            return nextPoint;
        }
    }
}
