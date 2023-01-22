using System.Collections;
using UnityEngine;

namespace Suburb.Common
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;

        public Camera GetCamera()
        {
            return playerCamera;
        }
    }
}