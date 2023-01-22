using Suburb.Common;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Suburb.Interactables
{
    public class Rover : MonoBehaviour, IInteractable, IInstallable
    {
        private InteractionRepository interactionRepository;

        [Inject]
        public void Construct(InteractionRepository interactionRepository)
        {
            this.interactionRepository = interactionRepository;
        }

        public void Interact()
        {
            Debug.Log("Rover interact");
        }

        public void Install()
        {
            interactionRepository.Login(gameObject, this);
        }

        public void Uninstall()
        {
            interactionRepository.Logout(gameObject);
        }
    }
}