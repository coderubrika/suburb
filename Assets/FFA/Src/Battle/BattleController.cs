using FFA.Battle.UI;
using UniRx;

namespace FFA.Battle
{
    public class BattleController
    {
        private readonly BattlePreparationView preparationView;
        private readonly BattleFightView fightView;
        
        private readonly CompositeDisposable disposables = new();
        
        public BattleController(
            BattlePreparationView preparationView,
            BattleFightView fightView)
        {
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
            //fightView.ColorizeSide()
            //.AddTo
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