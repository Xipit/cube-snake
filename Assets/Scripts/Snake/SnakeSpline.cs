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
            
            BezierKnot lastKnot = splinePath.Spline.ToArray().Last();

            if (stepDirection != inputDirection)
            {
                Vector3 directionVector = (inputDirection.GetDirectionVector(stepLength) -
                                           stepDirection.GetDirectionVector(stepLength)) / 2;
                lastKnot.Position += (float3)directionVector;
                
                lastKnot.Rotation = inputDirection.GetRotation();
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

            pointsOfSnake.Add(nextPoint);

            // TODO use nextPoint to get the next knot
            Vector3 positionInCube = nextPoint.SideCoordinate.GetPositionInCube(cube.Dimension, 1f); //TODO cubeScale -> variable
            Quaternion rotationInCube = nextPoint.SideCoordinate.GetRotationInCube();
            Vector3 positionInSide = nextPoint.FieldCoordinate.GetPositionInCubeSide(1f); // TODO change cubescale to variable

            Vector3 nextPointVector = positionInCube + rotationInCube * positionInSide;
            
            
            newKnot.Position = nextPointVector; // lastKnot.Position + (float3)stepDirection.GetDirectionVector(stepLength);
            newKnot.Rotation = nextPoint.SideCoordinate.GetRotationInCube();
            newKnot.TangentIn = new float3(0, 0, -0.33f);
            newKnot.TangentOut = new float3(0, 0, 0.33f);
 
            splinePath.Spline.Add(newKnot);
            splinePath.Spline.RemoveAt(0);
        }

        private CubePoint GetNextFieldOnCube()
        {
            CubePoint currentPoint = pointsOfSnake.Last();
            CubePoint nextPoint = currentPoint;
            
            // !!! Currently the local system equals up, down, ...
            // TODO edit inputManager
            
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
                        nextPoint = GetPointOnNeighbour(DirectionOnCubeSide.posVert, currentPoint); // TODO posVert isn't it always
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
                        nextPoint = GetPointOnNeighbour(DirectionOnCubeSide.negVert, currentPoint);
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
                        nextPoint = GetPointOnNeighbour(DirectionOnCubeSide.posHor, currentPoint);
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
                        nextPoint = GetPointOnNeighbour(DirectionOnCubeSide.negHor, currentPoint);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return nextPoint;
        }

        
        private CubePoint GetPointOnNeighbour(DirectionOnCubeSide currentDirectionOnSide, CubePoint currentPoint)
        {
            (CubeSideCoordinate neighborCoordinate, DirectionOnCubeSide neighborDirection) nextSide =
                currentPoint.SideCoordinate.GetNeighborWithDirection(currentDirectionOnSide);

            int neighbourMaxH = cube.Sides[(int)nextSide.neighborCoordinate].Dimension.H;
            int neighbourMaxV = cube.Sides[(int)nextSide.neighborCoordinate].Dimension.V;

            int currentH = currentPoint.FieldCoordinate.H;
            int currentV = currentPoint.FieldCoordinate.V;

            CubeFieldCoordinate? nextLocalField = null;

            switch (currentDirectionOnSide)
            {
                // snake moves negHor on current Side
                case DirectionOnCubeSide.negHor:
                    nextLocalField = nextSide.neighborDirection switch
                    {
                        DirectionOnCubeSide.negHor => new CubeFieldCoordinate(neighbourMaxH, currentV),
                        DirectionOnCubeSide.posHor => new CubeFieldCoordinate(0, neighbourMaxV - currentV),
                        DirectionOnCubeSide.negVert => new CubeFieldCoordinate(neighbourMaxH - currentV, neighbourMaxV),
                        DirectionOnCubeSide.posVert => new CubeFieldCoordinate(currentV, 0),
                        _ => null
                    };
                    break;
                
                // snake moves posHor on current Side
                case DirectionOnCubeSide.posHor:
                    nextLocalField = nextSide.neighborDirection switch
                    {
                        DirectionOnCubeSide.negHor => new CubeFieldCoordinate(neighbourMaxH, neighbourMaxV - currentV),
                        DirectionOnCubeSide.posHor => new CubeFieldCoordinate(0, currentV),
                        DirectionOnCubeSide.negVert => new CubeFieldCoordinate(currentV, neighbourMaxV),
                        DirectionOnCubeSide.posVert => new CubeFieldCoordinate(neighbourMaxH - currentV, 0),
                        _ => null
                    };
                    break;
                
                // snake moves negVert on current Side
                case DirectionOnCubeSide.negVert:
                    nextLocalField = nextSide.neighborDirection switch
                    {
                        DirectionOnCubeSide.negHor => new CubeFieldCoordinate(neighbourMaxH, neighbourMaxV - currentH),
                        DirectionOnCubeSide.posHor => new CubeFieldCoordinate(0, currentH),
                        DirectionOnCubeSide.negVert => new CubeFieldCoordinate(currentH, neighbourMaxV),
                        DirectionOnCubeSide.posVert => new CubeFieldCoordinate(neighbourMaxH - currentH, 0),
                        _ => null
                    };
                    break;
                    
                // snake moves posVert on current Side
                case DirectionOnCubeSide.posVert:
                    nextLocalField = nextSide.neighborDirection switch
                    {
                        DirectionOnCubeSide.negHor => new CubeFieldCoordinate(neighbourMaxH, currentH),
                        DirectionOnCubeSide.posHor => new CubeFieldCoordinate(0,  neighbourMaxV - currentH),
                        DirectionOnCubeSide.negVert => new CubeFieldCoordinate(neighbourMaxH - currentH, neighbourMaxV),
                        DirectionOnCubeSide.posVert => new CubeFieldCoordinate(currentH, 0),
                        _ => null
                    };
                    break;
            }

            if (nextLocalField == null)
            {
                Debug.LogError("local field on neighbour side could not be found");
                return new CubePoint(CubeSideCoordinate.Front, new CubeFieldCoordinate(0,0));
            }
            
            return new CubePoint(nextSide.neighborCoordinate, nextLocalField);
        }
    }
}
