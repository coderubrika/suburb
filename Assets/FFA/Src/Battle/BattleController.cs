using FFA.Battle.UI;
using UniRx;

namespace FFA.Battle
{
    public class BattleController
    {
        private readonly BattlePreparationView preparationView;
        private readonly CompositeDisposable disposables = new();
        
        public BattleController(BattlePreparationView preparationView)
        {
            this.preparationView = preparationView;
            preparationView.gameObject.SetActive(false);
        }
        
        public void StartPreparation()
        {
            preparationView.gameObject.SetActive(true);
            preparationView.Show()
                .Subscribe(_ =>
                {
                    preparationView.Hide();
                    preparationView.gameObject.SetActive(false);
                    StartFight();
                })
                .AddTo(disposables);
        }

        private void StartFight()
        {
            
        }

        public void Clear()
        {
            disposables.Clear();
            preparationView.Hide();
            preparationView.gameObject.SetActive(false);
        }
    }
}