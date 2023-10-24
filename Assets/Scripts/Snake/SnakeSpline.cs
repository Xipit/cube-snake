using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

namespace Snake
{
    public class SnakeSpline : MonoBehaviour
    {
        public SplineContainer splinePath;

        public float stepLength;
        public float stepInterval;
    
        private MovementDirection stepDirection;
        public MovementDirection inputDirection;

        public GameObject snakeHead;
        public GameObject snakeBody;
        public GameObject snakeTail;
        public GameObject emptyObject;
        private List<GameObject> bodyParts;

        private void Start()
        {
            stepDirection = inputDirection;
            
            InvokeRepeating(nameof(DetermineNextStepDirection), stepInterval * 0.75f, stepInterval);
            InvokeRepeating(nameof(UpdateSpline), stepInterval, stepInterval);

            // List Initializations
            bodyParts = new List<GameObject>();

            BuildSnakeBody();
            UpdateSnakeBody();

            Time.timeScale = 0.5F;
        }
    
        private void Update()
        {
            inputDirection = InputManager.Instance.GetPlayerInput(stepDirection) ?? inputDirection;
        }

        /// <summary>
        /// Set position of next spline point, according to current inputDirection.
        /// </summary>
        private void DetermineNextStepDirection()
        {
            BezierKnot lastKnot = splinePath.Spline.ToArray().Last();

            if (stepDirection != inputDirection)
            {
                Vector3 directionVector = (inputDirection.GetDirectionVector(stepLength) -
                                           stepDirection.GetDirectionVector(stepLength)) / 2;
                lastKnot.Position += (float3)directionVector;
                
                lastKnot.Rotation = inputDirection.GetRotation();
            }
        
            splinePath.Spline.SetKnot(splinePath.Spline.Count - 1, lastKnot);
            stepDirection = inputDirection;
        }
    
        /// <summary>
        /// Delete tail of splineArray and spawn new head, according to stepDirection
        /// </summary>
        private void UpdateSpline()
        {
            // add a new knot to the spline
            // head == end of splineArray
            // tail == start of splineArray
            
            BezierKnot lastKnot = splinePath.Spline.ToArray().Last();
            BezierKnot newKnot = new BezierKnot();

            newKnot.Position = lastKnot.Position + (float3)stepDirection.GetDirectionVector(stepLength);
            newKnot.Rotation = stepDirection.GetRotation();
            newKnot.TangentIn = new float3(0, 0, -0.33f);
            newKnot.TangentOut = new float3(0, 0, 0.33f);
 
            splinePath.Spline.Add(newKnot);
            splinePath.Spline.RemoveAt(0);

            UpdateSnakeBody();
        }

        private void UpdateSnakeBody(){
            // set each bodypart to a specific percantage of the spline when updating the spline
            for (int i = 0; i<bodyParts.Count; i++)
            {
                bodyParts[i].GetComponent<SplineAnimate>().NormalizedTime = i * (1.0f/bodyParts.Count);
                bodyParts[i].GetComponent<SplineAnimate>().Play();
            }
        }

        /// <summary>
        /// Fill the List of bodyParts (instantiated GameObjects) with each part of the snake
        /// </summary>
        private void BuildSnakeBody()
        {
            // Tail
            bodyParts.Add(Instantiate(snakeTail));
            ConfigureBodyAnimator(0);

            // Body (iterate doubled index for more density in the body)
            for (int i = 1; i<(splinePath.Spline.GetLength()*2)-3; i++) 
            {
                bodyParts.Add(Instantiate(snakeBody));
                ConfigureBodyAnimator(i);
                Debug.Log(i + " Body");
            }

            // Head
            bodyParts.Add(Instantiate(snakeHead));
            ConfigureBodyAnimator((int) (splinePath.Spline.GetLength() * 2) - 3);

            // Empty
            bodyParts.Add(Instantiate(emptyObject));
            ConfigureBodyAnimator((int) (splinePath.Spline.GetLength() * 2) - 2);
        }

        /// <summary>
        /// Ensure to set initial options like container and speed for the animator of each bodypart
        /// </summary>
        private void ConfigureBodyAnimator(int index)
        {
            SplineAnimate animate = bodyParts[index].GetComponent<SplineAnimate>();
            animate.Container = splinePath;
            animate.Loop = SplineAnimate.LoopMode.Once;
            animate.AnimationMethod = SplineAnimate.Method.Speed;
            animate.MaxSpeed = stepLength / stepInterval;
        }
    }
}
