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
        private List<CubePoint> Points;
        private Cube Cube;

        public SplineContainer SplinePath { get; private set; }
        public CubeSideCoordinate CurrentSideCoordinate { get; private set; }


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

        private bool shouldGrowNextUpdate = false;

        public void StartSnake(Cube cube, CubeSideCoordinate startSide, Snack snack)
        {
            this.Cube = cube;
            this.Snack = snack;
            this.SplinePath = transform.GetComponent<SplineContainer>();
            this.Points = CreateStartPoints(cube, startSide);

            BuildSnakeBody();

            if (!this.SplinePath)
            {
                Debug.LogError("Couldnt get Splinepath. Snake GameObject needs a Spline component attached!");
                return;
            }

            StepInputDirection = InputDirection;
            // This defines how the cube is rotated on the spawnSide, so that _Up_ Input matches _Up_ on the screen
            ReferenceDirectionForInput = DirectionOnCubeSide.posVert;

            // Start Cycle of Update Methods
            InvokeRepeating(nameof(DetermineNextStepDirection), StepInterval * 0.75f, StepInterval);
            InvokeRepeating(nameof(UpdateSpline), StepInterval, StepInterval);

            // Set first Snack on Cube
            Snack.AssignNewPosition(Points.ToArray());
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

        private void Update()
        {
            InputDirection = InputManager.Instance.GetPlayerInput(StepInputDirection) ?? InputDirection;
        }

        /// <summary>
        /// Set position of next spline point, according to current inputDirection.
        /// </summary>
        private void DetermineNextStepDirection()
        {
            if (StepInputDirection == InputDirection)
            {
                return;
            }

            StepInputDirection = InputDirection;

            VisualiseNextStepDirection();
        }

        private void VisualiseNextStepDirection()
        {
            CubePoint snakeHead = Points.Last();

            SplinePath.Spline.SetKnot(SplinePath.Spline.Count - 1, CalculateSplineKnot(snakeHead));
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

            SplinePath.Spline.Add(CalculateSplineKnot(nextPoint));
            Points.Add(nextPoint);

            if (shouldGrowNextUpdate is false)
            {
                SplinePath.Spline.RemoveAt(0);
                Points.RemoveAt(0);
            }
            else
            {
                shouldGrowNextUpdate = false;
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

            // check if a snakeBodyPart is on the next point --> GameOver
            for (int i = 0; i < Points.Count - 2; i++)
            {
                if (nextPoint.IsEqual(Points[i]))
                {
                    GetComponent<GameOver>().OnGameOver();
                    GameOver();
                }
            }
            //Debug.Log(nextPoint.SideCoordinate.ToString());
            CurrentSideCoordinate = nextPoint.SideCoordinate;
        }

        private void EatSnack()
        {
            Snack.AssignNewPosition(Points.ToArray());
            AddSnakeBodyPart();
            UpdateSnakeBodyAfterSnack();
            shouldGrowNextUpdate = true;
            GetComponent<SnackEating>().OnSnackEaten();
        }

        private BezierKnot CalculateSplineKnot(CubePoint cubePoint)
        {
            DirectionOnCubeSide stepDirectionOnCubeSide = StepInputDirection.ToLocalDirectionOnCubeSide(ReferenceDirectionForInput);

            BezierKnot bezierKnot = new BezierKnot();

            bezierKnot.Position = CalculateKnotPosition(cubePoint, stepDirectionOnCubeSide);
            bezierKnot.Rotation = CalculateKnotRotation(cubePoint, stepDirectionOnCubeSide);
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

            // move the position to the edge of the field, to which the snake moves
            positionInSide += (stepDirectionOnCubeSide switch
            {
                DirectionOnCubeSide.negHor => new Vector3(-1, 0, 0) * 0.5f * Cube.Scale,
                DirectionOnCubeSide.posHor => new Vector3(1, 0, 0) * 0.5f * Cube.Scale,
                DirectionOnCubeSide.negVert => new Vector3(0, 0, -1) * 0.5f * Cube.Scale,
                DirectionOnCubeSide.posVert => new Vector3(0, 0, 1) * 0.5f * Cube.Scale,
                _ => new Vector3(0, 0, 0)
            });

            return positionInCube + (rotationInCube * positionInSide);
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
            CubePoint snakeHead = Points.Last();

            if (Cube.Sides[(int)snakeHead.SideCoordinate].Dimension.IsPointInDirectionInDimension(snakeHead, direction))
            {
                return snakeHead.GetPointOnSameSide(direction);
            }
            else
            {
                (CubeSideCoordinate neighborCoordinate, DirectionOnCubeSide neighborDirection) nextSide =
                snakeHead.SideCoordinate.GetNeighborWithDirection(direction);

                CubePoint nextPoint = snakeHead.GetPointOnNeighbour(direction, Cube);

                ReferenceDirectionForInput = StepInputDirection.GetInputUpAsDirectionOnCubeSide(nextSide.neighborDirection);
                RotationReferenceManager.Instance.Rotate(StepInputDirection);

                return nextPoint;
            }
        }

        private void UpdateSnakeBody()
        {
            // set each bodypart to a specific percantage of the spline when updating the spline
            for (int i = 0; i < BodyParts.Count; i++)
            {
                SplineAnimate bodyPartAnimate = BodyParts[i].GetComponent<SplineAnimate>();
                bodyPartAnimate.StartOffset = 0;
                bodyPartAnimate.NormalizedTime = i * (1.0f / BodyParts.Count);
                bodyPartAnimate.Play();
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

        private void StopSnake()
        {
            for (int i = 0; i < BodyParts.Count - 1; i++)
            {
                SplineAnimate animate = BodyParts[i].GetComponent<SplineAnimate>();

                animate.StartOffset = i * (1.0f / BodyParts.Count);
                animate.Pause();
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

        private void GameOver()
        {

            StopSnake();

            BackgroundMusic backgroundMusic = FindObjectOfType<BackgroundMusic>(); // Searching for Background Music Script
            if (backgroundMusic != null)
            {
                backgroundMusic.StopMusic();
            }

            CancelInvoke(nameof(DetermineNextStepDirection));
            CancelInvoke(nameof(UpdateSpline));
        }
    }
}
