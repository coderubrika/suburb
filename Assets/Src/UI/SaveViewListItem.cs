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
                        responseDisposable = layoutService.Setup<(string, string)[], string, ModalConfirmCancelLayout>(new (string, string)[]
                        {
                            (ModalConfirmLayout.HEADER_LABEL, "Есть несохраненные изменения"),
                            (ModalConfirmLayout.BODY_LABEL, "Хотите сохранить изменения?"),
                            (ModalConfirmLayout.CONFIRM_LABEL, "Да"),
                            (ModalConfirmCancelLayout.CANCEL_LABEL, "Нет")
                        })
                            .Subscribe(status =>
                        {
                            responseDisposable.Dispose();
                            if (status == ModalConfirmCancelLayout.CANCEL_STATUS)
                            {
                                gameStateMachine.CloseGame();
                                savesService.Select(Item);
                                screensService.GoTo<GameScreen>();
                            }

                            if (status == ModalConfirmLayout.CONFIRM_STATUS)
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
                    responseDisposable = layoutService.Setup<(string, string)[], string, ModalConfirmLayout>(new (string, string)[]
                    {
                        (ModalConfirmLayout.HEADER_LABEL, "Есть несохраненные изменения"),
                        (ModalConfirmLayout.BODY_LABEL, "Хотите сохранить изменения?"),
                        (ModalConfirmLayout.CONFIRM_LABEL, "Да"),
                    })
                    .Subscribe(status =>
                        {
                            responseDisposable.Dispose();
                            if (status == ModalConfirmLayout.CONFIRM_STATUS)
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