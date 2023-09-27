using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeField 
{
    public int indexX;
    public int indexY;

    public bool isTunnel = false;

    public CubeField(int indexX, int indexY)
    {
        this.indexX = indexX;
        this.indexY = indexY;
    }
}
