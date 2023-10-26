using System;
using System.Collections;
using System.Collections.Generic;
using Snake;
using UnityEngine;

public class RotationReferenceManager : MonoBehaviour
{
    public Vector3 ReferencePosition;

    public Quaternion SideRotation;

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

    private void Start()
    {
        ReferencePosition = Camera.main.transform.position;
    }

    public void Rotate(InputDirection stepDirection)
    {
        return;
        transform.rotation = SideRotation;

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

        SideRotation = transform.rotation;
    }

    // WIP
    // https://stackoverflow.com/questions/1171849/finding-quaternion-representing-the-rotation-from-one-vector-to-another

    public void RotateEveryStep(InputDirection stepDirection, CubePoint point, Dimension3D cubeDimension)
    {
        Dimension2D sideDimension = point.SideCoordinate.GetDimension2D(cubeDimension);

        // promising
        // probably need to "cleanse" additional

        // might just need to animate "Rotate" function
        

        switch (stepDirection)
        {
            case InputDirection.Up:
                transform.Rotate(Vector3.right, -90 / sideDimension.V, Space.World);
                break;
            case InputDirection.Right:
                transform.Rotate(Vector3.up, 90 / sideDimension.H, Space.World);
                break;
            case InputDirection.Down:
                transform.Rotate(Vector3.right, 90 / sideDimension.V, Space.World);
                break;
            case InputDirection.Left:
                transform.Rotate(Vector3.up, -90 / sideDimension.H, Space.World);
                break;
            default:
                Debug.LogError("Cube could not be rotated!");
                break;
        }
    }

    public void RotateToCubePoint(CubePoint point, Dimension3D cubeDimension)
    {
        Debug.Log(point);

        Dimension2D sideDimension = point.SideCoordinate.GetDimension2D(cubeDimension);

        float maxH = sideDimension.H;
        float maxV = sideDimension.V;

        Vector2 normalizedPositionOnSide = new Vector2(
            point.FieldCoordinate.H / (maxH - 1),
            point.FieldCoordinate.V / (maxV - 1));

        Debug.Log(normalizedPositionOnSide);

        Quaternion adjustedRotation = SideRotation * Quaternion.Euler(new Vector3(
            (normalizedPositionOnSide.y - 0.5F) * 30,
            (normalizedPositionOnSide.x - 0.5F) * 30,
            0));

        transform.rotation = adjustedRotation;

    }


    public void CorrectRotate(Vector3 snakeHeadWorldPosition)
    {
        return;
        Vector3 cubeOrigin = new Vector3(0, 0, 0);
        Vector3 snakeHeadOffset = snakeHeadWorldPosition - cubeOrigin;

        float cameraHeight = 2;

        Vector3 cameraOffset = snakeHeadOffset * cameraHeight;
        Debug.Log("cameraOffset: " + cameraOffset);

        Vector3 referencePointOffset = ReferencePosition - cubeOrigin;
        Debug.Log("referencePointOffset: " + referencePointOffset);

        Vector3 crossProduct = Vector3.Cross(cameraOffset, referencePointOffset);

        Quaternion rotation = new Quaternion();

        rotation.x = crossProduct.x;
        rotation.y = crossProduct.y;
        rotation.z = crossProduct.z;

        rotation.w = Mathf.Sqrt((cameraOffset.sqrMagnitude * referencePointOffset.sqrMagnitude) + Vector3.Dot(cameraOffset, referencePointOffset));

        rotation.Normalize();

        transform.rotation = Quaternion.Inverse(rotation);
        transform.rotation = rotation;

        Debug.Log(snakeHeadWorldPosition);
        Debug.Log(rotation);
    }

    /* 

        calculate as if camera follows snake,
        then just rotate everything back to starting point

        World Positions:
        - Cube Origin (Middle of Cube)
        - snake Head Position

        calculate Vector between
        scale vector to camera Height

        calculate Vector between cube origin and starting Point
        get angle between these vectors



        rotate cube with angle

     */
}
