using UnityEngine;
using Suburb.Common;
using Zenject;
using TMPro;
using UnityEngine.UI;
using Suburb.Utils;
using UniRx;
using Suburb.Screens;
using Suburb.UI.Screens;
using Suburb.UI.Layouts;

namespace Suburb.UI
{
    public class SaveViewListItem : ItemWithButton<GameCollectedData>
    {
        private ScreensService screensService;
        private SavesService savesService;
        private LayoutService layoutService;
        private GameStateMachine gameStateMachine;

        [SerializeField] private TMP_Text header;
        [SerializeField] private TMP_Text dateTime;
        [SerializeField] private Button removeButton;

        public ReactiveCommand OnRemove = new();

        [Inject]
        public void Construct(
            SavesService savesService, 
            ScreensService screensService,
            GameStateMachine gameStateMachine,
            LayoutService layoutService)
        {
            this.screensService = screensService;
            this.savesService = savesService;
            this.gameStateMachine = gameStateMachine;
            this.layoutService = layoutService;

            header.text = Item.Name;
            dateTime.text = DateTimeUtils.ParseAndFormat(
                Item.SaveTime, 
                DateTimeUtils.DETAIL_DATE_TIME_FORMAT, 
                DateTimeUtils.SHORT_DATE_TIME_FORMAT);

            Button.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    gameStateMachine.CloseGame();
                    savesService.Select(Item);
                    screensService.GoTo<GameScreen>();
                })
                .AddTo(this);

            removeButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    layoutService.Setup<(string, string), bool, ModalConfirmLayout>(("Удаление сохранения", "Вы дейстивительно хотите удалить сохранение? Весь прогресс будет удален."))
                        .Subscribe(isConfirm =>
                        {
                            if (isConfirm)
                            {
                                savesService.Delete(Item.UID);
                                OnRemove.Execute();
                            }
                        })
                        .AddTo(this);
                })
                .AddTo(this);
        }
    }
}