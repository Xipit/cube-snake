using System;
using System.Collections;
using System.Collections.Generic;
using Snake;
using UnityEngine;
using Extensions;

/// <summary>
/// Responsible for rotating the cube (and snake).
/// <br/> Utilies a reference GameObject (RotationOrigin), which gets rotated first.
/// <br/> This Object will then smoothly lerp to the rotation of the reference GameObject.
/// </summary>
public class RotationManager : MonoBehaviour
{
    public GameObject RotationOrigin;

    private float RotationPerSide = 70;
    private float RotationSpeed = 0.4F;

    public static RotationManager Instance { get; private set; }

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

    private void Start()
    {
        if(!RotationOrigin)
        {
            Debug.LogError("No RotationOrigin referenced. Assign an empty GameObject!");
            return;
        }
    }

    private void Update()
    {
        AlignToRotationOrigin();
    }

    private void AlignToRotationOrigin()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, RotationOrigin.transform.rotation, RotationSpeed);
    }

    /// <summary>
    /// Rotate the cube when crossing the edge.
    /// </summary>
    /// <param name="stepDirection"></param>
    public void RotateOneSide(InputDirection stepDirection, CubePoint point, Dimension3D cubeDimension)
    {
        RotateOrigin(stepDirection, RotationPerSide, true);

        // Clean rotation drift, which will otherwise accumulate over time
        SnapRotationTo90Degrees();
        RotateToCubePoint(point, cubeDimension);
    }

    /// <summary>
    /// Rotate Cube every step in the direction of the step.
    /// </summary>
    public void RotateEveryStep(InputDirection stepDirection, CubePoint point, Dimension3D cubeDimension)
    {
        Dimension2D sideDimension = point.SideCoordinate.GetDimension2D(cubeDimension);

        switch (stepDirection)
        {
            case InputDirection.Up:
            case InputDirection.Down:
                RotateOrigin(stepDirection, (90 - RotationPerSide) / sideDimension.V);
                break;


            case InputDirection.Right:
            case InputDirection.Left:
                RotateOrigin(stepDirection, (90 - RotationPerSide) / sideDimension.H);
                break;

            default:
                Debug.LogError("Cube could not be rotated!");
                break;
        }
    }

    /// <summary>
    /// Rotate cube to align with cube point.
    /// </summary>
    public void RotateToCubePoint(CubePoint point, Dimension3D cubeDimension)
    {
        Dimension2D sideDimension = point.SideCoordinate.GetDimension2D(cubeDimension);
        CubeFieldCoordinate fieldCoordinate = point.FieldCoordinate;

        float horizontalAmount  = ((float)fieldCoordinate.H / sideDimension.H) - 0.5F;
        float verticalAmount    = ((float)fieldCoordinate.V / sideDimension.V) - 0.5F;

        RotateOrigin(InputDirection.Up, horizontalAmount * (90 - RotationPerSide));
        RotateOrigin(InputDirection.Right, verticalAmount * (90 - RotationPerSide));
    }

    private void RotateOrigin(InputDirection stepDirection, float amount, bool fastRotate = false)
    {
        switch (stepDirection)
        {
            case InputDirection.Up:
                RotationOrigin.transform.Rotate(Vector3.right, -amount, Space.World);
                break;

            case InputDirection.Right:
                RotationOrigin.transform.Rotate(Vector3.up, amount, Space.World);
                break;

            case InputDirection.Down:
                RotationOrigin.transform.Rotate(Vector3.right, amount, Space.World);
                break;

            case InputDirection.Left:
                RotationOrigin.transform.Rotate(Vector3.up, -amount, Space.World);
                break;

            default:
                Debug.LogError("Cube could not be rotated!");
                break;
        }

        if (fastRotate)
        {
            RotationSpeed = 0.4F;
        }
        else
        {
            RotationSpeed = 0.05F;
        }
    }

    private void SnapRotationTo90Degrees()
    {
        Quaternion cleanedRotation = RotationOrigin.transform.rotation.SnapToNearestRightAngle();

        RotationOrigin.transform.rotation = cleanedRotation;
    }

    
}
