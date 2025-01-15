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

        public const string CANCEL_LABEL = "CANCEL_LABEL";
        public const string CANCEL_STATUS = "CANCEL_STATUS";

        protected override void Awake()
        {
            base.Awake();

            labels.Add(CANCEL_LABEL, cancelText);

            cancelButton.OnClickAsObservable()
                .Subscribe(_ => OnResponce.Execute(CANCEL_STATUS))
                .AddTo(this);
        }
    }
}