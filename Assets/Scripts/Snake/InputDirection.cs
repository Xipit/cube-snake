using System;
using Unity.Mathematics;
using UnityEngine;

namespace Snake
{
    public enum InputDirection
    {
        Up,     // W
        Right,  // D
        Down,   // S
        Left    // A
    }

    static class InputDirectionMethods
    {
        public static InputDirection GetOppositeDirection(this InputDirection direction)
        {
            return direction switch
            {
                InputDirection.Left => InputDirection.Right,
                InputDirection.Right => InputDirection.Left,
                InputDirection.Down => InputDirection.Up,
                InputDirection.Up => InputDirection.Down,
                _ => direction
            };
        }

        /// <summary>
        /// Translates the movementDirection to the local coordinate system of a cubeSide.
        /// </summary>
        public static DirectionOnCubeSide ToLocalDirectionOnCubeSide(this InputDirection movementDirection,
            DirectionOnCubeSide currentInputUpDirection)
        {
            return movementDirection switch
            {
                // 
                InputDirection.Up => currentInputUpDirection,
                InputDirection.Right => currentInputUpDirection switch
                {
                    DirectionOnCubeSide.negHor => DirectionOnCubeSide.posVert,
                    DirectionOnCubeSide.posHor => DirectionOnCubeSide.negVert,
                    DirectionOnCubeSide.negVert => DirectionOnCubeSide.negHor,
                    DirectionOnCubeSide.posVert => DirectionOnCubeSide.posHor,
                    _ => throw new ArgumentOutOfRangeException(nameof(currentInputUpDirection), currentInputUpDirection,
                        null)
                },
                InputDirection.Down => currentInputUpDirection switch
                {
                    DirectionOnCubeSide.negHor => DirectionOnCubeSide.posHor,
                    DirectionOnCubeSide.posHor => DirectionOnCubeSide.negHor,
                    DirectionOnCubeSide.negVert => DirectionOnCubeSide.posVert,
                    DirectionOnCubeSide.posVert => DirectionOnCubeSide.negVert,
                    _ => throw new ArgumentOutOfRangeException(nameof(currentInputUpDirection), currentInputUpDirection,
                        null)
                },
                InputDirection.Left => currentInputUpDirection switch
                {
                    DirectionOnCubeSide.negHor => DirectionOnCubeSide.negVert,
                    DirectionOnCubeSide.posHor => DirectionOnCubeSide.posVert,
                    DirectionOnCubeSide.negVert => DirectionOnCubeSide.posHor,
                    DirectionOnCubeSide.posVert => DirectionOnCubeSide.negHor,
                    _ => throw new ArgumentOutOfRangeException(nameof(currentInputUpDirection), currentInputUpDirection,
                        null)
                },
                _ => throw new ArgumentOutOfRangeException(nameof(movementDirection), movementDirection, null)
            };
        }

        /// <summary>
        /// On each side the local coordinates have a different rotation. This Method returns the DirectionOnCubeSide where snake should go by pressing Input.Up.
        /// </summary>
        /// <param name="stepDirection">Direction of the movement of the snake relativ to the users view.</param>
        /// <param name="nextSideDirection">DirectionOnCubeSide of the side where the snake will go</param>
        public static DirectionOnCubeSide GetInputUpAsDirectionOnCubeSide(this InputDirection stepDirection,
            DirectionOnCubeSide nextSideDirection)
        {
            return stepDirection switch
            {
                InputDirection.Up => nextSideDirection,
                InputDirection.Down => nextSideDirection.InvertDirection(),
                InputDirection.Right => nextSideDirection switch
                {
                    DirectionOnCubeSide.negHor => DirectionOnCubeSide.negVert,
                    DirectionOnCubeSide.posHor => DirectionOnCubeSide.posVert,
                    DirectionOnCubeSide.negVert => DirectionOnCubeSide.posHor,
                    DirectionOnCubeSide.posVert => DirectionOnCubeSide.negHor,
                    _ => throw new ArgumentOutOfRangeException(nameof(nextSideDirection), nextSideDirection, null)
                },
                InputDirection.Left => nextSideDirection switch
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