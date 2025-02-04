using System;
using System.Collections;
using System.Collections.Generic;
using Suburb.Inputs;
using Suburb.Utils;
using UniRx;
using UnityEngine;

namespace ExitTheBoard
{
    public class Startup : MonoBehaviour
    {
        private LayerOrderer layerOrderer;
        private MouseProvider mouseProvider;
        private MouseResourceDistributor mouseResourceDistributor;
        private RectBasedSession session;
        
        [SerializeField] private RectTransform frame;

        private readonly CompositeDisposable disposables = new();
        
        private void Awake()
        { 
            layerOrderer = new();
            mouseProvider = new();
            mouseResourceDistributor = new(mouseProvider);
            session = new(frame);
        }

        private void OnEnable()
        {
            MouseSwipeCompositor compositor = new(mouseProvider, mouseResourceDistributor, MouseButtonType.Left);
            session.AddCompositor(compositor)
                .AddTo(disposables);
            layerOrderer.ConnectFirst(session)
                .AddTo(disposables);

            SwipeMember swipe = session.GetMember<SwipeMember>();
            swipe.OnDown
                .Subscribe(pos => this.Log($"Down: {pos}"))
                .AddTo(disposables);
            
            swipe.OnDrag
                .Subscribe(delta => this.Log($"Drag: {delta}"))
                .AddTo(disposables);
            
            swipe.OnUp
                .Subscribe(pos => this.Log($"Up: {pos}"))
                .AddTo(disposables);
        }

        private void OnDisable()
        {
            disposables.Clear();
        }
    }
}
