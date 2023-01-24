using Suburb.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Suburb.Interactables
{
    public abstract class InteractableObject : MonoBehaviour, IInteractable, IInstallable
    {
        private InteractionRepository interactionRepository;

        [Inject]
        public void Construct(InteractionRepository interactionRepository)
        {
            this.interactionRepository = interactionRepository;
        }

        public abstract void Interact(BaseInteractEventData baseInteractEventData);
        
        public virtual void Install()
        {
            interactionRepository.Login(gameObject, this);
        }

        public virtual void Uninstall()
        {
            interactionRepository.Logout(gameObject);
        }
    }
}
