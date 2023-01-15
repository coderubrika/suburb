using System.Collections;
using UnityEngine;

namespace Suburb.Common
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private Camera plauerCamera;

        public Camera GetCamera()
        {
            return plauerCamera;
        }
    }
}