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

        public float stepLength;
        public float stepInterval;
    
        private MovementDirection stepDirection;
        public MovementDirection inputDirection;
        
        /*private void Start()
        {
            Debug.Log("Awake is called");
            if (this.CubePreset.TryGetComponent<CubeDirector>(out CubeDirector director)
                && (director == null || (director != null && !director.enabled)))
            {
                Debug.LogError("CubePreset Prefab does not have necessary CubeDirector Script attached or enabled!");
                return;
            }

            Debug.Log(director.Cube.Dimension.X.ToString());
            cubeDirector = director;
            cube = cubeDirector.Cube;

            SpawnSnake.Instance.SpawnSnakeOnCube(CubeSpawner, splinePath, startSide);
            pointsOfSnake = new List<CubePoint>
            {
                SpawnSnake.Instance.GetStartPoint()
            };
            Debug.Log("SideCoordinate: " + pointsOfSnake.First().SideCoordinate + ", FieldCoordinate: " + pointsOfSnake.First().FieldCoordinate.H + " / " + pointsOfSnake.First().FieldCoordinate.V);

            stepDirection = inputDirection;
            
            InvokeRepeating(nameof(DetermineNextStepDirection), stepInterval * 0.75f, stepInterval);
            InvokeRepeating(nameof(UpdateSpline), stepInterval, stepInterval);
        }*/

        public void InitializeDataForSnake(Cube cube)
        {
            this.cube = cube;

            SpawnSnake.Instance.SpawnSnakeOnCube(cube, splinePath, startSide);
            pointsOfSnake = new List<CubePoint>
            {
                SpawnSnake.Instance.GetStartPoint()
            };
            Debug.Log("SideCoordinate: " + pointsOfSnake.First().SideCoordinate + ", FieldCoordinate: " + pointsOfSnake.First().FieldCoordinate.H + " / " + pointsOfSnake.First().FieldCoordinate.V);

            stepDirection = inputDirection;
            
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
            // TODO move the knot to the right point
            CubePoint lastPoint = pointsOfSnake.Last();
            
            BezierKnot lastKnot = splinePath.Spline.ToArray().Last();

            if (stepDirection != inputDirection)
            {
                lastKnot.Position = GetNextPositionVector(lastPoint);
                lastKnot.Rotation = GetNextRotation(lastPoint);
            }
        
            splinePath.Spline.SetKnot(splinePath.Spline.Count - 1, lastKnot);
            stepDirection = inputDirection;
        }
    
        /// <summary>
        /// Delete tail of splineArray and spawn new head, according to stepDirection
        /// </summary>
        private void UpdateSpline()
        {
            // add a new knot to the spline
            // head == end of splineArray
            // tail == start of splineArray

            BezierKnot lastKnot = splinePath.Spline.ToArray().Last();
            BezierKnot newKnot = new BezierKnot();


            CubePoint nextPoint = GetNextFieldOnCube();
            Debug.Log("SideCoordinate: " + nextPoint.SideCoordinate 
                                         + ", FieldCoordinate: " + nextPoint.FieldCoordinate.H 
                                         + " / " + nextPoint.FieldCoordinate.V);
            
            newKnot.Position = GetNextPositionVector(nextPoint);
            newKnot.Rotation = GetNextRotation(nextPoint);
            newKnot.TangentIn = new float3(0, 0, -0.33f);
            newKnot.TangentOut = new float3(0, 0, 0.33f);
 
            splinePath.Spline.Add(newKnot);
            splinePath.Spline.RemoveAt(0);
            
            pointsOfSnake.Add(nextPoint);
            pointsOfSnake.RemoveAt(0);
        }

        private Vector3 GetNextPositionVector(CubePoint nextPoint)
        {
            Vector3 positionInCube = nextPoint.SideCoordinate.GetPositionInCube(cube.Dimension, 1f); //TODO cubeScale -> variable
            Quaternion rotationInCube = nextPoint.SideCoordinate.GetRotationInCube();
            // position in the center of a field
            Vector3 positionInSide = nextPoint.FieldCoordinate.GetPositionInCubeSide(1f); // TODO change cubescale to variable

            // set position to the edge of a field
            positionInSide += stepDirection switch
            {
                MovementDirection.Up => new Vector3(0, 0, -0.5f),
                MovementDirection.Right => new Vector3(-0.5f, 0, 0),
                MovementDirection.Down => new Vector3(0, 0, 0.5f),
                MovementDirection.Left => new Vector3(0.5f, 0, 0),
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

        private CubePoint GetNextFieldOnCube()
        {
            CubePoint currentPoint = pointsOfSnake.Last();
            CubePoint nextPoint = currentPoint;
            
            // !!! Currently the local system equals up, down, ...
            // TODO edit inputManager
            // TODO when sideSwitch --> rotate cube (view)
            
            switch (stepDirection)
            {
                case MovementDirection.Up:
                    if (currentPoint.FieldCoordinate.V < cube.Sides[(int)currentPoint.SideCoordinate].Dimension.V)
                    {
                        CubeFieldCoordinate nextFieldCoordinate =
                            new CubeFieldCoordinate(currentPoint.FieldCoordinate.H, currentPoint.FieldCoordinate.V + 1);
                        
                        nextPoint = new CubePoint(currentPoint.SideCoordinate, nextFieldCoordinate);
                    }
                    else
                    {
                        nextPoint = currentPoint.GetPointOnNeighbour(DirectionOnCubeSide.posVert, cube); // TODO posVert isn't it always
                        
                    }
                    break;
                
                case MovementDirection.Down:
                    if (currentPoint.FieldCoordinate.V > 0)
                    {
                        CubeFieldCoordinate nextFieldCoordinate =
                            new CubeFieldCoordinate(currentPoint.FieldCoordinate.H, currentPoint.FieldCoordinate.V - 1);

                        nextPoint = new CubePoint(currentPoint.SideCoordinate, nextFieldCoordinate);
                    }
                    else
                    {
                        nextPoint = currentPoint.GetPointOnNeighbour(DirectionOnCubeSide.negVert, cube);
                    }
                    break;

                case MovementDirection.Right:
                    if (currentPoint.FieldCoordinate.H < cube.Sides[(int)currentPoint.SideCoordinate].Dimension.H)
                    {
                        CubeFieldCoordinate nextFieldCoordinate =
                            new CubeFieldCoordinate(currentPoint.FieldCoordinate.H + 1, currentPoint.FieldCoordinate.V);

                        nextPoint = new CubePoint(currentPoint.SideCoordinate, nextFieldCoordinate);
                    }
                    else
                    {
                        nextPoint = currentPoint.GetPointOnNeighbour(DirectionOnCubeSide.posHor, cube);
                    }
                    break;
                
                case MovementDirection.Left:
                    if (currentPoint.FieldCoordinate.H > 0)
                    {
                        CubeFieldCoordinate nextFieldCoordinate =
                            new CubeFieldCoordinate(currentPoint.FieldCoordinate.H - 1, currentPoint.FieldCoordinate.V);

                        nextPoint = new CubePoint(currentPoint.SideCoordinate, nextFieldCoordinate);
                    }
                    else
                    {
                        nextPoint = currentPoint.GetPointOnNeighbour(DirectionOnCubeSide.negHor, cube);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return nextPoint;
        }
    }
}
