using System;
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
        public static MovementDirection GetOppositeDirection(this MovementDirection direction)
        {
            return direction switch
            {
                MovementDirection.Left => MovementDirection.Right,
                MovementDirection.Right => MovementDirection.Left,
                MovementDirection.Down => MovementDirection.Up,
                MovementDirection.Up => MovementDirection.Down,
                _ => direction
            };
        }

        /// <summary>
        /// Translates the stepDirection to the local coordinate system of a cubeSide.
        /// </summary>
        public static DirectionOnCubeSide ToLocalDirectionOnCubeSide(this MovementDirection stepDirection,
            DirectionOnCubeSide currentInputUpDirection)
        {
            return stepDirection switch
            {
                MovementDirection.Up => currentInputUpDirection,
                MovementDirection.Right => currentInputUpDirection switch
                {
                    DirectionOnCubeSide.negHor => DirectionOnCubeSide.posVert,
                    DirectionOnCubeSide.posHor => DirectionOnCubeSide.negVert,
                    DirectionOnCubeSide.negVert => DirectionOnCubeSide.negHor,
                    DirectionOnCubeSide.posVert => DirectionOnCubeSide.posHor,
                    _ => throw new ArgumentOutOfRangeException(nameof(currentInputUpDirection), currentInputUpDirection,
                        null)
                },
                MovementDirection.Down => currentInputUpDirection switch
                {
                    DirectionOnCubeSide.negHor => DirectionOnCubeSide.posHor,
                    DirectionOnCubeSide.posHor => DirectionOnCubeSide.negHor,
                    DirectionOnCubeSide.negVert => DirectionOnCubeSide.posVert,
                    DirectionOnCubeSide.posVert => DirectionOnCubeSide.negVert,
                    _ => throw new ArgumentOutOfRangeException(nameof(currentInputUpDirection), currentInputUpDirection,
                        null)
                },
                MovementDirection.Left => currentInputUpDirection switch
                {
                    DirectionOnCubeSide.negHor => DirectionOnCubeSide.negVert,
                    DirectionOnCubeSide.posHor => DirectionOnCubeSide.posVert,
                    DirectionOnCubeSide.negVert => DirectionOnCubeSide.posHor,
                    DirectionOnCubeSide.posVert => DirectionOnCubeSide.negHor,
                    _ => throw new ArgumentOutOfRangeException(nameof(currentInputUpDirection), currentInputUpDirection,
                        null)
                },
                _ => throw new ArgumentOutOfRangeException(nameof(stepDirection), stepDirection, null)
            };
        }

        /// <summary>
        /// On each side the local coordinates have a different rotation. This Method returns the DirectionOnCubeSide where snake should go by pressing Input.Up.
        /// </summary>
        /// <param name="stepDirection">Direction of the movement of the snake relativ to the users view.</param>
        /// <param name="nextSideDirection">DirectionOnCubeSide of the side where the snake will go</param>
        public static DirectionOnCubeSide GetInputUpAsDirectionOnCubeSide(this MovementDirection stepDirection,
            DirectionOnCubeSide nextSideDirection)
        {
            return stepDirection switch
            {
                MovementDirection.Up => nextSideDirection,
                MovementDirection.Down => nextSideDirection.InvertDirection(),
                MovementDirection.Right => nextSideDirection switch
                {
                    DirectionOnCubeSide.negHor => DirectionOnCubeSide.negVert,
                    DirectionOnCubeSide.posHor => DirectionOnCubeSide.posVert,
                    DirectionOnCubeSide.negVert => DirectionOnCubeSide.posHor,
                    DirectionOnCubeSide.posVert => DirectionOnCubeSide.negHor,
                    _ => throw new ArgumentOutOfRangeException(nameof(nextSideDirection), nextSideDirection, null)
                },
                MovementDirection.Left => nextSideDirection switch
                {
                    DirectionOnCubeSide.negHor => DirectionOnCubeSide.posVert,
                    DirectionOnCubeSide.posHor => DirectionOnCubeSide.negVert,
                    DirectionOnCubeSide.negVert => DirectionOnCubeSide.negHor,
                    DirectionOnCubeSide.posVert => DirectionOnCubeSide.posHor,
                    _ => throw new ArgumentOutOfRangeException(nameof(nextSideDirection), nextSideDirection, null)
                },
                _ => throw new ArgumentOutOfRangeException(nameof(stepDirection), stepDirection, null)
            };
        }
    }
}