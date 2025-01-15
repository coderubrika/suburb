using System;
using System.Linq;
using Suburb.Utils;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Suburb.Common
{
    public class MenuMarsController
    {
        private readonly PrefabGroupsRepository prefabGroupsRepository;
        private readonly GameObject mars;
        private readonly GeneralPool<PrefabRef> asteroidsPool;
        private readonly OrbitalMover[] orbitalMovers = new OrbitalMover[ASTEROIDS_LENGTH];
        
        private const int ASTEROIDS_LENGTH = 15;
        
        private IDisposable updateDisposable;
        private bool isShown;
        private bool isPause;
        
        public MenuMarsController(PrefabGroupsRepository prefabGroupsRepository, Transform root, GameObject mars)
        {
            this.prefabGroupsRepository = prefabGroupsRepository;
            this.mars = mars;

            var prefabsGroup = this.prefabGroupsRepository.Items.FirstOrDefault(item => item.Name == "Asteroids");
            var prefabs = prefabsGroup.Prefabs;

            for (int i = 0; i < ASTEROIDS_LENGTH; i++)
            {
                orbitalMovers[i] = new OrbitalMover(
                    Random.onUnitSphere,
                    Random.Range(0.5f, 4f),
                    Random.Range(80, 100),
                    Random.Range(0.1f, 1f),
                    Random.rotationUniform
                );
            }
            
            asteroidsPool = new(
                item => item.gameObject.SetActive(true),
                item =>
                {
                    item.gameObject.SetActive(false);
                    item.transform.ResetLocal();
                },
                () =>
                {
                    var item = Object.Instantiate(prefabs[Random.Range(0, prefabs.Length)]);
                    item.transform.SetParent(root);
                    return item;
                });
        }

        public void Show()
        {
            if (isShown)
                return;
            isShown = true;
            
            for (int i = 0; i < ASTEROIDS_LENGTH; i++)
                orbitalMovers[i].Connect(mars.transform, asteroidsPool.Spawn());

            updateDisposable = Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    if (isPause)
                        return;
                    
                    mars.transform.localRotation *= Quaternion.Euler(Vector3.up * Time.deltaTime * 1);
                    foreach (var mover in orbitalMovers)
                        mover.Update();
                    
                });
        }

        public void Hide()
        {
            if (!isShown)
                return;
            isShown = false;
            
            for (int i = 0; i < ASTEROIDS_LENGTH; i++)
                asteroidsPool.Despawn(orbitalMovers[i].Disconnect());
            
            updateDisposable?.Dispose();
        }

        public void Pause()
        {
            isPause = true;
        }

        public void Resume()
        {
            isPause = false;
        }
    }
}