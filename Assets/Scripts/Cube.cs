using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

/// <summary>
/// Represents a Cube and all its needed data, such as worldposition and rotation, its CubeSides and CubeFields.
/// </summary>
public class Cube
{
    public Vector3 position;
    public Quaternion rotation;
    public float size;

    public Dimension3D dimension;

    // only 1 CubeSide for now to test
    // should be 6 when fully implemented
    public CubeSide[] sides = new CubeSide[6];

    public Cube(Vector3 position, Quaternion rotation, Dimension3D dimension, float size = 1f)
    {
        this.position = position;
        this.rotation = rotation;
        this.size = size;

        this.dimension = dimension;

        sides = createSides(dimension);
    }

    private CubeSide[] createSides(Dimension3D dimension)
    {
        CubeSide[] sidesHolder = new CubeSide[6];

        /*
         * 2D Dimension of how many fields are in the CubeSide
         * Position in Cube (Origin is in the Middle of the Cube)
         * Rotation in Cube
         */
        
        sidesHolder[0] = new CubeSide(
            new Dimension2D(dimension.X, dimension.Z),
            dimension.MultiplyBy(new Vector3(-1, +1, -1) * .5F)    * size,
            Quaternion.Euler(new Vector3(0,0,0)));

        sidesHolder[1] = new CubeSide(
            new Dimension2D(dimension.X, dimension.Y),
            dimension.MultiplyBy(new Vector3(-1, -1, -1) * .5F)    * size,
            Quaternion.Euler(new Vector3(-90, 0, 0)));

        sidesHolder[2] = new CubeSide(
            new Dimension2D(dimension.Z, dimension.Y),
            dimension.MultiplyBy(new Vector3(+1, -1, -1) * .5F)    * size,
            Quaternion.Euler(new Vector3(-90, -90, 0)));

        sidesHolder[3] = new CubeSide(
            new Dimension2D(dimension.X, dimension.Y),
            dimension.MultiplyBy(new Vector3(+1, -1, +1) * .5F)    * size,
            Quaternion.Euler(new Vector3(-90, -180, 0)));

        sidesHolder[4] = new CubeSide(
            new Dimension2D(dimension.Z, dimension.Y),
            dimension.MultiplyBy(new Vector3(-1, -1, +1) * .5F)    * size,
            Quaternion.Euler(new Vector3(-90, -270, 0)));

        sidesHolder[5] = new CubeSide(
            new Dimension2D(dimension.X, dimension.Z),
            dimension.MultiplyBy(new Vector3(-1, -1, +1) * .5F)    * size,
            Quaternion.Euler(new Vector3(-180, 0, 0)));

        return sidesHolder;
    }

    /// <summary>
    /// instantiates all needed 3D Meshes to represent this cube in 3D.
    /// </summary>
    public void instantiateObjects(GameObject fieldPrefab)
    {
        GameObject cube = new GameObject("Cube (" + dimension.X + ", " + dimension.Y + ", " + dimension.Z + ")");
        cube.transform.position = position;
        cube.transform.rotation = rotation;

        for (int i = 0; i < sides.Length; i++)
        {
            if(sides[i] != null)
            {
                GameObject side = sides[i].instantiateObjects(position, rotation, size, fieldPrefab);

                side.transform.parent = cube.transform;
            }

        }
    }
}