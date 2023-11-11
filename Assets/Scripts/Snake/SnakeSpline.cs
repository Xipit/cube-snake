using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using System.Linq;
using System;
using System.Drawing;
using Snake;
#nullable enable

public class SnakeSpline 
{
    // Points is used for storing the cubePoints of the snake before going into a tunnel (same for SplinePath)
    private List<CubePoint> Points;
    // TempPoints is used if the snake comes out of the tunnel. It is used as long there is at least one cubePoint left in Points (same for TempSplinePath)
    private List<CubePoint> TempPoints = new List<CubePoint>();

    public SplineContainer SplinePath { get; private set; }
    public SplineContainer TempSplinePath { get; private set; }

    private Tunnel? Tunnel;
    public int CurrentStepsInsideTunnel = 0;
    private int StepsInsideTunnel = 3;

    public DirectionOnCubeSide ReferenceDirectionForInput;

    public SnakeSpline(Transform snakeObject, Cube cube, CubeSideCoordinate startSide, DirectionOnCubeSide referenceDirectionForInput)
    {
        SplineContainer[] splineContainers = snakeObject.GetComponents<SplineContainer>();
        this.SplinePath = splineContainers[0];
        this.TempSplinePath = splineContainers[1];
        this.ReferenceDirectionForInput = referenceDirectionForInput;

        if (!this.SplinePath)
        {
            Debug.LogError("Couldnt get Splinepath. Snake GameObject needs a Spline component attached!");
            return;
        }
        
        if (!this.TempSplinePath)
        {
            Debug.LogError("Couldnt get TempSplinepath. Snake GameObject needs a second Spline component attached!");
            return;
        }
        
        
        this.Points = CreateStartPoints(cube, startSide);
    }

    private List<CubePoint> CreateStartPoints(Cube cube, CubeSideCoordinate startSide)
    {
        List<CubePoint> startPoints = new List<CubePoint>();

        // First Knot
        Vector3 positionInCube = startSide.GetPositionInCube(cube.Dimension, cube.Scale);
        Quaternion rotationInCube = startSide.GetRotationInCube();

        CubeFieldCoordinate startFieldCoordinate = startSide.GetDimension2D(cube.Dimension).GetFieldInLeftCenter();
        SplinePath.Spline.Add(AddKnotToPath(startFieldCoordinate, rotationInCube, positionInCube, cube.Scale, true));

        for (int i = 0; i < 3; i++)
        {
            CubeFieldCoordinate fieldCoordinate =
                new CubeFieldCoordinate(startFieldCoordinate.H + i, startFieldCoordinate.V);

            SplinePath.Spline.Add(AddKnotToPath(fieldCoordinate, rotationInCube, positionInCube, cube.Scale));

            startPoints.Add(new CubePoint(startSide, fieldCoordinate));
        }

        return startPoints;
    }

    private static BezierKnot AddKnotToPath(CubeFieldCoordinate fieldCoordinate, Quaternion rotationInCube, Vector3 positionInCube, float scaleFactor, bool isFirstKnot = false)
    {
        Vector3 positionInSide = fieldCoordinate.GetPositionInCubeSide(scaleFactor);

        Vector3 startPositionOfSnake = isFirstKnot switch
        {
            true => positionInCube + rotationInCube * (positionInSide - new Vector3(0.5f * scaleFactor, 0, 0)),
            false => positionInCube + rotationInCube * (positionInSide + new Vector3(0.5f * scaleFactor, 0, 0))
        };

        Vector3 rotation = new Vector3(0, 90, 90);

        BezierKnot startKnot = new BezierKnot();
        startKnot.Position = startPositionOfSnake;
        startKnot.Rotation = Quaternion.Euler(rotation);
        startKnot.TangentIn = new Vector3(0, 0, -0.33f);
        startKnot.TangentOut = new Vector3(0, 0, 0.33f);

        return startKnot;
    }

    public void VisualiseNextStepDirection(InputDirection stepInputDirection, Cube cube)
    {
        CubePoint snakeHead = GetSnakeHead();
        SplineContainer splinePathWithSnakeHead = GetSplineWithSnakeHead();
            
        splinePathWithSnakeHead.Spline.SetKnot(splinePathWithSnakeHead.Spline.Count - 1, CalculateSplineKnot(snakeHead, stepInputDirection, cube));
    }

    public void SetTunnel(Tunnel tunnel, CubePoint tunnelEntry)
    {
        Tunnel = tunnel;
    }

