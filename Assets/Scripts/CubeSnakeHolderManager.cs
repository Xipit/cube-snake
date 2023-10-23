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
                Debug.LogError("Cube could not be rotated!");
                break;
        }
    }
}
