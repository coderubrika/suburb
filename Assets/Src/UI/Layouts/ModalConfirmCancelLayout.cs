using Suburb.Utils;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Suburb.UI.Layouts
{
    public class ModalConfirmCancelLayout : ModalConfirmLayout
    {
        [SerializeField] private Button cancelButton;
        [SerializeField] private TMP_Text cancelText;

        protected override void Awake()
        {
            base.Awake();

            cancelButton.OnClickAsObservable()
                .Subscribe(_ => OnResponce.Execute(ExitStatus.Cancel))
                .AddTo(this);
        }

        public override void Init(ModalConfirmInput input)
        {
            cancelText.text = input.CancelIndex;
            base.Init(input);
        }
    }
}