using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TutorialInfo.Scripts.Snake;
using UnityEditor;
using UnityEngine;

public class Line : MonoBehaviour
{
    public Vector3 p0;
    public List<Vector3> path;
    public List<Vector3> newPath;
    public float lengthOfMove = 1;
    public MovementDirection movementDirection = MovementDirection.Right;
    private MovementDirection currentDirection;
    private Vector3 movementDirectionVector;
    public float snakeUpdateInterval = .75f;

    private void Start()
    {
        path.Add(p0);
        path.Add(new Vector3(p0.x + lengthOfMove, p0.y, p0.z));
        path.Add(new Vector3(p0.x + 2 * lengthOfMove, p0.y, p0.z));
        path.Add(new Vector3(p0.x + 3 * lengthOfMove, p0.y, p0.z));
        path.Add(new Vector3(p0.x + 4 * lengthOfMove, p0.y, p0.z));
        
        movementDirectionVector = new Vector3(lengthOfMove, 0, 0);
        currentDirection = movementDirection;

        InvokeRepeating("UpdateSnake", 1f, snakeUpdateInterval);
    }

    private void Update()
    {
        if (Input.GetAxis("Horizontal") < 0 && currentDirection != MovementDirection.Right)
        {
            movementDirection = MovementDirection.Left;
            movementDirectionVector = new Vector3(-lengthOfMove, 0, 0);
        }

        if (Input.GetAxis("Horizontal") > 0 && currentDirection != MovementDirection.Left)
        {
            movementDirection = MovementDirection.Right;
            movementDirectionVector = new Vector3(lengthOfMove, 0, 0);
        }

        if (Input.GetAxis("Vertical") < 0 && currentDirection != MovementDirection.Up)
        {
            movementDirection = MovementDirection.Down;
            movementDirectionVector = new Vector3(0, 0, -lengthOfMove);
        }

        if (Input.GetAxis("Vertical") > 0 && currentDirection != MovementDirection.Down)
        {
            movementDirection = MovementDirection.Up;
            movementDirectionVector = new Vector3(0, 0, lengthOfMove);
        }
    }

    private void UpdateSnake()
    {
        path.Add(path.Last() + movementDirectionVector);
        path.RemoveAt(0);
        currentDirection = movementDirection;

        // newPath = new List<Vector3>();
        // foreach (var point in path)
        // {
        //     newPath.Add(new Vector3(point.x + lengthOfMove, point.y, point.z));
        // }
        //
        // path = newPath;
    }
}