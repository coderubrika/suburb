using System;
using FFA.Battle.UI;
using Suburb.Inputs;
using Suburb.Utils;
using UniRx;
using UnityEngine;

namespace FFA.Battle
{
    public class BattleController
    {
        private readonly BattleService battleService;
        private readonly BattlePreparationView preparationView;
        private readonly BattleFightView fightView;

        private readonly CompositeDisposable disposables = new();

        public BattleController(
            BattleService battleService,
            BattlePreparationView preparationView,
            BattleFightView fightView)
        {
            this.battleService = battleService;
            this.preparationView = preparationView;
            this.fightView = fightView;

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
            var sideView = playerView.Side == BattleSide.Bottom
                ? fightView.GetSideView(BattleSide.Top)
                : fightView.GetSideView(BattleSide.Bottom);
            
            playerView.InputSession.AddExcludedRect(sideView.RectTransform)
                .AddTo(disposables);

            SwipeMember swipe = playerView.InputSession.GetMember<SwipeMember>();

            IDisposable dragDisposable = null;

            swipe.OnDown
                .Subscribe(downPosition =>
                {
                    Vector2 position = downPosition;
                    dragDisposable = swipe.OnDragStart.Merge(swipe.OnDrag)
                        .Subscribe(delta =>
                        {
                            position += delta;
                            if (sideView.RectTransform.Contain(position))
                            {
                                playerView.BlockControl(true);
                                sideView.PlayActive();
                            }
                            else
                            {
                                playerView.BlockControl(false);
                                sideView.PlayBase();
                            }
                        })
                        .AddTo(disposables);
                })
                .AddTo(disposables);

            swipe.OnUp
                .Subscribe(_ =>
                {
                    dragDisposable?.Dispose();
                    playerView.BlockControl(false);
                    sideView.PlayBase();
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