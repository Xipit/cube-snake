using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Extensions to existing classes
// https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods
namespace Extensions
{
    public static class Extensions 
    {
        public static Vector3 CopyWith(this Vector3 vector, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);
        }

        /// <summary>
        /// takes a rotation and returns the rotation that is the closest with all axes pointing at 90 degree intervals to the identity quaternion
        /// <br/> source: https://gamedev.stackexchange.com/a/183342
        /// </summary>
        public static Quaternion SnapToNearestRightAngle(this Quaternion currentRotation)
        {
            Vector3 closestToForward = (Vector3)(currentRotation * Vector3.forward).SnappedToNearestAxis();
            Vector3 closestToUp = SnappedToNearestAxis(currentRotation * Vector3.up);
            return Quaternion.LookRotation(closestToForward, closestToUp);
        }

        /// <summary>
        /// find the world axis that is closest to direction
        /// <br/> source: https://gamedev.stackexchange.com/a/183342
        /// </summary>
        public static Vector3 SnappedToNearestAxis(this Vector3 direction)
        {
            float x = Mathf.Abs(direction.x);
            float y = Mathf.Abs(direction.y);
            float z = Mathf.Abs(direction.z);
            if (x > y && x > z)
            {
                return new Vector3(Mathf.Sign(direction.x), 0, 0);
            }
            else if (y > x && y > z)
            {
                return new Vector3(0, Mathf.Sign(direction.y), 0);
            }
            else
            {
                return new Vector3(0, 0, Mathf.Sign(direction.z));
            }
        }
    }
}

