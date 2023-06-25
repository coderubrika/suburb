using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Suburb.Interactables
{
    public class InteractablesStore
    {
        private readonly Dictionary<GameObject, IInteractable> interactables = new();

        public bool CheckGameObject(GameObject candidate, out IInteractable interactable)
        {
            return interactables.TryGetValue(candidate, out interactable);
        }

        public void Login(GameObject gameObject, IInteractable interactable)
        {
            interactables[gameObject] = interactable;
        }

        public void Logout(GameObject candidate)
        {
            if (interactables.TryGetValue(candidate, out var interactable))
            {
                if (interactable != null)
                {
                    interactables[candidate] = null;
                }
            }
        }

        public void Reset()
        {
            interactables.Clear();
        }
    }
}