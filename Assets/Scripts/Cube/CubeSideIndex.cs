using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the "coordinate", or rather the identity of a CubeSide relating to its position in a Cube.
/// </summary>
public enum CubeSideCoordinate
{
    Front   = 0,
    Back    = 1,
    Right   = 2,
    Left    = 3,
    Up      = 4,
    Down    = 5
}

// the following methods are invoked by e.g. CubeSideCoordinate.Left.GetDimension2D(cubeDimension);
static class CubeSideCoordinateMethods
{
    /// <summary>
    /// Translate Dimension3D (Cube) to Dimension2D (CubeSides).
    /// </summary>
    public static Dimension2D GetDimension2D(this CubeSideCoordinate coordinate, Dimension3D cubeDimension)
    {
        switch (coordinate)
        {
            case CubeSideCoordinate.Front:
            case CubeSideCoordinate.Back:
                return new Dimension2D(cubeDimension.X, cubeDimension.Y);

            case CubeSideCoordinate.Right:
            case CubeSideCoordinate.Left:
                return new Dimension2D(cubeDimension.Z, cubeDimension.Y);

            case CubeSideCoordinate.Up:
            case CubeSideCoordinate.Down:
                return new Dimension2D(cubeDimension.X, cubeDimension.Z);

            default:
                Debug.LogError("CubeSideCoordinate not fully mapped in function");
                return new Dimension2D(0, 0);
        }
    }

    /// <summary>
    /// Get rotation of a CubeSide in a Cube to accurately represent a cube in 3D space.
    /// <br/> This works in conjunction with GetPositionInCube().
    /// </summary>
    public static Quaternion GetRotationInCube(this CubeSideCoordinate coordinate)
    {
        switch (coordinate)
        {
            case CubeSideCoordinate.Front:
                return Quaternion.Euler(new Vector3(-90, 0, 0));

            case CubeSideCoordinate.Back:
                return Quaternion.Euler(new Vector3(-90, -180, 0));

            case CubeSideCoordinate.Right:
                return Quaternion.Euler(new Vector3(-90, -90, 0));

            case CubeSideCoordinate.Left:
                return Quaternion.Euler(new Vector3(-90, -270, 0));

            case CubeSideCoordinate.Up:
                return Quaternion.Euler(new Vector3(0, 0, 0));

            case CubeSideCoordinate.Down:
                return Quaternion.Euler(new Vector3(0, -180, 180));

            default:
                Debug.LogError("CubeSideCoordinate not fully mapped in function");
                return Quaternion.identity;
        }
    }

    /// <summary>
    /// Get Position of a CubeSide in a Cube to accurately represent a cube in 3D space.
    /// /// <br/> This works in conjunction with GetRotationInCube().
    /// </summary>
    public static Vector3 GetPositionInCube(
        this CubeSideCoordinate coordinate,
        Dimension3D cubeDimension,
        float cubeScale)
    {
        switch (coordinate)
        {
            case CubeSideCoordinate.Front:
                return cubeDimension.MultiplyBy(new Vector3(-1, -1, -1) * .5F) * cubeScale;

            case CubeSideCoordinate.Back:
                return cubeDimension.MultiplyBy(new Vector3(+1, -1, +1) * .5F) * cubeScale;

            case CubeSideCoordinate.Right:
                return cubeDimension.MultiplyBy(new Vector3(+1, -1, -1) * .5F) * cubeScale;

            case CubeSideCoordinate.Left:
                return cubeDimension.MultiplyBy(new Vector3(-1, -1, +1) * .5F) * cubeScale;

            case CubeSideCoordinate.Up:
                return cubeDimension.MultiplyBy(new Vector3(-1, +1, -1) * .5F) * cubeScale;

            case CubeSideCoordinate.Down:
                return cubeDimension.MultiplyBy(new Vector3(-1, -1, +1) * .5F) * cubeScale;

            default:
                Debug.LogError("CubeSideCoordinate not fully mapped in function");
                return Vector3.zero;
        }
    }

