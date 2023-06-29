using UnityEngine;
using Suburb.Common;
using Zenject;
using TMPro;
using UnityEngine.UI;
using Suburb.Utils;
using UniRx;
using Suburb.Screens;
using Suburb.UI.Screens;

namespace Suburb.UI
{
    public class SaveViewListItem : ItemWithButton<GameCollectedData>
    {
        private SavesService savesService;

        [SerializeField] private TMP_Text header;
        [SerializeField] private TMP_Text dateTime;
        [SerializeField] private Button removeButton;

        public Button RemoveButton => removeButton;

        [Inject]
        public void Construct(
            SavesService savesService, 
            ScreensService screensService,
            GameStateMachine gameStateMachine)
        {
            this.savesService = savesService;

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
                .Subscribe(_ => savesService.Delete(Item.UID))
                .AddTo(this);
        }
    }
}