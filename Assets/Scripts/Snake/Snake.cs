#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

namespace Snake
{
    public class Snake : MonoBehaviour
    {
        // Points is used for storing the cubePoints of the snake before going into a tunnel (same for SplinePath)
        private List<CubePoint> Points;
        // TempPoints is used if the snake comes out of the tunnel. It is used as long there is at least one cubePoint left in Points (same for TempSplinePath)
        private List<CubePoint> TempPoints = new List<CubePoint>();
        private Cube Cube;

        public SplineContainer SplinePath { get; private set; }
        public SplineContainer TempSplinePath { get; private set; }

        public float StepInterval;


        private InputDirection StepInputDirection;
        public InputDirection InputDirection;

        private DirectionOnCubeSide ReferenceDirectionForInput;

        [Header("Snake Bodyparts")]
        public GameObject SnakeHeadPrefab;
        public GameObject SnakeBodyPrefab;
        public GameObject SnakeTailPrefab;
        public GameObject EmptyPrefab;
        private List<GameObject> BodyParts = new List<GameObject>();

        private Snack Snack;

        private bool ShouldGrowNextUpdate = false;

        private bool HeadGoesThroughTunnel = false;
        private int StepsInsideTunnel = 3;
        private int CurrentStepsInsideTunnel = 0;
        private Tunnel Tunnel;
        private CubePoint? TunnelEntry;

        private bool ShouldMoveOverEdge;
        private DirectionOnCubeSide TempReferenceDirectionForInput;

        private bool ShouldMoveOverEdge;
        private DirectionOnCubeSide TempReferenceDirectionForInput;

        public void StartSnake(Cube cube, CubeSideCoordinate startSide, Snack snack, GameMode mode)
        {
            this.Cube = cube;
            this.Snack = snack;
            SplineContainer[] splineContainers = transform.GetComponents<SplineContainer>();
            this.SplinePath = splineContainers[0];
            this.TempSplinePath = splineContainers[1];
            this.Points = CreateStartPoints(cube, startSide);

            // stepInterval = 0.4 / 2 = 0.2
            this.StepInterval = this.StepInterval / mode.speedFactor;

            BuildSnakeBody();

            if (!this.SplinePath)
            {
                Debug.LogError("Couldnt get Splinepath. Snake GameObject needs a Spline component attached!");
                return;
            }

            StepInputDirection = InputDirection;
            // This defines how the cube is rotated on the spawnSide, so that _Up_ Input matches _Up_ on the screen
            ReferenceDirectionForInput = DirectionOnCubeSide.posVert;

            RotationManager.Instance.RotateToCubePoint(GetSnakeHead(), Cube.Dimension);

            // Start Cycle of Update Methods
            InvokeRepeating(nameof(DetermineNextStepDirection), StepInterval * 0.75f, StepInterval);
            InvokeRepeating(nameof(UpdateSpline), StepInterval, StepInterval);

            // Set first Snack on Cube
            Snack.AssignNewPosition(Points.ToArray());

            GameAudioManager.Instance.SwitchCubeSide(startSide);
        }

        private void Update()
        {
            InputDirection = InputManager.Instance.GetPlayerInput(StepInputDirection) ?? InputDirection;
        }

        private List<CubePoint> CreateStartPoints(Cube cube, CubeSideCoordinate startSide)
        {
            List<CubePoint> startPoints = new List<CubePoint>();

            // First Knot
            Vector3 positionInCube = startSide.GetPositionInCube(cube.Dimension, cube.Scale);
            Quaternion rotationInCube = startSide.GetRotationInCube();

            CubeFieldCoordinate startFieldCoordinate = startSide.GetDimension2D(cube.Dimension).GetFieldInLeftCenter();
            SplinePath.Spline.Add(AddKnotToPath(startFieldCoordinate, rotationInCube, positionInCube, cube.Scale, true));

            for (int i = 0; i < 3; i++)
            {
                CubeFieldCoordinate fieldCoordinate =
                    new CubeFieldCoordinate(startFieldCoordinate.H + i, startFieldCoordinate.V);

                SplinePath.Spline.Add(AddKnotToPath(fieldCoordinate, rotationInCube, positionInCube, cube.Scale));

                startPoints.Add(new CubePoint(startSide, fieldCoordinate));
            }

            return startPoints;
        }

