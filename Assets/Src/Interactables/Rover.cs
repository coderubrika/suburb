using Suburb.Common;
using System;
using System.Collections;
using UniRx;
using UnityEngine;
using Zenject;

namespace Suburb.Interactables
{
    public class Rover : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private float rotateSpeed;

        private readonly CompositeDisposable disposables = new();

        private IDisposable moveDisposable;

        public void Pick()
        {
            moveDisposable?.Dispose();
        }

        public void Hide()
        {
            disposables.Clear();
        }

        public void Move(Vector3 position)
        {
            moveDisposable = Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    if (transform.position != position)
                    {
                        Vector3 direction = position - transform.position;
                        transform.position = Vector3.MoveTowards(transform.position, position, moveSpeed * Time.deltaTime);
                        
                        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, moveSpeed * Time.deltaTime);
                    }
                    else
                    {
                        moveDisposable?.Dispose();
                    }
                })
                .AddTo(disposables);
        }
    }
}