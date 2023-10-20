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

        /* private SplineAnimate headAnimate; */
        private List<SplineAnimate> bodyAnimators;
        /* private SplineAnimate tailAnimate; */

        private List<GameObject> bodyParts;

        private void Start()
        {
            stepDirection = inputDirection;
            
            InvokeRepeating(nameof(DetermineNextStepDirection), stepInterval * 0.75f, stepInterval);
            InvokeRepeating(nameof(UpdateSpline), stepInterval, stepInterval);

            // List Initializations
            bodyAnimators = new List<SplineAnimate>();
            bodyParts = new List<GameObject>();

            GetSnakeAnimators();
            Debug.Log("Länge Spline: " + splinePath.Spline.GetLength());
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

            // animate Head
            /* headAnimate.NormalizedTime = (splinePath.Spline.GetLength() -1) / splinePath.Spline.GetLength();
            headAnimate.Play(); */

            // animate Body Parts
            for (int i = 0; i<bodyAnimators.Count; i++)
            {
                bodyAnimators[i].NormalizedTime = i * (1.0f/bodyAnimators.Count);
                bodyAnimators[i].Play();
            }
        }

        private void GetSnakeAnimators()
        {
            // Iterate Spline and Configure Animators for Each Body Part
            Debug.Log("Obergrenze: " + ((splinePath.Spline.GetLength()-2)*2));
            for (int i = 1; i<(splinePath.Spline.GetLength()-2)*2; i++ ) 
            {
                Debug.Log("Spline kleiner counter: " + i);
                if (i > (splinePath.Spline.GetLength()-3)*2)
                {
                    // Empty
                    Debug.Log("Empty");
                    Debug.Log((splinePath.Spline.GetLength() *2)-2);
                }
                else if (i == 1)
                {
                    // Tail
                    bodyParts.Add(Instantiate(snakeTail));
                    Debug.Log("Tail");
                }
                else if (i == (splinePath.Spline.GetLength()-3)*2)
                {
                    // Head
                    bodyParts.Add(Instantiate(snakeHead));
                    Debug.Log("Head");
                }
                else {
                    // Body
                    bodyParts.Add(Instantiate(snakeBody));
                    Debug.Log("Body");
                }

                SplineAnimate animate = bodyParts[i-1].GetComponent<SplineAnimate>();
                animate.Container = splinePath;
                animate.Loop = SplineAnimate.LoopMode.Once;
                animate.AnimationMethod = SplineAnimate.Method.Speed;
                animate.MaxSpeed = stepLength / stepInterval;
                bodyAnimators.Add(animate);
            }
        }
    }
}
