using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;


public class SnakeBody 
{
    private Cube Cube;
    private float StepInterval;

    private Transform SnakeObject;
    public GameObject SnakeHeadPrefab;
    public GameObject SnakeBodyPrefab;
    public GameObject SnakeTailPrefab;
    public GameObject EmptyPrefab;

    private List<GameObject> BodyParts = new List<GameObject>();

    public SnakeBody(Cube cube, float stepInterval, SplineContainer splinePath, Transform snakeObject, GameObject snakeHeadPrefab, GameObject snakeBodyPrefab, GameObject snakeTailPrefab, GameObject emptyPrefab)
    {
        Cube = cube;
        StepInterval = stepInterval;

        SnakeObject = snakeObject;
        SnakeHeadPrefab = snakeHeadPrefab;
        SnakeBodyPrefab = snakeBodyPrefab;
        SnakeTailPrefab = snakeTailPrefab;
        EmptyPrefab = emptyPrefab;

        BodyParts = BuildSnakeBody(splinePath);
    }

    public void UpdateSnakeBody(SplineContainer splinePath, SplineContainer tempSplinePath)
    {
        // Points & SplinePath --> Points and Knots before going into the Tunnel
        // TempPoints & TempSplinePath --> Points and Knots after coming out of the TunnelExit

        if (HeadGoesThroughTunnel && CurrentStepsInsideTunnel > 1)
        {
            int movingBodyPartsCount = BodyParts.Count - (CurrentStepsInsideTunnel - 2);

            // stop the gameObjects which are moved into the cube through the tunnelEntry
            for (int i = (BodyParts.Count - 1); i >= movingBodyPartsCount; i--)
            {
                SplineAnimate bodyPartAnimate = BodyParts[i].GetComponent<SplineAnimate>();
                bodyPartAnimate.Container = splinePath;
                bodyPartAnimate.StartOffset = 1f;
                bodyPartAnimate.NormalizedTime = 0f;
                bodyPartAnimate.Pause();
            }

            // keep the gameObject animates which are still moving towards the tunnelEntry
            for (int i = 0; i < movingBodyPartsCount; i++)
            {
                SplineAnimate bodyPartAnimate = BodyParts[i].GetComponent<SplineAnimate>();
                bodyPartAnimate.Container = splinePath;
                bodyPartAnimate.StartOffset = 0;
                bodyPartAnimate.NormalizedTime = i * (1.0f / movingBodyPartsCount);
                bodyPartAnimate.Play();
            }

        }
        else if (TempPoints.Count > 0)
        {
            // a part of the snake came through the tunnelExit and is now moving on the cube again

            int containerChangedCounter = 0;
            int bodyPartsOnTempSplinePath = 0;

            for (int i = BodyParts.Count - 1; i >= 0; i--)
            {
                SplineAnimate bodyPartAnimate = BodyParts[i].GetComponent<SplineAnimate>();

                // move last part of the Snake to TempSplinePath
                if (bodyPartAnimate.Container == splinePath && containerChangedCounter < 2)
                {
                    bodyPartAnimate.Container = tempSplinePath;
                    containerChangedCounter++;
                }

                // count how many BodyParts are placed on the TempSplinePath
                if (bodyPartAnimate.Container == tempSplinePath)
                {
                    bodyPartsOnTempSplinePath++;
                }
            }

            // animate the gameObjects on both splinePaths
            int k = 0;
            for (int i = 0; i < BodyParts.Count; i++)
            {
                SplineAnimate bodyPartAnimate = BodyParts[i].GetComponent<SplineAnimate>();
                bodyPartAnimate.StartOffset = 0;

                if (bodyPartAnimate.Container == splinePath)
                {
                    bodyPartAnimate.NormalizedTime = i * (1.0f / (BodyParts.Count - bodyPartsOnTempSplinePath));
                }
                else
                {
                    bodyPartAnimate.NormalizedTime = k * (1.0f / bodyPartsOnTempSplinePath);
                    k++;
                }

                bodyPartAnimate.Play();
            }
        }
        else
        {
            // set each bodypart to a specific percantage of the spline when updating the spline
            for (int i = 0; i < BodyParts.Count; i++)
            {
                SplineAnimate bodyPartAnimate = BodyParts[i].GetComponent<SplineAnimate>();
                bodyPartAnimate.Container = splinePath;
                bodyPartAnimate.StartOffset = 0;
                bodyPartAnimate.NormalizedTime = i * (1.0f / BodyParts.Count);
                bodyPartAnimate.Play();
            }
        }
    }

