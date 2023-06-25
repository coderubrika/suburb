using Suburb.Common;
using Suburb.Utils;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Suburb.Interactables
{
    public class Land : MonoBehaviour, IInteractable
    {
        public Vector3 ContactPosition { get; private set; }

        public void Interact(BaseInteractEventData data)
        {
            ContactPosition = data.Ray.GetPoint(data.Distance);
        }
    }
}