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
        private SnakeSpline Spline;
        private SnakeBody Body;

        private Cube Cube;

        public float StepInterval;


        private InputDirection StepInputDirection;
        public InputDirection InputDirection;

        private DirectionOnCubeSide ReferenceDirectionForInput;

        [Header("Snake Bodyparts")]
        public GameObject SnakeHeadPrefab;
        public GameObject SnakeBodyPrefab;
        public GameObject SnakeTailPrefab;
        public GameObject EmptyPrefab;

        private Snack Snack;

        private bool ShouldGrowNextUpdate = false;
        private bool HeadGoesThroughTunnel = false;

        public void StartSnake(Cube cube, CubeSideCoordinate startSide, Snack snack, GameMode mode)
        {
            this.Spline = new SnakeSpline(this.transform, cube, startSide);
            this.Body = new SnakeBody(cube, StepInterval, Spline.SplinePath, this.transform, SnakeHeadPrefab, SnakeBodyPrefab, SnakeTailPrefab, EmptyPrefab);
            this.Cube = cube;
            this.Snack = snack;

            // stepInterval = 0.4 / 2 = 0.2
            this.StepInterval = this.StepInterval / mode.speedFactor;

            StepInputDirection = InputDirection;
            // This defines how the cube is rotated on the spawnSide, so that _Up_ Input matches _Up_ on the screen
            ReferenceDirectionForInput = DirectionOnCubeSide.posVert;

            RotationManager.Instance.RotateToCubePoint(Spline.GetSnakeHead(), Cube.Dimension);

            // Start Cycle of Update Methods
            InvokeRepeating(nameof(DetermineNextStepDirection), StepInterval * 0.75f, StepInterval);
            InvokeRepeating(nameof(UpdateSnake), StepInterval, StepInterval);

            // Set first Snack on Cube
            Snack.AssignNewPosition(Spline.GetPointsWithSnakeHead().ToArray());

            GameAudioManager.Instance.SwitchCubeSide(startSide);
        }

        private void Update()
        {
            InputDirection = InputManager.Instance.GetPlayerInput(StepInputDirection) ?? InputDirection;
        }

        
        private void UpdateSnake()
        {
            DirectionOnCubeSide stepDirectionOnCubeSide = StepInputDirection.ToLocalDirectionOnCubeSide(ReferenceDirectionForInput);
            CubePoint nextPoint = GetPointOnCubeInDirection(stepDirectionOnCubeSide);


            foreach (Tunnel tunnel in Cube.Tunnels)
            {
                if (tunnel.HasPoint(nextPoint))
                {
                    HeadGoesThroughTunnel = true;
                    Spline.SetTunnel(tunnel, nextPoint);
                    // TunnelEntry = tunnel.PointA.IsEqual(nextPoint) ? tunnel.PointA : tunnel.PointB;
                }
            }
            Spline.UpdateSpline(stepDirectionOnCubeSide, StepInputDirection, nextPoint, Cube, HeadGoesThroughTunnel);

            // check if the snack is going to be eaten by the snake
            if (nextPoint.IsEqual(Snack.Position))
            {
                EatSnack();
            }
            else
            {
                Body.UpdateSnakeBody(Spline.SplinePath, Spline.TempSplinePath);
            }

            if (nextPoint.IsEqualToPointInList(Spline.GetAllPoints()))
            {
                StopSnake();
                GameManager.Instance.GameOver();
            }
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

            Spline.VisualiseNextStepDirection(StepInputDirection, Cube);
        }

        private void EatSnack()
        {
            Snack.AssignNewPosition(Spline.GetAllPoints().ToArray());
            Body.AddSnakeBodyPart(Spline.SplinePath);
            Body.UpdateSnakeBodyAfterSnack();
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

        

        private CubePoint GetPointOnCubeInDirection(DirectionOnCubeSide direction)
        {
            var snakeHead = Spline.GetSnakeHead();

            if (Cube.Sides[(int)snakeHead.SideCoordinate].Dimension.IsPointInDirectionInDimension(snakeHead, direction))
            {
                //ShouldMoveOverEdge = false;
                return snakeHead.GetPointOnSameSide(direction);
            }
            else // snake moves across an edge 
            {
                (CubeSideCoordinate neighborCoordinate, DirectionOnCubeSide neighborDirection) nextSide =
                snakeHead.SideCoordinate.GetNeighborWithDirection(direction);

                CubePoint nextPoint = snakeHead.GetPointOnNeighbour(direction, Cube);

                //ShouldMoveOverEdge = true;
                //TempReferenceDirectionForInput = StepInputDirection.GetInputUpAsDirectionOnCubeSide(nextSide.neighborDirection);
                ReferenceDirectionForInput = StepInputDirection.GetInputUpAsDirectionOnCubeSide(nextSide.neighborDirection);

                GameAudioManager.Instance.SwitchCubeSide(nextPoint.SideCoordinate);

                return nextPoint;
            }
        }

        private void MoveAcrossEdge(CubePoint point)
        {

        }


        private void StopSnake()
        {
            PauseSnake();

            CancelInvoke(nameof(DetermineNextStepDirection));
            CancelInvoke(nameof(UpdateSnake));

            Debug.Log("Game Over");
        }
    }
}
