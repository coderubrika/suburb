using System;
using UnityEngine;

namespace Suburb.Utils
{
    public static class MathUtils
    {
        public static Vector3 FindDestination(Vector3 sourcePosition, Vector3 forward, float distance)
        {
            return sourcePosition + forward * distance;
        }
    }
}