    public void UpdateSnakeBodyAfterSnack()
    {
        // set each bodypart to a specific percantage of the spline 
        //  - Tail and old BodyParts pause the animation
        //  - new BodyParts, Head, and EmptyGameObject move further

        // Tail and old BodyParts
        for (int i = 0; i < BodyParts.Count - 4; i++)
        {
            SplineAnimate bodyPartAnimate = BodyParts[i].GetComponent<SplineAnimate>();
            bodyPartAnimate.StartOffset = i * (1.0f / (BodyParts.Count - 2));
            bodyPartAnimate.Pause();
        }

        // new BodyParts
        for (int i = BodyParts.Count - 4; i < BodyParts.Count - 2; i++)
        {
            SplineAnimate bodyPartAnimate = BodyParts[i].GetComponent<SplineAnimate>();
            bodyPartAnimate.StartOffset = (i - 2) * (1.0f / (BodyParts.Count - 2));
            bodyPartAnimate.NormalizedTime = (i - 2) * (1.0f / (BodyParts.Count - 2));
            bodyPartAnimate.Play();
        }

        // Head and EmptyGameObject
        for (int i = BodyParts.Count - 2; i < BodyParts.Count; i++)
        {
            SplineAnimate bodyPartAnimate = BodyParts[i].GetComponent<SplineAnimate>();
            bodyPartAnimate.NormalizedTime = i * (1.0f / BodyParts.Count);
            bodyPartAnimate.Play();
        }
    }

    private void PauseSnake(List<GameObject> bodyParts)
    {
        for (int i = 0; i < bodyParts.Count - 1; i++)
        {
            SplineAnimate animate = bodyParts[i].GetComponent<SplineAnimate>();

            animate.StartOffset = i * (1.0f / bodyParts.Count);
            animate.MaxSpeed = 0; // Pause
        }
    }

    /// <summary>
    /// Fill the List of BodyParts (instantiated GameObjects) with each part of the snake
    /// </summary>
    private List<GameObject> BuildSnakeBody(SplineContainer splinePath)
    {
        List<GameObject> bodyParts = new List<GameObject>();

        // Tail
        bodyParts.Add(
            InstantiateManager.Instance.InstantiateGameObjectAsChild(SnakeTailPrefab, SnakeObject));

        // Body (iterate doubled index for more density in the body)
        for (int i = 1; i < (splinePath.Spline.GetLength() * 2) - 3; i++)
        {
            bodyParts.Add(
            InstantiateManager.Instance.InstantiateGameObjectAsChild(SnakeBodyPrefab, SnakeObject));
        }

        // Head
        bodyParts.Add(
            InstantiateManager.Instance.InstantiateGameObjectAsChild(SnakeHeadPrefab, SnakeObject));

        // Empty
        bodyParts.Add(
            InstantiateManager.Instance.InstantiateGameObjectAsChild(EmptyPrefab, SnakeObject));

        for (int i = 0; i < bodyParts.Count; i++)
        {
            ConfigureBodyAnimator(i, splinePath);
        }

        return bodyParts;
    }

    /// <summary>
    /// Add a new BodyPart to the snake. (At the moment: adds 2 BodyParts for more density)
    /// </summary>
    public void AddSnakeBodyPart(SplineContainer splinePath)
    {
        int index = BodyParts.Count - 2;

        for (int i = 0; i < 2; i++)
        {
            BodyParts.Insert(index,
                InstantiateManager.Instance.InstantiateGameObjectAsChild(SnakeBodyPrefab, SnakeObject));

            ConfigureBodyAnimator(index, splinePath);
        }
    }

    /// <summary>
    /// Ensure to set initial options like container and speed for the animator of each bodypart
    /// </summary>
    private void ConfigureBodyAnimator(int index, SplineContainer splinePath)
    {
        SplineAnimate animate = BodyParts[index].GetComponent<SplineAnimate>();
        animate.Container = splinePath;
        animate.Loop = SplineAnimate.LoopMode.Once;
        animate.AnimationMethod = SplineAnimate.Method.Speed;
        animate.MaxSpeed = Cube.Scale / StepInterval;

        animate.StartOffset = index * (1.0f / BodyParts.Count);
    }

}
