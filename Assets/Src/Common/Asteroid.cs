using System;
using System.Collections.Generic;
using UnityEngine;

namespace Suburb.Common
{
    public class Asteroid : MonoBehaviour, IPoolItem
    {
        [SerializeField] private float mass;

        private Vector3 velocity;
        
        public float Mass => mass;
        public Vector3 Velocity { get; set; }
        public Dictionary<string, object> Parameters { get; } = new();
        public void Spawn()
        {
        }

        public void Despawn()
        {
        }
    }
}