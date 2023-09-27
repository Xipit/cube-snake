using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a Cube and all its needed data, such as worldposition and rotation, its CubeSides and CubeFields.
/// </summary>
public class Cube
{
    /*
     *  ^ Z
     *  |
     *  |   Y
     *  | /    
     *   /     X
     *   ------> 
     */

    public Dimension3D dimension;

    // only 1 CubeSide for now to test
    // should be 6 when fully implemented
    public CubeSide prototypeSide;

    public Vector3 position;
    public Quaternion rotation; // probably needs to be a quaternion

    public Cube(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;

        dimension = new Dimension3D(3, 3, 3);

        prototypeSide = new CubeSide(new Dimension2D(dimension.X, dimension.Y));
    }

    /// <summary>
    /// instantiates all needed 3D Meshes to represent this cube in 3D.
    /// </summary>
    public void instantiateMeshes(GameObject fieldPrefab)
    {
        // give CubeSides a starting (A) point and rotation

        /*
         *  Vector3 pointA;
            Vector3 pointB;
            Quaternion rotationA;
            Vector3 result = pointA + (pointB * rotationA);
         */

        // CubeField indexPosition is pointB

        prototypeSide.instantiateMeshes(position, rotation, fieldPrefab);
    }
}