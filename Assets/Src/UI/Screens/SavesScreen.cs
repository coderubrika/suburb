using Suburb.Screens;
using Suburb.Common;
using Suburb.Utils;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System;
using Suburb.ExpressRouter;
using Suburb.ResourceMaps;
using Suburb.Serialization;
using Suburb.Utils.Serialization;
using UnityEngine.Serialization;

namespace Suburb.UI.Screens
{
    public class SavesScreen : BaseScreen
    {
        private SavesService savesService;
        private InjectCreator injectCreator;
        
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private GameObject emptyMessage;
        [SerializeField] private Button backButton;
        [SerializeField] private Button newSaveButton;
        [SerializeField] private Button saveCurrentButton;
        [SerializeField] private SaveViewListItem saveViewListItemPrefab;
        [SerializeField] private RectTransform itemsMount;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private ValueStartEndAnimationData<float> fadeInAnimationData;
        [SerializeField] private ValueStartEndAnimationData<float> fadeOutAnimationData;
        
        private readonly List<SaveViewListItem> saveViews = new();
        private IDisposable changesDisposable;
        private bool isSaveMode;

        [Inject]
        public void Construct(
            ScreensService screensService, 
            SavesService savesService,
            InjectCreator injectCreator,
            MenuSceneService menuSceneService)
        {
            this.savesService = savesService;
            this.injectCreator = injectCreator;
            
            var intoAnimation = new CompositeAnimation(
                () => UIUtils.FadeCanvas(canvasGroup, fadeInAnimationData),
                menuSceneService.AnimateRight);
            var leaveAnimation = new CompositeAnimation(() => UIUtils.FadeCanvas(canvasGroup, fadeOutAnimationData));
            
            screensService.UseTransition(
                intoAnimation.Animate, 
                Rule.AllToThis(nameof(SavesScreen)),
                MiddlewareOrder.To).AddTo(this);
            
            screensService.UseTransition(
                leaveAnimation.Animate, 
                Rule.ThisToAll(nameof(SavesScreen)),
                MiddlewareOrder.From).AddTo(this);
            
            backButton.OnClickAsObservable()
                .Subscribe(_ => screensService.GoToPrevious())
                .AddTo(this);

            newSaveButton.OnClickAsObservable()
                .Subscribe(_ => savesService.SaveAsNew())
                .AddTo(this);

            saveCurrentButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    saveCurrentButton.interactable = false;
                    savesService.SaveSelected();
                })
                .AddTo(this);
        }

        protected override void Show()
        {
            saveCurrentButton.interactable = savesService.IsSelectedSaved && savesService.HasChanges;
            newSaveButton.interactable = savesService.HasSelectedSave;
            RenderList();

            changesDisposable = savesService.OnChangeSaves
                .Subscribe(type =>
                {
                    if (type is SavesService.ChangeType.Overwrite or SavesService.ChangeType.Save)
                        isSaveMode = false;

                    saveCurrentButton.interactable = savesService.IsSelectedSaved && savesService.HasChanges;
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
                var newItem = injectCreator.Create(saveViewListItemPrefab, itemsMount, data, isSaveMode);
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
