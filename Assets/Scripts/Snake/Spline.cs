using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Snake;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class Spline : MonoBehaviour
{
    public SplineContainer SplinePath;

    public float lengthOfMove;
    public float snakeUpdateInterval;
    
    public MovementDirection movementDirection;
    private MovementDirection currentDirection;

    private void Start()
    {
        currentDirection = movementDirection;
        InvokeRepeating(nameof(SplineUpdate), 1f, snakeUpdateInterval);
    }
    
    private void Update()
    {
        UpdateMovementDirection();
    }

    private void SplineUpdate()
    {
        var lastKnot = SplinePath.Spline.ToArray().Last();
        var newKnot = new BezierKnot();
        
        if (currentDirection != movementDirection)
        {
            newKnot.Position = lastKnot.Position + (GetDirectionVector(currentDirection) +
                               GetDirectionVector(movementDirection)) / 2;
        }
        else
        {
            newKnot.Position = lastKnot.Position + GetDirectionVector(movementDirection);
        }

        newKnot.Rotation = GetRotation();
        newKnot.TangentIn = new float3(0, 0, -0.33f);
        newKnot.TangentOut = new float3(0, 0, 0.33f);
 
        SplinePath.Spline.Add(newKnot);

        currentDirection = movementDirection;
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
    
    public float3 GetDirectionVector(MovementDirection mD)
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

    public Quaternion GetRotation()
    {
        return movementDirection switch
        {
            MovementDirection.Left => Quaternion.Euler(0,270,0),
            MovementDirection.Right => Quaternion.Euler(0,90,0),
            MovementDirection.Down => Quaternion.Euler(0,180,0),
            MovementDirection.Up => Quaternion.Euler(0,0,0),
            _ => Quaternion.Euler(0,0,0)
        };
    }
}
