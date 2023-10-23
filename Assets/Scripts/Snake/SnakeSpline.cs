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
        private List<SplineAnimate> bodyAnimators;
        private List<GameObject> bodyParts;

        private void Start()
        {
            stepDirection = inputDirection;
            
            InvokeRepeating(nameof(DetermineNextStepDirection), stepInterval * 0.75f, stepInterval);
            InvokeRepeating(nameof(UpdateSpline), stepInterval, stepInterval);

            // List Initializations
            bodyAnimators = new List<SplineAnimate>();
            bodyParts = new List<GameObject>();

            BuildSnakeAnimators();
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

            // set each bodypart to a specific percantage of the spline when updating the spline
            for (int i = 0; i<bodyAnimators.Count; i++)
            {
                bodyAnimators[i].NormalizedTime = i * (1.0f/bodyAnimators.Count);
                bodyAnimators[i].Play();
            }
        }

        // Fill bodyParts (instantiated GameObjects) and bodyAnimators (SplineAnimate) for each part of the snake
        private void BuildSnakeAnimators()
        {
            // Tail
            bodyParts.Add(Instantiate(snakeTail));
            ConfigureBodyAnimator(bodyParts[0]);

            // Body (iterate doubled index for more density in the body)
            for (int i = 1; i<(splinePath.Spline.GetLength()*2)-3; i++) 
            {
                bodyParts.Add(Instantiate(snakeBody));
                ConfigureBodyAnimator(bodyParts[i]);
                Debug.Log(i + " Body");
            }

            // Head
            bodyParts.Add(Instantiate(snakeHead));
            ConfigureBodyAnimator(bodyParts[(int) (splinePath.Spline.GetLength()*2)-3]);

            // Empty
            bodyParts.Add(Instantiate(emptyObject));
            ConfigureBodyAnimator(bodyParts[(int) (splinePath.Spline.GetLength()*2)-2]);
        }

        // Set initial options like container and speed for the animator of each
        private void ConfigureBodyAnimator(GameObject bodyPart)
        {
            SplineAnimate animate = bodyPart.GetComponent<SplineAnimate>();
            animate.Container = splinePath;
            animate.Loop = SplineAnimate.LoopMode.Once;
            animate.AnimationMethod = SplineAnimate.Method.Speed;
            animate.MaxSpeed = stepLength / stepInterval;
            bodyAnimators.Add(animate);
        }
    }
}