    /// <summary>
    /// Delete tail of splineArray and spawn new head, according to stepDirection
    /// </summary>
    public void UpdateSpline(DirectionOnCubeSide stepDirectionOnCubeSide, InputDirection stepInputDirection, CubePoint nextPoint, Cube cube, DirectionOnCubeSide referenceDirectionForInput, bool shouldGrow)
    {
        ReferenceDirectionForInput = referenceDirectionForInput;
        // Head is in Tunnel
        bool headIsInTunnel = Tunnel?.HeadGoesThroughTunnel ?? false;
        if (headIsInTunnel)
        {
            if (CurrentStepsInsideTunnel == 0)
            {
                // move over edge if the tunnelEntry is on the next cubeSide
                /*if (ShouldMoveOverEdge)
                {
                    RotationManager.Instance.RotateOneSide(stepInputDirection, nextPoint, cube.Dimension);
                    ReferenceDirectionForInput = TempReferenceDirectionForInput;
                }*/

                EnterTunnel(stepInputDirection, cube);
            }
            else if (CurrentStepsInsideTunnel == StepsInsideTunnel)
            {
                ExitTunnel(stepInputDirection, cube);
                
                Tunnel.HeadGoesThroughTunnel = false;
            }
            else
            {
                MoveInsideTunnel(stepInputDirection, cube);   
            }

            SplinePath.Spline.RemoveAt(0);
            Points.RemoveAt(0);
        }
        // Head is on the cube
        else
        {
            /*
            if (ShouldMoveOverEdge)
            {
                RotationManager.Instance.RotateOneSide(stepInputDirection, nextPoint, cube.Dimension);
                ReferenceDirectionForInput = TempReferenceDirectionForInput;
            }*/

            GetSplineWithSnakeHead().Spline.Add(CalculateSplineKnot(nextPoint, stepInputDirection, cube));
            GetPointsWithSnakeHead().Add(nextPoint);

            if (shouldGrow is false)
            {
                // After being in the tunnel it is possible, that the splinePath contains less elements than Points
                if (SplinePath.Spline.Count > 0)
                {
                    SplinePath.Spline.RemoveAt(0);
                }
                Points.RemoveAt(0);
            }

            // Reset Points and TempPoints, as the snake has now completely exited the tunnel
            if (Points.Count == 0)
            {
                Points = TempPoints;
                SplinePath.Spline = TempSplinePath.Spline;

                TempPoints = new List<CubePoint>();
                TempSplinePath.Spline = new Spline();
                
                Tunnel = null;
            }

            RotationManager.Instance.RotateEveryStep(stepInputDirection, GetSnakeHead(), cube.Dimension);
        }
    }

    private void EnterTunnel(InputDirection stepInputDirection, Cube cube)
    {
        SplinePath.Spline.Add(CalculateSplineKnot(Tunnel.Entry, stepInputDirection, cube));
        Points.Add(Tunnel.Entry);

        CurrentStepsInsideTunnel++;
    }

    private void MoveInsideTunnel(InputDirection stepInputDirection, Cube cube)
    {
        // needs to added to Points, so we do decrease the length of the snake (the length depends on the length of Points)
        Points.Add(Tunnel.Entry);

        if (CurrentStepsInsideTunnel == 1)
        {
            // add one more knot inside the tunnel to store gameObjects before changing their splineContainer to TempSplinePath
            SplinePath.Spline.Add(CalculateSplineKnot(Tunnel.Entry, stepInputDirection, cube));
        }

        CurrentStepsInsideTunnel++;
    }

    private void ExitTunnel(InputDirection stepInputDirection, Cube cube)
    {
        CubePoint tunnelExit = Tunnel.GetOtherCubePoint(Tunnel.Entry);
        // Knot is in the inside of the cube behind the tunnelExit
        TempSplinePath.Spline.Add(CalculateSplineKnot(tunnelExit, stepInputDirection, cube));
        TempPoints.Add(tunnelExit);

        RotationManager.Instance.RotateToOppositeSide(stepInputDirection);
        UpdateReferenceDirectionForInputOnOppositeSide(Tunnel.Entry, stepInputDirection, cube);
        
        Tunnel.HeadGoesThroughTunnel = false;
        // Knot is located on the outside of the cube at the tunnelExit (GoesThroughTunnel needs to be false here)
        TempSplinePath.Spline.Add(CalculateSplineKnot(tunnelExit, stepInputDirection, cube));

        CurrentStepsInsideTunnel = 0;
    }
    
    private void UpdateReferenceDirectionForInputOnOppositeSide(CubePoint point, InputDirection stepInputDirection, Cube cube)
    {
        DirectionOnCubeSide direction = stepInputDirection.ToLocalDirectionOnCubeSide(ReferenceDirectionForInput);

        for (int i = 0; i < 2; i++)
        {
            CubePoint nextPoint = point.GetPointOnNeighbour(direction, cube);

            (CubeSideCoordinate neighborCoordinate, DirectionOnCubeSide neighborDirection) nextSide =
                point.SideCoordinate.GetNeighborWithDirection(direction);

            ReferenceDirectionForInput = stepInputDirection.GetInputUpAsDirectionOnCubeSide(nextSide.neighborDirection);

            point = nextPoint;
            direction = nextSide.neighborDirection;
        }
    }

