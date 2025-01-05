using Suburb.Screens;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FFA.Screens
{
    public class MainScreen : BaseScreen
    {
        [SerializeField] private Button play;
        [SerializeField] private Button exit;

        [Inject]
        private void Construct(ScreensService screensService)
        {
            exit.OnClickAsObservable()
                .Subscribe(_ => Application.Quit())
                .AddTo(this);
            
            play.OnClickAsObservable()
                .Subscribe(_ => screensService.GoTo<BattleScreen>())
                .AddTo(this);
        }
    }
}