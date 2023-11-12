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
        private Tunnel? Tunnel = null;

        public void StartSnake(Cube cube, CubeSideCoordinate startSide, Snack snack, GameMode mode)
        {
            // This defines how the cube is rotated on the spawnSide, so that _Up_ Input matches _Up_ on the screen
            ReferenceDirectionForInput = DirectionOnCubeSide.posVert;

            // stepInterval = 0.4 / 2 = 0.2
            this.StepInterval = this.StepInterval / mode.speedFactor;
            
            this.Spline = new SnakeSpline(this.transform, cube, startSide, ReferenceDirectionForInput);
            this.Body = new SnakeBody(cube, StepInterval, Spline.SplinePath, this.transform, SnakeHeadPrefab, SnakeBodyPrefab, SnakeTailPrefab, EmptyPrefab);
            this.Cube = cube;
            this.Snack = snack;

            StepInputDirection = InputDirection;

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
            ReferenceDirectionForInput = Spline.ReferenceDirectionForInput;
            DirectionOnCubeSide stepDirectionOnCubeSide = StepInputDirection.ToLocalDirectionOnCubeSide(ReferenceDirectionForInput);
            CubePoint nextPoint = GetPointOnCubeInDirection(stepDirectionOnCubeSide);
            
            foreach (Tunnel tunnel in Cube.Tunnels)
            {
                if (tunnel.HasPoint(nextPoint))
                {
                    Tunnel = tunnel;
                    Tunnel.Entry = nextPoint;
                    Tunnel.IsSnakeHeadInTunnel = true;
                    
                    Spline.SetTunnel(Tunnel, nextPoint);
                }
            }
            Spline.UpdateSpline(stepDirectionOnCubeSide, StepInputDirection, nextPoint, Cube, ReferenceDirectionForInput, ShouldGrowNextUpdate);
            ShouldGrowNextUpdate = false;

            // check if the snack is going to be eaten by the snake
            if (nextPoint.IsEqual(Snack.Position))
            {
                EatSnack();
            }
            else
            {
                bool tunnelContainsSnakeBodyPart = Spline.TunnelContainsSnakeBodyPart();
                bool headGoesThroughTunnel = Tunnel?.IsSnakeHeadInTunnel ?? false;
                Body.UpdateSnakeBody(Spline.SplinePath, Spline.TempSplinePath, headGoesThroughTunnel, Spline.CurrentStepsInsideTunnel, tunnelContainsSnakeBodyPart);
            }

            if (nextPoint.IsEqualToPointInList(Spline.GetAllPointsWithoutSnakeHead()))
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
            if (Tunnel is {IsSnakeHeadInTunnel: true})
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
            GameManager.Instance.AddSnakeLength();
            ShouldGrowNextUpdate = true;
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
                RotationManager.Instance.RotateOneSide(StepInputDirection, nextPoint, Cube.Dimension);
                
                GameAudioManager.Instance.SwitchCubeSide(nextPoint.SideCoordinate);

                return nextPoint;
            }
        }

        private void StopSnake()
        {
            Body.PauseSnake();

            CancelInvoke(nameof(DetermineNextStepDirection));
            CancelInvoke(nameof(UpdateSnake));

            Debug.Log("Game Over");
        }
    }
}
