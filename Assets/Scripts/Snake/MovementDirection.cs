using Unity.Mathematics;
using UnityEngine;

namespace Snake
{
    public enum MovementDirection
    {
        Up,
        Right,
        Down,
        Left
    }

    static class MovementDirectionMethods
    {
        public static Vector3 GetDirectionVector(this MovementDirection direction, float stepLength)
        {
            return direction switch
            {
                MovementDirection.Left => new Vector3(-stepLength, 0, 0),
                MovementDirection.Right => new Vector3(stepLength, 0, 0),
                MovementDirection.Down => new Vector3(0, 0, -stepLength),
                MovementDirection.Up => new Vector3(0, 0, stepLength),
                _ => new Vector3(0, 0, 0)
            };
        }

        public static Quaternion GetRotation(this MovementDirection direction)
        {
            return direction switch
            {
                MovementDirection.Left => Quaternion.Euler(0,270,0),
                MovementDirection.Right => Quaternion.Euler(0,90,0),
                MovementDirection.Down => Quaternion.Euler(0,180,0),
                MovementDirection.Up => Quaternion.Euler(0,0,0),
                _ => Quaternion.Euler(0,0,0)
            };
        }

        public static MovementDirection GetOppositeDirection(this MovementDirection direction)
        {
            return direction switch
            {
                MovementDirection.Left => MovementDirection.Right,
                MovementDirection.Right => MovementDirection.Left,
                MovementDirection.Down => MovementDirection.Up,
                MovementDirection.Up => MovementDirection.Down,
                _ => MovementDirection.Up // TODO Error Logging
            };
        }
    }
}