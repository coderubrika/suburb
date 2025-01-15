using System;
using Suburb.Inputs;
using Suburb.Utils;
using UniRx;
using UnityEngine;
using Zenject;

namespace TestAssets.Src
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float maxSpeed;
        
        private Vector3 direction;
        private bool isMoving;
        
        public void StartMoving()
        {
            isMoving = true;
        }

        public void StopMoving()
        {
            isMoving = false;
        }

        public void PutDirection(Vector3 direction)
        {
            this.direction = direction;
        }

        private void Update()
        {
            if (!isMoving)
                return;

            transform.position += direction * maxSpeed * Time.deltaTime;
        }
    }
}