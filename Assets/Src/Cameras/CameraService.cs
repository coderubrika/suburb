using System.Collections.Generic;
using Suburb.Utils;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace Suburb.Cameras
{
    public class CameraService : IInitializable
    {
        private readonly InjectCreator injectCreator;
        private readonly Dictionary<string, Camera> overlayCameras = new();
        private readonly (string Name, Camera Camera)[] overlayCamerasPrefabs;
        private readonly Camera mainCameraPrefab;
        
        private Camera mainCamera;

        public Camera Main => mainCamera;

        public CameraService(
            InjectCreator injectCreator,
            Camera mainCameraPrefab, 
            (string Name, Camera Camera)[] overlayCamerasPrefabs)
        {
            this.injectCreator = injectCreator;
            this.mainCameraPrefab = mainCameraPrefab;
            this.overlayCamerasPrefabs = overlayCamerasPrefabs;
        }

        public Camera GetCamera(string cameraName)
        {
            overlayCameras.TryGetValue(cameraName, out Camera camera);
            return camera;
        }

        public void Initialize()
        {
            var camerasRoot = new GameObject("CamerasRoot").transform;
            GameObject.DontDestroyOnLoad(camerasRoot.gameObject);
            
            mainCamera = injectCreator.Create<Camera>(mainCameraPrefab.gameObject, camerasRoot);
            var mainCameraData = mainCamera.GetUniversalAdditionalCameraData();
            if (mainCameraData.renderType != CameraRenderType.Base)
            {
                this.LogError($"Camera: {mainCamera.name} must be RenderType: Base");
                return;
            }
            
            if (overlayCameras == null)
                return;

            foreach (var pair in overlayCamerasPrefabs)
            {
                Camera camera = pair.Camera;
                var cameraData = camera.GetUniversalAdditionalCameraData();
                if (cameraData.renderType == CameraRenderType.Base)
                {
                    this.LogError($"Camera: {camera.name} must be RenderType: Overlay");
                    continue;
                }
                
                Camera newCamera = injectCreator.Create<Camera>(camera.gameObject, camerasRoot);
                overlayCameras.Add(pair.Name, newCamera);
                mainCameraData.cameraStack.Add(newCamera);
            }
        }
    }
}