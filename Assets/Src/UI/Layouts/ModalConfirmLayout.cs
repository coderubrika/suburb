using Suburb.Screens;
using Suburb.Utils;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Suburb.UI.Layouts
{
    public class ModalConfirmLayout : BaseLayout<ModalConfirmInput, ExitStatus>
    {
        [SerializeField] private TMP_Text header;
        [SerializeField] private TMP_Text body;
        [SerializeField] private TMP_Text confirmText;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button closeZoneButton;
        [SerializeField] private RectTransform root;

        protected virtual void Awake()
        {
            Observable.Merge(
                    closeButton.OnClickAsObservable(),
                    closeZoneButton.OnClickAsObservable())
                .Subscribe(_ => OnResponce.Execute(ExitStatus.Close))
                .AddTo(this);

            confirmButton.OnClickAsObservable()
                .Subscribe(_ => OnResponce.Execute(ExitStatus.Confirm))
                .AddTo(this);
        }

        public override void Init(ModalConfirmInput input)
        {
            header.text = input.HeaderIndex;
            body.text = input.BodyIndex;
            confirmText.text = input.ConfirmIndex;
        }

        protected override void Show()
        {
            base.Show();
            UIUtils.UpdateContent(root);
        }
    }
}