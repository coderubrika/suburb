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
using System;

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
                    if (savesService.TmpData.IsDataHasChanges)
                    {
                        IDisposable responseDisposable = null;
                        responseDisposable = layoutService.Setup<ModalConfirmInput, ExitStatus, ModalConfirmCancelLayout>(new ModalConfirmInput
                        {
                            HeaderIndex = "Есть несохраненные изменения",
                            BodyIndex = "Хотите сохранить изменения?",
                            CancelIndex = "Нет",
                            ConfirmIndex = "Да"
                        })
                        .Subscribe(status =>
                        {
                            responseDisposable.Dispose();
                            if (status == ExitStatus.Cancel)
                            {
                                gameStateMachine.CloseGame();
                                savesService.Select(Item);
                                screensService.GoTo<GameScreen>();
                            }

                            if (status == ExitStatus.Confirm)
                            {
                                // еще не готово, гдесь надо вызвать еще одну модалку,
                                // ту что служит для создания нового сохранения
                                // перед этим надо настроить локализацию и сверстать адаптивные модалки, создать Input типы и параметры по умолчанию
                            }
                        })
                        .AddTo(this);

                        return;
                    }

                    gameStateMachine.CloseGame();
                    savesService.Select(Item);
                    screensService.GoTo<GameScreen>();
                })
                .AddTo(this);

            removeButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    IDisposable responseDisposable = null;
                    responseDisposable = layoutService.Setup<ModalConfirmInput, ExitStatus, ModalConfirmLayout>(new ModalConfirmInput
                    {
                        HeaderIndex = "Удаление сохранения",
                        BodyIndex = "Вы дейстивительно хотите удалить сохранение?\n Весь прогресс будет удален.",
                        CancelIndex = "Нет",
                        ConfirmIndex = "Да"
                    })
                        .Subscribe(status =>
                        {
                            responseDisposable.Dispose();
                            if (status == ExitStatus.Confirm)
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