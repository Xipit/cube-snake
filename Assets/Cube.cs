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
    public Vector3 rotation; // probably needs to be a quaternion

    public Cube()
    {
        dimension = new Dimension3D(3, 3, 3);

        prototypeSide = new CubeSide(new Dimension2D(dimension.X, dimension.Y));
    }

    /// <summary>
    /// instantiates all needed 3D Meshes to represent this cube in 3D.
    /// </summary>
    public void instantiateMeshes()
    {
        
    }
}