    private BezierKnot CalculateSplineKnot(CubePoint cubePoint, InputDirection stepInputDirection, Cube cube)
    {
        DirectionOnCubeSide stepDirectionOnCubeSide = stepInputDirection.ToLocalDirectionOnCubeSide(ReferenceDirectionForInput);

        BezierKnot bezierKnot = new BezierKnot();

        bezierKnot.Position = CalculateKnotPosition(cubePoint, stepDirectionOnCubeSide, cube);
        bezierKnot.Rotation = Tunnel is { HeadGoesThroughTunnel: true }
            ? CalculateKnotRotationInTunnel(cubePoint, stepDirectionOnCubeSide)
            : CalculateKnotRotation(cubePoint, stepDirectionOnCubeSide);
        bezierKnot.TangentIn = new Vector3(0, 0, -0.33f);
        bezierKnot.TangentOut = new Vector3(0, 0, 0.33f);

        return bezierKnot;
    }

    private Vector3 CalculateKnotPosition(CubePoint point, DirectionOnCubeSide stepDirectionOnCubeSide, Cube cube)
    {
        Vector3 positionInCube = point.SideCoordinate.GetPositionInCube(cube.Dimension, cube.Scale);
        Quaternion rotationInCube = point.SideCoordinate.GetRotationInCube();

        // position of the center of a field
        Vector3 positionInSide = point.FieldCoordinate.GetPositionInCubeSide(cube.Scale);

        // move the position to the edge of the field, to which the snake moves. Except it is a tunnelField, than we need the centered position.
        bool shouldMoveOnCube = !Tunnel?.HeadGoesThroughTunnel ?? true;
        if (shouldMoveOnCube)
        {
            positionInSide += (stepDirectionOnCubeSide switch
            {
                DirectionOnCubeSide.negHor => new Vector3(-1, 0, 0) * 0.5f * cube.Scale,
                DirectionOnCubeSide.posHor => new Vector3(1, 0, 0) * 0.5f * cube.Scale,
                DirectionOnCubeSide.negVert => new Vector3(0, 0, -1) * 0.5f * cube.Scale,
                DirectionOnCubeSide.posVert => new Vector3(0, 0, 1) * 0.5f * cube.Scale,
                _ => new Vector3(0, 0, 0)
            });
        }
        else
        {
            if (CurrentStepsInsideTunnel == 1)
            {
                // position of this knot is deep inside of the tunnel (it needs to be there so we can store the gameObjects before changing their splineContainer to TempSplinePath
                positionInSide += new Vector3(0, -2, 0) * 0.5f * cube.Scale;
            }
            else
            {
                positionInSide += new Vector3(0, -1, 0) * 0.5f * cube.Scale;
            }
        }


        return positionInCube + (rotationInCube * positionInSide);
    }

    private Quaternion CalculateKnotRotationInTunnel(CubePoint point, DirectionOnCubeSide stepDirectionOnCubeSide)
    {
        Quaternion sideRotation = point.SideCoordinate.GetRotationInCube();

        Vector3 rotationOnSide;

        // The rotation of the knot must point into the cube if the point is a TunnelEntry. If it is a TunnelExit, it must point away from the cube.
        bool isTunnelEntry = point.IsEqual(Tunnel.Entry!);

        // this switch is essential for the rotation of a knot on a cubeSide when the snake moves into the cube
        // I couldn't get the rotation around a specific point, so it needed this workaround
        switch (point.SideCoordinate)
        {
            case CubeSideCoordinate.Front:
            case CubeSideCoordinate.Right:
            case CubeSideCoordinate.Back:
            case CubeSideCoordinate.Left:
                rotationOnSide = stepDirectionOnCubeSide switch
                {
                    DirectionOnCubeSide.negHor => isTunnelEntry ? new Vector3(90, 0, -90) : new Vector3(90, -180, -90),
                    DirectionOnCubeSide.posHor => isTunnelEntry ? new Vector3(90, 0, 90) : new Vector3(90, 180, 90),
                    DirectionOnCubeSide.negVert => isTunnelEntry ? new Vector3(-270, 90, 90) : new Vector3(-90, 90, 90),
                    DirectionOnCubeSide.posVert => isTunnelEntry ? new Vector3(-90, 180, 0) : new Vector3(90, 180, 0),
                    _ => throw new ArgumentOutOfRangeException()
                };
                break;

            case CubeSideCoordinate.Up:
                rotationOnSide = stepDirectionOnCubeSide switch
                {
                    DirectionOnCubeSide.negHor => isTunnelEntry ? new Vector3(90, -90, 180) : new Vector3(-90, -90, 180),
                    DirectionOnCubeSide.posHor => isTunnelEntry ? new Vector3(90, 90, 180) : new Vector3(-90, 90, 180),
                    DirectionOnCubeSide.negVert => isTunnelEntry ? new Vector3(90, 0, 0) : new Vector3(-90, 0, 0),
                    DirectionOnCubeSide.posVert => isTunnelEntry ? new Vector3(90, 0, 180) : new Vector3(-90, 0, 180),
                    _ => throw new ArgumentOutOfRangeException()
                };
                break;

            case CubeSideCoordinate.Down:
                rotationOnSide = stepDirectionOnCubeSide switch
                {
                    DirectionOnCubeSide.negHor => isTunnelEntry ? new Vector3(-90, 90, 180) : new Vector3(90, 90, 180),
                    DirectionOnCubeSide.posHor => isTunnelEntry ? new Vector3(-90, -90, 180) : new Vector3(90, -90, 180),
                    DirectionOnCubeSide.negVert => isTunnelEntry ? new Vector3(-90, 0, 0) : new Vector3(-270, 0, 0),
                    DirectionOnCubeSide.posVert => isTunnelEntry ? new Vector3(-90, 0, 180) : new Vector3(90, 0, 180),
                    _ => throw new ArgumentOutOfRangeException()
                };
                break;

            default:
                rotationOnSide = Vector3.zero;
                break;
        }

        Vector3 rotation = sideRotation.eulerAngles + rotationOnSide;

        return Quaternion.Euler(rotation);
    }

