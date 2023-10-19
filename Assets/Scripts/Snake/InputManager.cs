using System.Collections;
using System.Collections.Generic;
using Snake;
using UnityEngine;
# nullable enable
public class InputManager : MonoBehaviour
{
    public static InputManager? Instance { get; private set; }

    public MovementDirection? GetPlayerInput(MovementDirection stepDirection)
    {
        MovementDirection? direction = null;

        if (Input.GetAxis("Horizontal") < 0)
        {
            direction =  MovementDirection.Left;
        }
        if (Input.GetAxis("Horizontal") > 0)
        {
            direction =  MovementDirection.Right;
        }
        if (Input.GetAxis("Vertical") < 0)
        {
            direction = MovementDirection.Down;
        }
        if (Input.GetAxis("Vertical") > 0)
        {
            direction = MovementDirection.Up;
        }
        
        
        if (direction == stepDirection.GetOppositeDirection())
        {
            return stepDirection;
        }
        
        return direction;
    }
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
}
