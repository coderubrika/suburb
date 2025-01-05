using System;
using DG.Tweening;
using Suburb.Inputs;
using Suburb.Screens;
using Suburb.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FFA.UI.Screens
{
    public class BattleScreen : BaseScreen
    {
        private PlayerView.Pool playersPool;
        private InjectCreator injectCreator;
        private LayerOrderer layerOrderer;
        
        [SerializeField] private StartFightTimer startFightTimer;
        [SerializeField] private Image fieldFader;
        [SerializeField] private BattleZoneChoicePreparationView zoneDown;
        [SerializeField] private BattleZoneChoicePreparationView zoneTop;
        [SerializeField] private RectTransform battleZone;
        
        private readonly CompositeDisposable disposables = new();

        private float fieldFaderAlpha;
        
        [Inject]
        private void Construct(
            PlayerView.Pool playersPool,
            InjectCreator injectCreator,
            LayerOrderer layerOrderer)
        {
            this.playersPool = playersPool;
            this.injectCreator = injectCreator;
            this.layerOrderer = layerOrderer;
            
            fieldFaderAlpha = fieldFader.color.a;
        }
        
        protected override void Show()
        {
            base.Show();
            fieldFader.gameObject.SetActive(true);
            fieldFader.color = UIUtils.GetNewAlpha(fieldFader.color, fieldFaderAlpha);
            zoneDown.gameObject.SetActive(true);
            zoneDown.Init(1);
            
            // zoneTop.gameObject.SetActive(true);
            // zoneTop.Init(2);
            
            startFightTimer.gameObject.SetActive(true);
            startFightTimer.ResetTimer();
            
            // Observable.WhenAll(zoneDown.OnResponse.Take(1), zoneTop.OnResponse.Take(1))
            //     .ObserveOnMainThread()
            //     .ContinueWith(_ => startFightTimer.StartTimer(TimeSpan.FromSeconds(0.5f)))
            //     .ObserveOnMainThread()
            //     .Do(_ => startFightTimer.gameObject.SetActive(false))
            //     .ObserveOnMainThread()
            //     .ContinueWith(_ => Observable.WhenAll(
            //         zoneDown.Show("Player2"), 
            //         zoneTop.Show("Player1")))
            //     .ContinueWith(_ => fieldFader.DOFade(0, 0.4f).ToObservableOnComplete())
            //     .Subscribe()
            //     .AddTo(disposables);
            
            zoneDown.OnResponse
                .Subscribe(SetupPlayer)
                .AddTo(disposables);
            
            zoneTop.OnResponse
                .Subscribe(SetupPlayer)
                .AddTo(disposables);
        }

        protected override void Hide()
        {
            disposables.Clear();
            zoneDown.Hide();
            zoneTop.Hide();
            startFightTimer.ResetTimer();
            DOTween.Kill(fieldFader);
            startFightTimer.gameObject.SetActive(false);
        }

        private void SetupPlayer((Vector2 Position, ISession session) data)
        {
            this.Log($"SetupPlayer: {data.Position}");
            var player = playersPool.Spawn();
            player.transform.SetParent(battleZone);
            player.transform.localScale = Vector3.one;
            player.transform.position = data.Position;
            var playerSession = new RectBasedSession(player.transform as RectTransform);
            playerSession.SetBookResources(true);
            playerSession.SetPreventNext(true);
                    
            var touchCompositor = injectCreator.Create<OneTouchPluginCompositor>();
            playerSession.AddCompositor(touchCompositor)
                .AddTo(disposables);
            // var swipeMouseCompositor = injectCreator.Create<MouseSwipeCompositor>(MouseButtonType.Left);
            // playerSession.AddCompositor(swipeMouseCompositor)
            //     .AddTo(disposables);
                    
            var swipeTouchPlugin = injectCreator.Create<OneTouchSwipePlugin>();
            touchCompositor.Link<SwipeMember>(swipeTouchPlugin)
                .AddTo(disposables);

            SwipeMember member = playerSession.GetMember<SwipeMember>();
                    
            member.OnDrag
                .Subscribe(delta => player.transform.position += delta.To3())
                .AddTo(disposables);
                    
            layerOrderer.ConnectAfter(data.session, playerSession)
                .AddTo(disposables);
        }
    }
}