    private Quaternion CalculateKnotRotation(CubePoint point, DirectionOnCubeSide stepDirectionOnCubeSide)
    {
        Quaternion sideRotation = point.SideCoordinate.GetRotationInCube();

        Vector3 rotationOnSide;

        // this switch is essential for the rotation of a knot on a cubeSide
        // I couldn't get the rotation around a specific point, so it needed this workaround
        switch (point.SideCoordinate)
        {
            case CubeSideCoordinate.Front:
            case CubeSideCoordinate.Right:
            case CubeSideCoordinate.Back:
            case CubeSideCoordinate.Left:
                rotationOnSide = stepDirectionOnCubeSide switch
                {
                    DirectionOnCubeSide.negHor => new Vector3(90, -90, -90),
                    DirectionOnCubeSide.posHor => new Vector3(90, 90, 90),
                    DirectionOnCubeSide.negVert => new Vector3(-180, 90, 90),
                    DirectionOnCubeSide.posVert => new Vector3(0, 180, 0),
                    _ => throw new ArgumentOutOfRangeException()
                };
                break;

            case CubeSideCoordinate.Up:
                rotationOnSide = stepDirectionOnCubeSide switch
                {
                    DirectionOnCubeSide.negHor => new Vector3(0, -90, 180),
                    DirectionOnCubeSide.posHor => new Vector3(0, 90, 180),
                    DirectionOnCubeSide.negVert => new Vector3(-180, 0, 0),
                    DirectionOnCubeSide.posVert => new Vector3(0, 0, 180),
                    _ => throw new ArgumentOutOfRangeException()
                };
                break;

            case CubeSideCoordinate.Down:
                rotationOnSide = stepDirectionOnCubeSide switch
                {
                    DirectionOnCubeSide.negHor => new Vector3(0, 90, 180),
                    DirectionOnCubeSide.posHor => new Vector3(0, -90, 180),
                    DirectionOnCubeSide.negVert => new Vector3(-180, 0, 0),
                    DirectionOnCubeSide.posVert => new Vector3(0, 0, 180),
                    _ => throw new ArgumentOutOfRangeException()
                };
                break;

            default:
                rotationOnSide = Vector3.zero;
                break;
        }

        Vector3 rotation = sideRotation.eulerAngles + rotationOnSide;

        return Quaternion.Euler(rotation);
    }

    public List<CubePoint> GetPointsWithSnakeHead()
    {
        return TempPoints.Count > 0 ? TempPoints : Points;
    }

    public SplineContainer GetSplineWithSnakeHead()
    {
        return TempPoints.Count > 0 ? TempSplinePath : SplinePath;
    }

    /// <summary>
    /// Returns the current CubePoint of the snakeHead. This already considers if the snake goes through the tunnel.
    /// </summary>
    public CubePoint GetSnakeHead()
    {
        return GetPointsWithSnakeHead().Last();
    }

    public List<CubePoint> GetAllPoints()
    {
        return Points.Concat(TempPoints).ToList();
    }

    public List<CubePoint> GetAllPointsWithoutSnakeHead()
    {
        List<CubePoint> allPoints = GetAllPoints();
        
        allPoints.RemoveAt(allPoints.Count - 1);

        return allPoints;
    }

    public bool TunnelContainsSnakeBodyPart()
    {
        return (TempPoints.Count > 0);
    }
}
