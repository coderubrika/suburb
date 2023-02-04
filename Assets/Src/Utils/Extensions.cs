using UnityEngine;
using System;

namespace Suburb.Utils
{
    public static class Extensions
    {
        public static bool IsClose(this Vector2 source, float closeDistance)
        {
            return Mathf.Abs(source.x) < closeDistance
                && Mathf.Abs(source.y) < closeDistance;
        }

        public static bool IsClose(this Vector3 source, float closeDistance)
        {
            return Mathf.Abs(source.x) < closeDistance
                && Mathf.Abs(source.y) < closeDistance
                && Mathf.Abs(source.z) < closeDistance;
        }

        public static bool IsCloseWithOther(this Vector2 source, Vector2 destination, float closeDistance)
        {
            return Mathf.Abs(source.x - destination.x) < closeDistance 
                && Mathf.Abs(source.y - destination.y) < closeDistance;
        }

        public static bool IsCloseWithOther(this Vector3 source, Vector3 destination, float closeDistance)
        {
            return Mathf.Abs(source.x - destination.x) < closeDistance
                && Mathf.Abs(source.y - destination.y) < closeDistance
                && Mathf.Abs(source.z - destination.z) < closeDistance;
        }

        public static void Log<T>(this T obj, string message, string filter="")
        {
            Debug.Log($"{filter}[{typeof(T).Name}] {message}");
        }

        public static void LogWarning<T>(this T obj, string message, string filter = "")
        {
            Debug.LogWarning($"{filter}[{typeof(T).Name}] {message}");
        }

        public static void LogError<T>(this T obj, string message, string filter = "")
        {
            Debug.LogError($"{filter}[{typeof(T).Name}] {message}");
        }
    }
}
