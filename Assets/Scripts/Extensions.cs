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
    }
}