        private static BezierKnot AddKnotToPath(CubeFieldCoordinate fieldCoordinate, Quaternion rotationInCube, Vector3 positionInCube, float scaleFactor, bool isFirstKnot = false)
        {
            Vector3 positionInSide = fieldCoordinate.GetPositionInCubeSide(scaleFactor);

            Vector3 startPositionOfSnake = isFirstKnot switch
            {
                true => positionInCube + rotationInCube * (positionInSide - new Vector3(0.5f * scaleFactor, 0, 0)),
                false => positionInCube + rotationInCube * (positionInSide + new Vector3(0.5f * scaleFactor, 0, 0))
            };

            Vector3 rotation = new Vector3(0, 90, 90);

            BezierKnot startKnot = new BezierKnot();
            startKnot.Position = startPositionOfSnake;
            startKnot.Rotation = Quaternion.Euler(rotation);
            startKnot.TangentIn = new Vector3(0, 0, -0.33f);
            startKnot.TangentOut = new Vector3(0, 0, 0.33f);

            return startKnot;
        }


        /// <summary>
        /// Set position of next spline point, according to current inputDirection.
        /// </summary>
        private void DetermineNextStepDirection()
        {
            if (HeadGoesThroughTunnel)
            {
                return;
            }

            if (StepInputDirection == InputDirection)
            {
                return;
            }

            StepInputDirection = InputDirection;

            VisualiseNextStepDirection();
        }

        private void VisualiseNextStepDirection()
        {
            CubePoint snakeHead = GetSnakeHead();

            if (TempPoints.Count > 0)
            {
                TempSplinePath.Spline.SetKnot(TempSplinePath.Spline.Count - 1, CalculateSplineKnot(snakeHead));
            }
            else
            {
                SplinePath.Spline.SetKnot(SplinePath.Spline.Count - 1, CalculateSplineKnot(snakeHead));
            }
        }

