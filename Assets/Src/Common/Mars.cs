using System;
using System.Linq;
using Suburb.ResourceMaps;
using Suburb.Utils;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Suburb.Common
{
    public class Mars : MonoBehaviour
    {
        //  не хочу чтобы так это делалось хочу чтобы был тупо префаб если потребуется хранить компонент
        // то только тот что отвечает за ссылки на другие компоненты поэтому таких вещей как моно на этих
        // штуках висеть не будет
        
        [SerializeField] private float mass;
        [SerializeField] private Vector3 minVelocity;
        [SerializeField] private Vector3 maxVelocity;
        [SerializeField] private float rotationSpeed;
        
        private Asteroid[] asteroids = new Asteroid[50];
        
        [Inject]
        public void Construct(ResourcesService resourcesService)
        {
            var prefabs = resourcesService.GetPrefabsGroup("Asteroids")
                .Select(prefab => prefab.GetComponent<Asteroid>())
                .ToArray();
            
            AssetsGroupPool<Asteroid> poolGroup = resourcesService.GetPoolGroup("Asteroids", prefabs);
            
            for (int i = 0; i < asteroids.Length; i++)
            {
                var asteroid = asteroids[i] = poolGroup.Spawn(0);
                asteroid.gameObject.SetActive(false);
                asteroid.transform.localScale = Vector3.one * Random.Range(0.01f, 1f);
                asteroid.transform.position = transform.position + Random.insideUnitSphere * (transform.localScale.x + 10);
                asteroid.Velocity = MathUtils.RandVector3(minVelocity, maxVelocity);
            }
        }

        private void Update()
        {
            transform.localRotation *= Quaternion.Euler(Vector3.forward * (rotationSpeed * Time.deltaTime));
            foreach (var asteroid in asteroids)
                CalcAsteroidTransform(asteroid);
        }

        private void OnEnable()
        {
            asteroids.SetActiveGameObjects(true);
        }

        private void OnDisable()
        {
            asteroids.SetActiveGameObjects(false);
        }

        private void CalcAsteroidTransform(Asteroid asteroid)
        {
            var position = asteroid.transform.position;
            Vector3 directionToPlanet = transform.position - position;
            float distanceSquared = directionToPlanet.sqrMagnitude;

            float distance = directionToPlanet.magnitude;
            
            if (distance < 48 || distance > 500)
            {
                asteroid.transform.localScale = Vector3.one * Random.Range(0.01f, 1f);
                asteroid.transform.position = transform.position + Random.insideUnitSphere * (transform.localScale.x + 10);
                asteroid.Velocity = MathUtils.RandVector3(minVelocity, maxVelocity);
                
                position = asteroid.transform.position;
                directionToPlanet = transform.position - position;
                distanceSquared = directionToPlanet.sqrMagnitude;

                distance = directionToPlanet.magnitude;
            }
            Vector3 directionToPlanetNorm = directionToPlanet / distance;

            Vector3 gravitationForce =
                directionToPlanetNorm * (MathUtils.GRAVITATION_CONST * mass * asteroid.Mass) / distanceSquared;
            Vector3 acceleration = gravitationForce / asteroid.Mass;
            asteroid.Velocity += acceleration;
            position += asteroid.Velocity * Time.deltaTime;
            asteroid.transform.position = position;
        }
    }
}