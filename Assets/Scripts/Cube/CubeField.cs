#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeField 
{
    public CubeFieldCoordinate CubeFieldCoordinate { get; }
    public CubeSideCoordinate CubeSideCoordinate { get; }

    public Tunnel? Tunnel = null;

    public CubeField(CubeFieldCoordinate cubeFieldCoordinate, CubeSideCoordinate cubeSideCoordinate, Tunnel[] tunnels)
    {
        this.CubeFieldCoordinate = cubeFieldCoordinate;
        this.CubeSideCoordinate = cubeSideCoordinate;

        CubePoint point = new CubePoint(cubeSideCoordinate, cubeFieldCoordinate);

        for (int i = 0; i < tunnels.Length; i++)
        {
            if (tunnels[i].HasPoint(point))
            {
                Tunnel = tunnels[i];
            }
        }
    }

    public GameObject InstantiateObject(GameObject side, Vector3 positionInWorld, Quaternion rotationInWorld, GameObject prefab)
    {
        return InstantiateManager.Instance.InstantiateGameObjectAsChild(positionInWorld, rotationInWorld, prefab, side.transform);
    }
}