        /// <summary>
        /// Delete tail of splineArray and spawn new head, according to stepDirection
        /// </summary>
        private void UpdateSpline()
        {
            // add a new knot to the spline
            // head == end of splineArray
            // tail == start of splineArray

            DirectionOnCubeSide stepDirectionOnCubeSide = StepInputDirection.ToLocalDirectionOnCubeSide(ReferenceDirectionForInput);
            CubePoint nextPoint = GetPointOnCubeInDirection(stepDirectionOnCubeSide);

            foreach (Tunnel tunnel in Cube.Tunnels)
            {
                if (tunnel.HasPoint(nextPoint))
                {
                    HeadGoesThroughTunnel = true;
                    Tunnel = tunnel;
                    TunnelEntry = tunnel.PointA.IsEqual(nextPoint) ? tunnel.PointA : tunnel.PointB;
                }
            }

            if (HeadGoesThroughTunnel && TunnelEntry != null)
            {
                // is called if the snakeHead is in a tunnel

                // TunnelEntry
                if (CurrentStepsInsideTunnel == 0)
                {
                    // move over edge if the tunnelEntry is on the next cubeSide
                    if (ShouldMoveOverEdge)
                    {
                        RotationManager.Instance.RotateOneSide(StepInputDirection, nextPoint, Cube.Dimension);
                        ReferenceDirectionForInput = TempReferenceDirectionForInput;
                    }

                    SplinePath.Spline.Add(CalculateSplineKnot(TunnelEntry));
                    Points.Add(TunnelEntry);

                    CurrentStepsInsideTunnel++;
                }
                // TunnelExit
                else if (CurrentStepsInsideTunnel == StepsInsideTunnel)
                {
                    CubePoint tunnelExit = Tunnel.GetExitCubePoint(TunnelEntry);
                    // Knot is in the inside of the cube behind the tunnelExit
                    TempSplinePath.Spline.Add(CalculateSplineKnot(tunnelExit));
                    TempPoints.Add(tunnelExit);

                    UpdateReferenceDirectionForInputOnOppositeSide(TunnelEntry);

                    HeadGoesThroughTunnel = false;
                    // Knot is located on the outside of the cube at the tunnelExit (GoesThroughTunnel needs to be false here)
                    TempSplinePath.Spline.Add(CalculateSplineKnot(tunnelExit));

                    CurrentStepsInsideTunnel = 0;
                    TunnelEntry = null;

                    RotationManager.Instance.RotateToOppositeSide(StepInputDirection);
                }
                // Everything between TunnelEntry and TunnelExit
                else
                {
                    // needs to added to Points, so we do decrease the length of the snake (the length depends on the length of Points)
                    Points.Add(TunnelEntry);

                    if (CurrentStepsInsideTunnel == 1)
                    {
                        // add one more knot inside the tunnel to store gameObjects before changing their splineContainer to TempSplinePath
                        SplinePath.Spline.Add(CalculateSplineKnot(TunnelEntry));
                    }

                    CurrentStepsInsideTunnel++;
                }

                SplinePath.Spline.RemoveAt(0);
                Points.RemoveAt(0);
            }
            else
            {
                // is called if the snakeHead is not in a tunnel

                if (ShouldMoveOverEdge)
                {
                    RotationManager.Instance.RotateOneSide(StepInputDirection, nextPoint, Cube.Dimension);
                    ReferenceDirectionForInput = TempReferenceDirectionForInput;
                }

                if (TempPoints.Count > 0)
                {
                    TempSplinePath.Spline.Add(CalculateSplineKnot(nextPoint));
                    TempPoints.Add(nextPoint);
                }
                else
                {
                    SplinePath.Spline.Add(CalculateSplineKnot(nextPoint));
                    Points.Add(nextPoint);
                }

                if (ShouldGrowNextUpdate is false)
                {
                    // After being in the tunnel it is possible, that the splinePath contains less elements than Points
                    if (SplinePath.Spline.Count > 0)
                    {
                        SplinePath.Spline.RemoveAt(0);
                    }
                    Points.RemoveAt(0);
                }
                else
                {
                    ShouldGrowNextUpdate = false;
                }

                if (Points.Count == 0)
                {
                    // the snake is fully moved through the tunnel --> TempPoints no longer needs to be used
                    Points = TempPoints;
                    SplinePath.Spline = TempSplinePath.Spline;

                    TempPoints = new List<CubePoint>();
                    TempSplinePath.Spline = new Spline();
                }

                RotationManager.Instance.RotateEveryStep(StepInputDirection, GetSnakeHead(), Cube.Dimension);
            }


            // check if the snack is going to be eaten by the snake
            if (nextPoint.IsEqual(Snack.Position))
            {
                EatSnack();
            }
            else
            {
                UpdateSnakeBody();
            }

            if (nextPoint.IsEqualToPointInLists(Points, TempPoints))
            {
                StopSnake();
                GameManager.Instance.GameOver();
            }
        }

        /// <summary>
        /// Returns the current CubePoint of the snakeHead. This already considers if the snake goes through the tunnel.
        /// </summary>
        private CubePoint GetSnakeHead()
        {
            return TempPoints.Count > 0 ? TempPoints.Last() : Points.Last();
        }

        private void EatSnack()
        {
            Snack.AssignNewPosition(Points.ToArray());
            AddSnakeBodyPart();
            UpdateSnakeBodyAfterSnack();
            GameAudioManager.Instance.EatSnackAudioSource.Play();
            ShouldGrowNextUpdate = true;
        }

        private void UpdateReferenceDirectionForInputOnOppositeSide(CubePoint point)
        {
            DirectionOnCubeSide direction = StepInputDirection.ToLocalDirectionOnCubeSide(ReferenceDirectionForInput);

            for (int i = 0; i < 2; i++)
            {
                CubePoint nextPoint = point.GetPointOnNeighbour(direction, Cube);

                (CubeSideCoordinate neighborCoordinate, DirectionOnCubeSide neighborDirection) nextSide =
                    point.SideCoordinate.GetNeighborWithDirection(direction);

                ReferenceDirectionForInput = StepInputDirection.GetInputUpAsDirectionOnCubeSide(nextSide.neighborDirection);

                point = nextPoint;
                direction = nextSide.neighborDirection;
            }
        }