    /// <summary>
    /// When crossing the edge of a CubeSide, this function tells you to which side you are going and in what direction (side-specific).
    /// </summary>
    /// <param name="direction">The direction you are moving. Can be easily calculated by checking Dimension boundaries and your position</param>
    /// <returns>(-CubeSideCoordinate of your target CubeSide- , -The Direction you are going in the target CubeSide- )</returns>
    public static (CubeSideCoordinate neighborCoordinate, DirectionOnCubeSide neighborDirection) GetNeighborWithDirection(this CubeSideCoordinate coordinate, DirectionOnCubeSide direction)
    {
        /*
         *  ^ V (ertical)
         *  |
         *  |   H (orizontal)
         *   ---> 
         */

        switch (coordinate)
        {
            case CubeSideCoordinate.Front:
                switch (direction)
                {
                    case DirectionOnCubeSide.negHor:
                        return (CubeSideCoordinate.Left, DirectionOnCubeSide.negHor);

                    case DirectionOnCubeSide.posHor:
                        return (CubeSideCoordinate.Right, DirectionOnCubeSide.posHor);

                    case DirectionOnCubeSide.negVert:
                        return (CubeSideCoordinate.Down, DirectionOnCubeSide.negVert);

                    case DirectionOnCubeSide.posVert:
                        return (CubeSideCoordinate.Up, DirectionOnCubeSide.posVert);

                    default:
                        Debug.LogError("Direction2D not fully mapped in function");
                        return (neighborCoordinate: CubeSideCoordinate.Front, neighborDirection: DirectionOnCubeSide.posHor);
                }

            case CubeSideCoordinate.Back:
                switch (direction)
                {
                    case DirectionOnCubeSide.negHor:
                        return (CubeSideCoordinate.Right, DirectionOnCubeSide.negHor);

                    case DirectionOnCubeSide.posHor:
                        return (CubeSideCoordinate.Left, DirectionOnCubeSide.posHor);

                    case DirectionOnCubeSide.negVert:
                        return (CubeSideCoordinate.Down, DirectionOnCubeSide.posVert);

                    case DirectionOnCubeSide.posVert:
                        return (CubeSideCoordinate.Up, DirectionOnCubeSide.negVert);

                    default:
                        Debug.LogError("Direction2D not fully mapped in function");
                        return (neighborCoordinate: CubeSideCoordinate.Front, neighborDirection: DirectionOnCubeSide.posHor);
                }

            case CubeSideCoordinate.Right:
                switch (direction)
                {
                    case DirectionOnCubeSide.negHor:
                        return (CubeSideCoordinate.Front, DirectionOnCubeSide.negHor);

                    case DirectionOnCubeSide.posHor:
                        return (CubeSideCoordinate.Back, DirectionOnCubeSide.posHor);

                    case DirectionOnCubeSide.negVert:
                        return (CubeSideCoordinate.Down, DirectionOnCubeSide.posHor);

                    case DirectionOnCubeSide.posVert:
                        return (CubeSideCoordinate.Up, DirectionOnCubeSide.negHor);

                    default:
                        Debug.LogError("Direction2D not fully mapped in function");
                        return (neighborCoordinate: CubeSideCoordinate.Front, neighborDirection: DirectionOnCubeSide.posHor);
                }

            case CubeSideCoordinate.Left:
                switch (direction)
                {
                    case DirectionOnCubeSide.negHor:
                        return (CubeSideCoordinate.Back, DirectionOnCubeSide.negHor);

                    case DirectionOnCubeSide.posHor:
                        return (CubeSideCoordinate.Front, DirectionOnCubeSide.posHor);

                    case DirectionOnCubeSide.negVert:
                        return (CubeSideCoordinate.Down, DirectionOnCubeSide.negHor);

                    case DirectionOnCubeSide.posVert:
                        return (CubeSideCoordinate.Up, DirectionOnCubeSide.posHor);

                    default:
                        Debug.LogError("Direction2D not fully mapped in function");
                        return (neighborCoordinate: CubeSideCoordinate.Front, neighborDirection: DirectionOnCubeSide.posHor);
                }

            case CubeSideCoordinate.Up:
                switch (direction)
                {
                    case DirectionOnCubeSide.negHor:
                        return (CubeSideCoordinate.Left, DirectionOnCubeSide.negVert);

                    case DirectionOnCubeSide.posHor:
                        return (CubeSideCoordinate.Right, DirectionOnCubeSide.negVert);

                    case DirectionOnCubeSide.negVert:
                        return (CubeSideCoordinate.Front, DirectionOnCubeSide.negVert);

                    case DirectionOnCubeSide.posVert:
                        return (CubeSideCoordinate.Back, DirectionOnCubeSide.negVert);

                    default:
                        Debug.LogError("Direction2D not fully mapped in function");
                        return (neighborCoordinate: CubeSideCoordinate.Front, neighborDirection: DirectionOnCubeSide.posHor);
                }

            case CubeSideCoordinate.Down:
                switch (direction)
                {
                    case DirectionOnCubeSide.negHor:
                        return (CubeSideCoordinate.Left, DirectionOnCubeSide.posVert);

                    case DirectionOnCubeSide.posHor:
                        return (CubeSideCoordinate.Right, DirectionOnCubeSide.posVert);

                    case DirectionOnCubeSide.negVert:
                        return (CubeSideCoordinate.Back, DirectionOnCubeSide.posVert);

                    case DirectionOnCubeSide.posVert:
                        return (CubeSideCoordinate.Front, DirectionOnCubeSide.posVert);

                    default:
                        Debug.LogError("Direction2D not fully mapped in function");
                        return (neighborCoordinate: CubeSideCoordinate.Front, neighborDirection: DirectionOnCubeSide.posHor);
                }

            default:
                Debug.LogError("CubeSideCoordinate not fully mapped in function");
                return (neighborCoordinate: CubeSideCoordinate.Front, neighborDirection: DirectionOnCubeSide.posHor);
        }
    }

    public static CubeSideCoordinate GetOpposingCubeSideCoordinate(this CubeSideCoordinate coordinate)
    {
        switch (coordinate)
        {
            case CubeSideCoordinate.Front:
                return CubeSideCoordinate.Back;

            case CubeSideCoordinate.Back:
                return CubeSideCoordinate.Front;

            case CubeSideCoordinate.Right:
                return CubeSideCoordinate.Left;

            case CubeSideCoordinate.Left:
                return CubeSideCoordinate.Right;

            case CubeSideCoordinate.Up:
                return CubeSideCoordinate.Down;

            case CubeSideCoordinate.Down:
                return CubeSideCoordinate.Up;

            default:
                Debug.LogError("CubeSideCoordinate not fully mapped in function");
                return CubeSideCoordinate.Front;
        }
    }

}


// incase i need to program another switch
/*
 * 
 * case CubeSideCoordinate.Front:

            case CubeSideCoordinate.Back:

            case CubeSideCoordinate.Right:

            case CubeSideCoordinate.Left:

            case CubeSideCoordinate.Up:

            case CubeSideCoordinate.Down:
 * 
 * 
 */