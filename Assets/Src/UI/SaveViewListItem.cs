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
using System.Collections.Generic;

namespace Suburb.UI
{
    public class SaveViewListItem : ItemWithButton<GameCollectedData>
    {
        private ScreensService screensService;
        private SavesService savesService;
        private LayoutService layoutService;
        private GameStateMachine gameStateMachine;
        private LocalizationService localizationService;

        [SerializeField] private TMP_Text header;
        [SerializeField] private TMP_Text dateTime;
        [SerializeField] private Button removeButton;

        [Inject]
        public void Construct(
            SavesService savesService, 
            ScreensService screensService,
            GameStateMachine gameStateMachine,
            LayoutService layoutService,
            LocalizationService localizationService,
            bool isSaveMode)
        {
            this.screensService = screensService;
            this.savesService = savesService;
            this.gameStateMachine = gameStateMachine;
            this.layoutService = layoutService;

            header.text = localizationService.GetLocalizedText(Item.Name);
            dateTime.text = DateTimeUtils.ParseAndFormat(
                Item.SaveTime,
                DateTimeUtils.DETAIL_DATE_TIME_FORMAT,
                DateTimeUtils.SHORT_DATE_TIME_FORMAT);

            Button.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    if (isSaveMode)
                    {
                        ShowModalToRewrite();
                        return;
                    }

                    ShowModalToLoad();

                    if (savesService.HasChanges)
                        return;

                    gameStateMachine.CloseGame();
                    savesService.Select(Item);
                    screensService.GoTo<GameScreen>();
                })
                .AddTo(this);

            removeButton.OnClickAsObservable()
                .Subscribe(_ => ShowModalToRemove())
                .AddTo(this);
        }

        private void ShowModalToRewrite()
        {
            if (!savesService.HasChanges)
                return;

            IDisposable responseDisposable = null;
            responseDisposable = layoutService.Setup<IEnumerable<(string, string)>, string, ModalConfirmLayout>(ModalUtils.AskRewriteSaveInput)
                .Subscribe(status =>
                {
                    responseDisposable.Dispose();

                    if (status == ModalConfirmLayout.CONFIRM_STATUS)
                        savesService.SaveAs(Item.UID);
                })
            .AddTo(this);
        }

        private void ShowModalToLoad()
        {
            if (!savesService.HasChanges)
                return;

            IDisposable responseDisposable = null;
            responseDisposable = layoutService.Setup<IEnumerable<(string, string)>, string, ModalConfirmCancelLayout>(ModalUtils.HaveSaveChangesCancelInput)
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
                        SavesScreen screen = screensService.GoTo<SavesScreen>();
                        screen.SwitchToSave();
                    }
                })
            .AddTo(this);
        }

        private void ShowModalToRemove()
        {
            IDisposable responseDisposable = null;
            responseDisposable = layoutService.Setup<IEnumerable<(string, string)>, string, ModalConfirmLayout>(ModalUtils.HaveSaveChangesInput)
            .Subscribe(status =>
            {
                responseDisposable.Dispose();
                if (status == ModalConfirmLayout.CONFIRM_STATUS)
                    savesService.Delete(Item.UID);
            })
                .AddTo(this);
        }
    }
}