        private BezierKnot CalculateSplineKnot(CubePoint cubePoint)
        {
            DirectionOnCubeSide stepDirectionOnCubeSide = StepInputDirection.ToLocalDirectionOnCubeSide(ReferenceDirectionForInput);

            BezierKnot bezierKnot = new BezierKnot();

            bezierKnot.Position = CalculateKnotPosition(cubePoint, stepDirectionOnCubeSide);
            bezierKnot.Rotation = HeadGoesThroughTunnel
                ? CalculateKnotRotationInTunnel(cubePoint, stepDirectionOnCubeSide)
                : CalculateKnotRotation(cubePoint, stepDirectionOnCubeSide);
            bezierKnot.TangentIn = new Vector3(0, 0, -0.33f);
            bezierKnot.TangentOut = new Vector3(0, 0, 0.33f);

            return bezierKnot;
        }

        private Vector3 CalculateKnotPosition(CubePoint point, DirectionOnCubeSide stepDirectionOnCubeSide)
        {
            Vector3 positionInCube = point.SideCoordinate.GetPositionInCube(Cube.Dimension, Cube.Scale);
            Quaternion rotationInCube = point.SideCoordinate.GetRotationInCube();

            // position of the center of a field
            Vector3 positionInSide = point.FieldCoordinate.GetPositionInCubeSide(Cube.Scale);

            // move the position to the edge of the field, to which the snake moves. Except it is a tunnelField, than we need the centered position.
            if (!HeadGoesThroughTunnel)
            {
                positionInSide += (stepDirectionOnCubeSide switch
                {
                    DirectionOnCubeSide.negHor => new Vector3(-1, 0, 0) * 0.5f * Cube.Scale,
                    DirectionOnCubeSide.posHor => new Vector3(1, 0, 0) * 0.5f * Cube.Scale,
                    DirectionOnCubeSide.negVert => new Vector3(0, 0, -1) * 0.5f * Cube.Scale,
                    DirectionOnCubeSide.posVert => new Vector3(0, 0, 1) * 0.5f * Cube.Scale,
                    _ => new Vector3(0, 0, 0)
                });
            }
            else
            {
                if (CurrentStepsInsideTunnel == 1)
                {
                    // position of this knot is deep inside of the tunnel (it needs to be there so we can store the gameObjects before changing their splineContainer to TempSplinePath
                    positionInSide += new Vector3(0, -2, 0) * 0.5f * Cube.Scale;
                }
                else
                {
                    positionInSide += new Vector3(0, -1, 0) * 0.5f * Cube.Scale;
                }
            }


            return positionInCube + (rotationInCube * positionInSide);
        }

