using System;
using System.Collections;
using System.Collections.Generic;
using Snake;
using UnityEngine;

public class RotationReferenceManager : MonoBehaviour
{
    public static RotationReferenceManager Instance { get; private set; }

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

    public void Rotate(InputDirection stepDirection)
    {
        switch (stepDirection)
        {
            case InputDirection.Up:
                transform.Rotate(Vector3.right, -90, Space.World);
                break;
            case InputDirection.Right:
                transform.Rotate(Vector3.up, 90, Space.World); 
                break;
            case InputDirection.Down:
                transform.Rotate(Vector3.right, 90, Space.World);
                break;
            case InputDirection.Left:
                transform.Rotate(Vector3.up, -90, Space.World);
                break;
            default:
                Debug.LogError("Cube could not be rotated!");
                break;
        }
    }
}
