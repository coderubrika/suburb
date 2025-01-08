using System;
using FFA.Battle.UI;
using Suburb.Inputs;
using Suburb.Utils;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace FFA.Battle
{
    public class BattleController
    {
        private readonly BattleService battleService;
        private readonly BattlePreparationView preparationView;
        private readonly BattleFightView fightView;
        private readonly PlayerButton.Pool playerButtonPool;
        
        private readonly CompositeDisposable disposables = new();

        public BattleController(
            BattleService battleService,
            BattlePreparationView preparationView,
            BattleFightView fightView,
            PlayerButton.Pool playerButtonPool)
        {
            this.battleService = battleService;
            this.preparationView = preparationView;
            this.fightView = fightView;
            this.playerButtonPool = playerButtonPool;
            
            fightView.gameObject.SetActive(false);
            preparationView.gameObject.SetActive(false);
        }

        public void StartPreparation()
        {
            fightView.gameObject.SetActive(true);
            fightView.Init();
            preparationView.gameObject.SetActive(true);
            preparationView.Show()
                .ObserveOnMainThread()
                .Subscribe(_ =>
                {
                    preparationView.Hide();
                    preparationView.gameObject.SetActive(false);
                    fightView.gameObject.SetActive(true);
                    fightView.Init();
                    fightView.Show();

                    StartFight();
                })
                .AddTo(disposables);
        }

        private void StartFight()
        {
            // todo optimize by foreach
            foreach (var playerView in battleService.GetPlayersList(BattleSide.Bottom))
                SetupPlayer(playerView);

            foreach (var playerView in battleService.GetPlayersList(BattleSide.Top))
                SetupPlayer(playerView);
        }

        private void SetupPlayer(PlayerView playerView)
        {
            var otherSideView = playerView.Side == BattleSide.Bottom
                ? fightView.GetSideView(BattleSide.Top)
                : fightView.GetSideView(BattleSide.Bottom);
            
            var selfSideView = playerView.Side == BattleSide.Bottom
                ? fightView.GetSideView(BattleSide.Bottom)
                : fightView.GetSideView(BattleSide.Top);

            var playerButton = playerButtonPool.Spawn(playerView.Side, playerView.PlayerData);
            playerView.PlayerController.SetAnchorBackTransform(playerButton.transform);
            
            IDisposable downDisposable = playerButton.Button.OnPointerDownAsObservable()
                .Subscribe(_ => playerView.PlayerController.SetMoveToAnchorBack(true))
                .AddTo(disposables);
            
            IDisposable upDisposable = playerButton.Button.OnPointerUpAsObservable()
                .Subscribe(_ => playerView.PlayerController.SetMoveToAnchorBack(false))
                .AddTo(disposables);
            
            playerView.AddTo(downDisposable);
            playerView.AddTo(upDisposable);
            
            playerButton.transform.SetParent(selfSideView.ButtonsRoot);
            playerButton.transform.localScale = Vector3.one;
            playerButton.transform.localRotation = Quaternion.identity;
            
            playerView.InputSession.AddExcludedRect(otherSideView.RectTransform)
                .AddTo(disposables);

            SwipeMember swipe = playerView.InputSession.GetMember<SwipeMember>();

            IDisposable dragDisposable = null;

            swipe.OnDown
                .Subscribe(downPosition =>
                {
                    playerButton.Button.interactable = false;
                    Vector2 position = downPosition;
                    dragDisposable = swipe.OnDragStart.Merge(swipe.OnDrag)
                        .Subscribe(delta =>
                        {
                            position += delta;
                            if (otherSideView.RectTransform.Contain(position))
                            {
                                playerView.PlayerController.BlockControl(true);
                                otherSideView.PlayActive();
                            }
                            else
                            {
                                playerView.PlayerController.BlockControl(false);
                                otherSideView.PlayBase();
                            }
                        })
                        .AddTo(disposables);
                })
                .AddTo(disposables);

            swipe.OnUp
                .Subscribe(_ =>
                {
                    playerButton.Button.interactable = true;
                    dragDisposable?.Dispose();
                    playerView.PlayerController.BlockControl(false);
                    otherSideView.PlayBase();
                })
                .AddTo(disposables);
        }

        public void Clear()
        {
            disposables.Clear();
            preparationView.Hide();
            preparationView.gameObject.SetActive(false);
            fightView.gameObject.SetActive(false);
            fightView.Hide();
        }
    }
}