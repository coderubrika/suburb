using Suburb.Screens;
using Suburb.Common;
using Suburb.Utils;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System;

namespace Suburb.UI.Screens
{
    public class SavesScreen : BaseScreen
    {
        private ScreensService screensService;
        private SavesService savesService;
        private InjectCreator injectCreator;

        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private GameObject emptyMessage;
        [SerializeField] private Button backButton;
        [SerializeField] private Button newSaveButton;
        [SerializeField] private SaveViewListItem saveViewListItemPrefab;
        [SerializeField] private RectTransform itemsMount;

        private readonly List<SaveViewListItem> saveViews = new();
        private IDisposable changesDisposable;
        private bool isSaveMode;

        [Inject]
        public void Construct(
            ScreensService screensService, 
            SavesService savesService,
            InjectCreator injectCreator)
        {
            this.screensService = screensService;
            this.savesService = savesService;
            this.injectCreator = injectCreator;

            backButton.OnClickAsObservable()
                .Subscribe(_ => screensService.GoToPrevious())
                .AddTo(this);

            newSaveButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    savesService.SaveAsNew();
                    RenderList();
                })
                .AddTo(this);
        }

        protected override void Show()
        {
            newSaveButton.interactable = savesService.HasSelectedSave;
            RenderList();

            changesDisposable = savesService.OnChangeSaves
                .Subscribe(type =>
                {
                    if (type == SavesService.ChangeType.Rewrite)
                        isSaveMode = false;

                    RenderList();
                })
                .AddTo(this);

            base.Show();
        }

        protected override void Hide()
        {
            base.Hide();
            changesDisposable?.Dispose();
        }

        private void RenderList()
        {
            GameCollectedData[] saveDatas = savesService.GetSaves();
            saveViews.DestroyGameObjects();

            foreach (var data in saveDatas)
            {
                var newItem = injectCreator.Create<SaveViewListItem>(saveViewListItemPrefab, itemsMount, new object[] { data, isSaveMode });
                saveViews.Add(newItem);
            }

            emptyMessage.SetActive(saveDatas.Length == 0);
            scrollRect.enabled = saveDatas.Length != 0;
            scrollRect.verticalScrollbar.gameObject.SetActive(saveDatas.Length != 0);

            UIUtils.UpdateContent(itemsMount);
        }

        public void SwitchToSave()
        {
            isSaveMode = true;
            RenderList();
        }
    }
}
