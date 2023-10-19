using System;
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
        private SplineAnimate splineAnimate;

        private void Start()
        {
            stepDirection = inputDirection;
            
            InvokeRepeating(nameof(DetermineNextStepDirection), stepInterval * 0.75f, stepInterval);
            InvokeRepeating(nameof(UpdateSpline), stepInterval, stepInterval);

            CreateSnakeBody();
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
            splineAnimate.NormalizedTime = (splinePath.Spline.GetLength() -1) / splinePath.Spline.GetLength();
            splineAnimate.Play();
        }

        private void CreateSnakeBody()
        {
            // Z & Y-axis shift for center alignment
            // TODO must be changed when moving over edges
            splineAnimate = snakeHead.GetComponent<SplineAnimate>();
            splineAnimate.Container = splinePath;
            splineAnimate.Loop = SplineAnimate.LoopMode.Once;
            splineAnimate.AnimationMethod = SplineAnimate.Method.Speed;
            splineAnimate.MaxSpeed = stepLength / stepInterval;
        }
    }
}
