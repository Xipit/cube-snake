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

    public static Vector2 ToVector2(this DirectionOnCubeSide direction)
    {
        return (direction switch
        {
            DirectionOnCubeSide.negHor => new Vector2(-1, 0),
            DirectionOnCubeSide.posHor => new Vector2(1, 0),
            DirectionOnCubeSide.negVert => new Vector2(0, -1),
            DirectionOnCubeSide.posVert => new Vector2(0, 1),
            _ => Vector2.zero
        });
    }
}