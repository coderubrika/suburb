using Suburb.Screens;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Suburb.UI.Screens
{
    public class SavesScreen : BaseScreen
    {
        private ScreensService screensService;

        [SerializeField] private Button backButton;

        [Inject]
        public void Construct(ScreensService screensService)
        {
            this.screensService = screensService;

            backButton.OnClickAsObservable()
                .Subscribe(_ => screensService.GoToPrevious())
                .AddTo(this);
        }
    }
}
