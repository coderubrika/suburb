using System;
using DG.Tweening;
using Suburb.Inputs;
using Suburb.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FFA.UI
{
    public class BattleZoneChoicePreparationView : MonoBehaviour
    {
        private InjectCreator injectCreator;
        private LayerOrderer layerOrderer;
        
        [SerializeField] private AnnouncementOfStartBattle announcementOfStartBattle;
        [SerializeField] private Image spawnPlayersImage;
        
        private readonly CompositeDisposable disposables = new();
        private float spawnPlayersAlpha;

        public ReactiveCommand<(Vector2 Position, ISession session)> OnResponse { get; } = new();
        
        [Inject]
        private void Construct(InjectCreator injectCreator, LayerOrderer layerOrderer)
        {
            this.injectCreator = injectCreator;
            this.layerOrderer = layerOrderer;
            spawnPlayersImage.gameObject.SetActive(false);
            announcementOfStartBattle.gameObject.SetActive(false);
            spawnPlayersAlpha = spawnPlayersImage.color.a;
        }

        public void Init(int numOfPlayers)
        {
            for (int i = 0; i < numOfPlayers; i++)
            {
                OneTouchPluginCompositor touchPluginCompositor = injectCreator.Create<OneTouchPluginCompositor>();
                OneTouchSwipePlugin swipePlugin = injectCreator.Create<OneTouchSwipePlugin>();
                
                var session = new RectBasedSession(spawnPlayersImage.rectTransform);
                session.SetBookResources(false);
                
                session.AddCompositor(touchPluginCompositor)
                    .AddTo(disposables);
                
                touchPluginCompositor.Link<SwipeMember>(swipePlugin)
                    .AddTo(disposables);

                IDisposable sessionDisposable = layerOrderer.ConnectFirst(session);
                SetupSwipeHandling(session, sessionDisposable);
            }
            
            spawnPlayersImage.gameObject.SetActive(true);
            spawnPlayersImage.color = UIUtils.GetNewAlpha(spawnPlayersImage.color, spawnPlayersAlpha);
            announcementOfStartBattle.gameObject.SetActive(false);
        }
        
        public IObservable<Unit> Show(string opponentName)
        {
            return spawnPlayersImage.DOFade(0, 0.4f).ToObservableOnComplete()
                .ObserveOnMainThread()
                .ContinueWith(_ =>
                {
                    spawnPlayersImage.gameObject.SetActive(false);
                    announcementOfStartBattle.gameObject.SetActive(true);
                    return announcementOfStartBattle.PlayStartBattle(opponentName);
                })
                .Do(_ => announcementOfStartBattle.gameObject.SetActive(false));
        }

        public void Hide()
        {
            disposables.Clear();
            DOTween.Kill(spawnPlayersImage);
            announcementOfStartBattle.Hide();
            announcementOfStartBattle.gameObject.SetActive(false);
        }

        private void SetupSwipeHandling(CompositorsSession session, IDisposable sessionDisposable)
        {
            session.GetMember<SwipeMember>().OnDown
                .Subscribe(position =>
                {
                    session.SetBookResources(false);
                    OnResponse.Execute((position, session));
                    sessionDisposable.Dispose();
                })
                .AddTo(disposables);
        }
    }
}