using System;
using System.Collections;
using System.Collections.Generic;
using Snake;
using UnityEngine;

public class CubeSnakeHolderManager : MonoBehaviour
{
    public static CubeSnakeHolderManager Instance { get; private set; }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    
    
    public void RotateCubeSnakeHolder(MovementDirection stepDirection)
    {
        switch (stepDirection)
        {
            case MovementDirection.Up:
                transform.Rotate(Vector3.right, -90, Space.World);
                break;
            case MovementDirection.Right:
                transform.Rotate(Vector3.up, 90, Space.World); 
                break;
            case MovementDirection.Down:
                transform.Rotate(Vector3.right, 90, Space.World);
                break;
            case MovementDirection.Left:
                transform.Rotate(Vector3.up, -90, Space.World);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(stepDirection), stepDirection, null);
        }
    }

    public DirectionOnCubeSide TranslateStepDirectionToLocalDirectionOnSide(MovementDirection stepDirection, DirectionOnCubeSide currentInputUpDirection)
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

    // TODO is there a better file/script for this method?

    /// <summary>
    /// On each side the local coordinates have a different rotation. This Method returns the DirectionOnCubeSide where snake should go by typing Input.Up.
    /// </summary>
    /// <param name="stepDirection">Direction of the movement of the snake relativ to the users view.</param>
    /// <param name="nextSideDirection">DirectionOnCubeSide of the side where the snake will go</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public DirectionOnCubeSide GetInputUpAsDirectionOnCubeSide(MovementDirection stepDirection, DirectionOnCubeSide nextSideDirection)
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