        private Quaternion CalculateKnotRotationInTunnel(CubePoint point, DirectionOnCubeSide stepDirectionOnCubeSide)
        {
            Quaternion sideRotation = point.SideCoordinate.GetRotationInCube();

            Vector3 rotationOnSide;

            // The rotation of the knot must point into the cube if the point is a TunnelEntry. If it is a TunnelExit, it must point away from the cube.
            bool isTunnelEntry = point.IsEqual(TunnelEntry!);

            // this switch is essential for the rotation of a knot on a cubeSide when the snake moves into the cube
            // I couldn't get the rotation around a specific point, so it needed this workaround
            switch (point.SideCoordinate)
            {
                case CubeSideCoordinate.Front:
                case CubeSideCoordinate.Right:
                case CubeSideCoordinate.Back:
                case CubeSideCoordinate.Left:
                    rotationOnSide = stepDirectionOnCubeSide switch
                    {
                        DirectionOnCubeSide.negHor => isTunnelEntry ? new Vector3(90, 0, -90) : new Vector3(90, -180, -90),
                        DirectionOnCubeSide.posHor => isTunnelEntry ? new Vector3(90, 0, 90) : new Vector3(90, 180, 90),
                        DirectionOnCubeSide.negVert => isTunnelEntry ? new Vector3(-270, 90, 90) : new Vector3(-90, 90, 90),
                        DirectionOnCubeSide.posVert => isTunnelEntry ? new Vector3(-90, 180, 0) : new Vector3(90, 180, 0),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;

                case CubeSideCoordinate.Up:
                    rotationOnSide = stepDirectionOnCubeSide switch
                    {
                        DirectionOnCubeSide.negHor => isTunnelEntry ? new Vector3(90, -90, 180) : new Vector3(-90, -90, 180),
                        DirectionOnCubeSide.posHor => isTunnelEntry ? new Vector3(90, 90, 180) : new Vector3(-90, 90, 180),
                        DirectionOnCubeSide.negVert => isTunnelEntry ? new Vector3(90, 0, 0) : new Vector3(-90, 0, 0),
                        DirectionOnCubeSide.posVert => isTunnelEntry ? new Vector3(90, 0, 180) : new Vector3(-90, 0, 180),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;

                case CubeSideCoordinate.Down:
                    rotationOnSide = stepDirectionOnCubeSide switch
                    {
                        DirectionOnCubeSide.negHor => isTunnelEntry ? new Vector3(-90, 90, 180) : new Vector3(90, 90, 180),
                        DirectionOnCubeSide.posHor => isTunnelEntry ? new Vector3(-90, -90, 180) : new Vector3(90, -90, 180),
                        DirectionOnCubeSide.negVert => isTunnelEntry ? new Vector3(-90, 0, 0) : new Vector3(-270, 0, 0),
                        DirectionOnCubeSide.posVert => isTunnelEntry ? new Vector3(-90, 0, 180) : new Vector3(90, 0, 180),
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

        private Quaternion CalculateKnotRotation(CubePoint point, DirectionOnCubeSide stepDirectionOnCubeSide)
        {
            Quaternion sideRotation = point.SideCoordinate.GetRotationInCube();

            Vector3 rotationOnSide;

            // this switch is essential for the rotation of a knot on a cubeSide
            // I couldn't get the rotation around a specific point, so it needed this workaround
            switch (point.SideCoordinate)
            {
                case CubeSideCoordinate.Front:
                case CubeSideCoordinate.Right:
                case CubeSideCoordinate.Back:
                case CubeSideCoordinate.Left:
                    rotationOnSide = stepDirectionOnCubeSide switch
                    {
                        DirectionOnCubeSide.negHor => new Vector3(90, -90, -90),
                        DirectionOnCubeSide.posHor => new Vector3(90, 90, 90),
                        DirectionOnCubeSide.negVert => new Vector3(-180, 90, 90),
                        DirectionOnCubeSide.posVert => new Vector3(0, 180, 0),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;

                case CubeSideCoordinate.Up:
                    rotationOnSide = stepDirectionOnCubeSide switch
                    {
                        DirectionOnCubeSide.negHor => new Vector3(0, -90, 180),
                        DirectionOnCubeSide.posHor => new Vector3(0, 90, 180),
                        DirectionOnCubeSide.negVert => new Vector3(-180, 0, 0),
                        DirectionOnCubeSide.posVert => new Vector3(0, 0, 180),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    break;

                case CubeSideCoordinate.Down:
                    rotationOnSide = stepDirectionOnCubeSide switch
                    {
                        DirectionOnCubeSide.negHor => new Vector3(0, 90, 180),
                        DirectionOnCubeSide.posHor => new Vector3(0, -90, 180),
                        DirectionOnCubeSide.negVert => new Vector3(-180, 0, 0),
                        DirectionOnCubeSide.posVert => new Vector3(0, 0, 180),
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

        private CubePoint GetPointOnCubeInDirection(DirectionOnCubeSide direction)
        {
            var snakeHead = GetSnakeHead();

            if (Cube.Sides[(int)snakeHead.SideCoordinate].Dimension.IsPointInDirectionInDimension(snakeHead, direction))
            {
                ShouldMoveOverEdge = false;
                return snakeHead.GetPointOnSameSide(direction);
            }
            else // snake moves across an edge 
            {
                (CubeSideCoordinate neighborCoordinate, DirectionOnCubeSide neighborDirection) nextSide =
                snakeHead.SideCoordinate.GetNeighborWithDirection(direction);

                CubePoint nextPoint = snakeHead.GetPointOnNeighbour(direction, Cube);

                ShouldMoveOverEdge = true;
                TempReferenceDirectionForInput = StepInputDirection.GetInputUpAsDirectionOnCubeSide(nextSide.neighborDirection);

                GameAudioManager.Instance.SwitchCubeSide(nextPoint.SideCoordinate);

                return nextPoint;
            }
        }

        private void UpdateSnakeBody()
        {
            // Points & SplinePath --> Points and Knots before going into the Tunnel
            // TempPoints & TempSplinePath --> Points and Knots after coming out of the TunnelExit

            if (HeadGoesThroughTunnel && CurrentStepsInsideTunnel > 1)
            {
                int movingBodyPartsCount = BodyParts.Count - (CurrentStepsInsideTunnel - 2);

                // stop the gameObjects which are moved into the cube through the tunnelEntry
                for (int i = (BodyParts.Count - 1); i >= movingBodyPartsCount; i--)
                {
                    SplineAnimate bodyPartAnimate = BodyParts[i].GetComponent<SplineAnimate>();
                    bodyPartAnimate.Container = SplinePath;
                    bodyPartAnimate.StartOffset = 1f;
                    bodyPartAnimate.NormalizedTime = 0f;
                    bodyPartAnimate.Pause();
                }

                // keep the gameObject animates which are still moving towards the tunnelEntry
                for (int i = 0; i < movingBodyPartsCount; i++)
                {
                    SplineAnimate bodyPartAnimate = BodyParts[i].GetComponent<SplineAnimate>();
                    bodyPartAnimate.Container = SplinePath;
                    bodyPartAnimate.StartOffset = 0;
                    bodyPartAnimate.NormalizedTime = i * (1.0f / movingBodyPartsCount);
                    bodyPartAnimate.Play();
                }

            }
            else if (TempPoints.Count > 0)
            {
                // a part of the snake came through the tunnelExit and is now moving on the cube again

                int containerChangedCounter = 0;
                int bodyPartsOnTempSplinePath = 0;

                for (int i = BodyParts.Count - 1; i >= 0; i--)
                {
                    SplineAnimate bodyPartAnimate = BodyParts[i].GetComponent<SplineAnimate>();

                    // move last part of the Snake to TempSplinePath
                    if (bodyPartAnimate.Container == SplinePath && containerChangedCounter < 2)
                    {
                        bodyPartAnimate.Container = TempSplinePath;
                        containerChangedCounter++;
                    }

                    // count how many BodyParts are placed on the TempSplinePath
                    if (bodyPartAnimate.Container == TempSplinePath)
                    {
                        bodyPartsOnTempSplinePath++;
                    }
                }

                // animate the gameObjects on both splinePaths
                int k = 0;
                for (int i = 0; i < BodyParts.Count; i++)
                {
                    SplineAnimate bodyPartAnimate = BodyParts[i].GetComponent<SplineAnimate>();
                    bodyPartAnimate.StartOffset = 0;

                    if (bodyPartAnimate.Container == SplinePath)
                    {
                        bodyPartAnimate.NormalizedTime = i * (1.0f / (BodyParts.Count - bodyPartsOnTempSplinePath));
                    }
                    else
                    {
                        bodyPartAnimate.NormalizedTime = k * (1.0f / bodyPartsOnTempSplinePath);
                        k++;
                    }

                    bodyPartAnimate.Play();
                }
            }
            else
            {
                // set each bodypart to a specific percantage of the spline when updating the spline
                for (int i = 0; i < BodyParts.Count; i++)
                {
                    SplineAnimate bodyPartAnimate = BodyParts[i].GetComponent<SplineAnimate>();
                    bodyPartAnimate.Container = SplinePath;
                    bodyPartAnimate.StartOffset = 0;
                    bodyPartAnimate.NormalizedTime = i * (1.0f / BodyParts.Count);
                    bodyPartAnimate.Play();
                }
            }
        }

        private void UpdateSnakeBodyAfterSnack()
        {
            // set each bodypart to a specific percantage of the spline 
            //  - Tail and old BodyParts pause the animation
            //  - new BodyParts, Head, and EmptyGameObject move further

            // Tail and old BodyParts
            for (int i = 0; i < BodyParts.Count - 4; i++)
            {
                SplineAnimate bodyPartAnimate = BodyParts[i].GetComponent<SplineAnimate>();
                bodyPartAnimate.StartOffset = i * (1.0f / (BodyParts.Count - 2));
                bodyPartAnimate.Pause();
            }

            // new BodyParts
            for (int i = BodyParts.Count - 4; i < BodyParts.Count - 2; i++)
            {
                SplineAnimate bodyPartAnimate = BodyParts[i].GetComponent<SplineAnimate>();
                bodyPartAnimate.StartOffset = (i - 2) * (1.0f / (BodyParts.Count - 2));
                bodyPartAnimate.NormalizedTime = (i - 2) * (1.0f / (BodyParts.Count - 2));
                bodyPartAnimate.Play();
            }

            // Head and EmptyGameObject
            for (int i = BodyParts.Count - 2; i < BodyParts.Count; i++)
            {
                SplineAnimate bodyPartAnimate = BodyParts[i].GetComponent<SplineAnimate>();
                bodyPartAnimate.NormalizedTime = i * (1.0f / BodyParts.Count);
                bodyPartAnimate.Play();
            }
        }

        private void PauseSnake()
        {
            for (int i = 0; i < BodyParts.Count - 1; i++)
            {
                SplineAnimate animate = BodyParts[i].GetComponent<SplineAnimate>();

                animate.StartOffset = i * (1.0f / BodyParts.Count);
                animate.MaxSpeed = 0; // Pause
            }
        }

        /// <summary>
        /// Fill the List of BodyParts (instantiated GameObjects) with each part of the snake
        /// </summary>
        private void BuildSnakeBody()
        {
            // Tail
            BodyParts.Add(Instantiate(SnakeTailPrefab));

            // Body (iterate doubled index for more density in the body)
            for (int i = 1; i < (SplinePath.Spline.GetLength() * 2) - 3; i++)
            {
                BodyParts.Add(Instantiate(SnakeBodyPrefab));
            }

            // Head
            BodyParts.Add(Instantiate(SnakeHeadPrefab));

            // Empty
            BodyParts.Add(Instantiate(EmptyPrefab));

            for (int i = 0; i < BodyParts.Count; i++)
            {
                ConfigureBodyAnimator(i);
            }
        }

        /// <summary>
        /// Add a new BodyPart to the snake. (At the moment: adds 2 BodyParts for more density)
        /// </summary>
        private void AddSnakeBodyPart()
        {
            int index = BodyParts.Count - 2;

            for (int i = 0; i < 2; i++)
            {
                BodyParts.Insert(index, Instantiate(SnakeBodyPrefab));
                ConfigureBodyAnimator(index);
            }
        }

        /// <summary>
        /// Ensure to set initial options like container and speed for the animator of each bodypart
        /// </summary>
        private void ConfigureBodyAnimator(int index)
        {
            SplineAnimate animate = BodyParts[index].GetComponent<SplineAnimate>();
            animate.Container = SplinePath;
            animate.Loop = SplineAnimate.LoopMode.Once;
            animate.AnimationMethod = SplineAnimate.Method.Speed;
            animate.MaxSpeed = Cube.Scale / StepInterval;

            animate.StartOffset = index * (1.0f / BodyParts.Count);
        }

        private void StopSnake()
        {
            PauseSnake();

            CancelInvoke(nameof(DetermineNextStepDirection));
            CancelInvoke(nameof(UpdateSpline));

            Debug.Log("Game Over");
        }
    }
}
