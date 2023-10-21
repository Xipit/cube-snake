using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Direction on a cube side
/// <br/> 
/// <br/>^ Vertical
/// <br/>|   
/// <br/> ---> Horizontal
/// </summary>
public enum DirectionOnCubeSide
{
    negHor, // negative Horizontal
    posHor, // positive Horizontal
    negVert, // negative Vertical
    posVert // positive Vertical
}


/*
 *  ^ V (ertical)
 *  |
 *  |   H (orizontal)
 *   ---> 
 */


static class DirectionOnCubeSideMethods
{
    public static DirectionOnCubeSide InvertDirection(this DirectionOnCubeSide direction)
    {
        return direction switch
        {
            DirectionOnCubeSide.negHor => DirectionOnCubeSide.posHor,
            DirectionOnCubeSide.posHor => DirectionOnCubeSide.negHor,
            DirectionOnCubeSide.negVert => DirectionOnCubeSide.posVert,
            DirectionOnCubeSide.posVert => DirectionOnCubeSide.negVert,
            _ => direction
        };
    }
}