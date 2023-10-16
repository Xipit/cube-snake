using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Snake
{
    public class Spline : MonoBehaviour
    {
        public SplineContainer splinePath;

        public float lengthOfMove;
        public float snakeUpdateInterval;
    
        public MovementDirection movementDirection;
        private MovementDirection currentDirection;

        private void Start()
        {
            currentDirection = movementDirection;
            InvokeRepeating(nameof(UpdateSpline), snakeUpdateInterval, snakeUpdateInterval);
            InvokeRepeating(nameof(UpdateCurrentDirection), snakeUpdateInterval * 0.75f, snakeUpdateInterval);
        }
    
        private void Update()
        {
            UpdateMovementDirection();
        }

        private void UpdateCurrentDirection()
        {
            var lastKnot = splinePath.Spline.ToArray().Last();

            if (currentDirection != movementDirection)
            {
                lastKnot.Position += (GetDirectionVector(movementDirection) -
                                      GetDirectionVector(currentDirection)) / 2;
                lastKnot.Rotation = GetRotation(movementDirection);
            }
        
            splinePath.Spline.SetKnot(splinePath.Spline.Count - 1, lastKnot);
            currentDirection = movementDirection;
        }
    
        private void UpdateSpline()
        {
            var lastKnot = splinePath.Spline.ToArray().Last();
            var newKnot = new BezierKnot();

            newKnot.Position = lastKnot.Position + GetDirectionVector(currentDirection);

            newKnot.Rotation = GetRotation(currentDirection);
            newKnot.TangentIn = new float3(0, 0, -0.33f);
            newKnot.TangentOut = new float3(0, 0, 0.33f);
 
            splinePath.Spline.Add(newKnot);
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

        private float3 GetDirectionVector(MovementDirection mD)
        {
            return mD switch
            {
                MovementDirection.Left => new float3(-lengthOfMove, 0, 0),
                MovementDirection.Right => new float3(lengthOfMove, 0, 0),
                MovementDirection.Down => new float3(0, 0, -lengthOfMove),
                MovementDirection.Up => new float3(0, 0, lengthOfMove),
                _ => new float3(0, 0, 0)
            };
        }

        private Quaternion GetRotation(MovementDirection mD)
        {
            return mD switch
            {
                MovementDirection.Left => Quaternion.Euler(0,270,0),
                MovementDirection.Right => Quaternion.Euler(0,90,0),
                MovementDirection.Down => Quaternion.Euler(0,180,0),
                MovementDirection.Up => Quaternion.Euler(0,0,0),
                _ => Quaternion.Euler(0,0,0)
            };
        }
    }
}
