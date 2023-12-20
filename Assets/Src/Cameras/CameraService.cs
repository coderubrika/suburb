using Suburb.Utils;
using UnityEngine;
using Zenject;

namespace Suburb.Cameras
{
    public class CameraService : IInitializable
    {
        private readonly InjectCreator injectCreator;
        public CameraService(
            InjectCreator injectCreator)
        {
            this.injectCreator = injectCreator;
        }

        public void Initialize()
        {
            var camerasRoot = new GameObject("CamerasRoot").transform;
            GameObject.DontDestroyOnLoad(camerasRoot.gameObject);
        }
    }
}