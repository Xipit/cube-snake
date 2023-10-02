using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Snake
{
    public class Path : MonoBehaviour
    {
        public Vector3 startPosition;
        public List<Vector3> path;
        
        public int startLength;
        public float lengthOfMove;
        
        public MovementDirection movementDirection;
        private MovementDirection currentDirection;
        private Vector3 movementDirectionVector;
        
        public float snakeUpdateInterval;
        public int stepsPerMove;
        private int stepsPerMoveCount = 0;
        private void Start()
        {
            // Set MovementDirection and Vector
            UpdateMovementDirection();
            movementDirectionVector = GetDirectionVector();
            currentDirection = movementDirection;
            
            // Initialize first points of the path
            InitializePath();

            // Update Points of path for the snake --> Movement
            InvokeRepeating(nameof(UpdateSnake), 1f, snakeUpdateInterval / stepsPerMove);
        }

        private void Update()
        {
            UpdateMovementDirection();
        }

        private void UpdateSnake()
        {
            // change movementDirection of the snake
            if (stepsPerMoveCount >= stepsPerMove)
            {
                movementDirectionVector = GetDirectionVector();
                currentDirection = movementDirection;
                stepsPerMoveCount = 0;
            }
            
            // move snake
            path.Add(path.Last() + movementDirectionVector / stepsPerMove);
            path.RemoveAt(0);

            stepsPerMoveCount++;
        }

        private void UpdateMovementDirection()
        {
            if (Input.GetAxis("Horizontal") < 0 && currentDirection != MovementDirection.Right)
            {
                movementDirection = MovementDirection.Left;
            }

            if (Input.GetAxis("Horizontal") > 0 && currentDirection != MovementDirection.Left)
            {
                movementDirection = MovementDirection.Right;
            }

            if (Input.GetAxis("Vertical") < 0 && currentDirection != MovementDirection.Up)
            {
                movementDirection = MovementDirection.Down;
            }

            if (Input.GetAxis("Vertical") > 0 && currentDirection != MovementDirection.Down)
            {
                movementDirection = MovementDirection.Up;
            }
        }

        public Vector3 GetDirectionVector()
        {
            return movementDirection switch
            {
                MovementDirection.Left => movementDirectionVector = new Vector3(-lengthOfMove, 0, 0),
                MovementDirection.Right => movementDirectionVector = new Vector3(lengthOfMove, 0, 0),
                MovementDirection.Down => movementDirectionVector = new Vector3(0, 0, -lengthOfMove),
                MovementDirection.Up => movementDirectionVector = new Vector3(0, 0, lengthOfMove),
                _ => new Vector3(0, 0, 0)
            };
        }

        private void InitializePath()
        {
            path.Add(startPosition);
        
            for (int i = 0; i < (startLength * stepsPerMove); i++)
            {
                // path.Add(new Vector3(startPosition.x + ((i * lengthOfMove) / stepsPerMove), startPosition.y, startPosition.z));
                
                path.Add(path.Last() + movementDirectionVector / stepsPerMove);
            }
        }
    }
}