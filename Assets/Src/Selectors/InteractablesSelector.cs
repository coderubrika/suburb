using Suburb.Common;
using Suburb.Core;
using UnityEngine;

namespace Suburb.Selectors
{
    public class InteractablesSelector
    {
        private readonly PlayerCamera playerCamera;
        private readonly PointerService pointerService;

        public InteractablesSelector(PlayerCamera playerCamera, PointerService pointerService)
        {
            this.playerCamera = playerCamera;
            this.pointerService = pointerService;
        }
    }
}
