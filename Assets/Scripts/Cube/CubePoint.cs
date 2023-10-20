#nullable enable
#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePoint 
{
    public CubeFieldCoordinate FieldCoordinate { get; }
    public CubeSideCoordinate SideCoordinate { get; }

    public CubePoint(CubeSideCoordinate sideCoordinate, CubeFieldCoordinate fieldCoordinate)
    {
        FieldCoordinate = fieldCoordinate;
        SideCoordinate = sideCoordinate;
    }

    public bool IsEqual(CubePoint point)
    {
        return point.FieldCoordinate.H == this.FieldCoordinate.H
            && point.FieldCoordinate.V == this.FieldCoordinate.V
            && point.SideCoordinate == this.SideCoordinate;
    }
    
    public CubePoint GetPointOnNeighbour(DirectionOnCubeSide currentDirectionOnSide, Cube cube)
        {
            (CubeSideCoordinate neighborCoordinate, DirectionOnCubeSide neighborDirection) nextSide =
                this.SideCoordinate.GetNeighborWithDirection(currentDirectionOnSide);

            int neighbourMaxH = cube.Sides[(int)nextSide.neighborCoordinate].Dimension.H;
            int neighbourMaxV = cube.Sides[(int)nextSide.neighborCoordinate].Dimension.V;

            int currentH = this.FieldCoordinate.H;
            int currentV = this.FieldCoordinate.V;

            CubeFieldCoordinate? nextLocalField = null;

            switch (currentDirectionOnSide)
            {
                // snake moves negHor on current Side
                case DirectionOnCubeSide.negHor:
                    nextLocalField = nextSide.neighborDirection switch
                    {
                        DirectionOnCubeSide.negHor => new CubeFieldCoordinate(neighbourMaxH, currentV),
                        DirectionOnCubeSide.posHor => new CubeFieldCoordinate(0, neighbourMaxV - currentV),
                        DirectionOnCubeSide.negVert => new CubeFieldCoordinate(neighbourMaxH - currentV, neighbourMaxV),
                        DirectionOnCubeSide.posVert => new CubeFieldCoordinate(currentV, 0),
                        _ => null
                    };
                    break;
                
                // snake moves posHor on current Side
                case DirectionOnCubeSide.posHor:
                    nextLocalField = nextSide.neighborDirection switch
                    {
                        DirectionOnCubeSide.negHor => new CubeFieldCoordinate(neighbourMaxH, neighbourMaxV - currentV),
                        DirectionOnCubeSide.posHor => new CubeFieldCoordinate(0, currentV),
                        DirectionOnCubeSide.negVert => new CubeFieldCoordinate(currentV, neighbourMaxV),
                        DirectionOnCubeSide.posVert => new CubeFieldCoordinate(neighbourMaxH - currentV, 0),
                        _ => null
                    };
                    break;
                
                // snake moves negVert on current Side
                case DirectionOnCubeSide.negVert:
                    nextLocalField = nextSide.neighborDirection switch
                    {
                        DirectionOnCubeSide.negHor => new CubeFieldCoordinate(neighbourMaxH, neighbourMaxV - currentH),
                        DirectionOnCubeSide.posHor => new CubeFieldCoordinate(0, currentH),
                        DirectionOnCubeSide.negVert => new CubeFieldCoordinate(currentH, neighbourMaxV),
                        DirectionOnCubeSide.posVert => new CubeFieldCoordinate(neighbourMaxH - currentH, 0),
                        _ => null
                    };
                    break;
                    
                // snake moves posVert on current Side
                case DirectionOnCubeSide.posVert:
                    nextLocalField = nextSide.neighborDirection switch
                    {
                        DirectionOnCubeSide.negHor => new CubeFieldCoordinate(neighbourMaxH, currentH),
                        DirectionOnCubeSide.posHor => new CubeFieldCoordinate(0,  neighbourMaxV - currentH),
                        DirectionOnCubeSide.negVert => new CubeFieldCoordinate(neighbourMaxH - currentH, neighbourMaxV),
                        DirectionOnCubeSide.posVert => new CubeFieldCoordinate(currentH, 0),
                        _ => null
                    };
                    break;
            }

            if (nextLocalField == null)
            {
                Debug.LogError("local field on neighbour side could not be found");
                return new CubePoint(CubeSideCoordinate.Front, new CubeFieldCoordinate(0,0));
            }
            
            return new CubePoint(nextSide.neighborCoordinate, nextLocalField);
        }
}
