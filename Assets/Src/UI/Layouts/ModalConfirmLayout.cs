using Suburb.Screens;
using Suburb.Utils;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Suburb.UI.Layouts
{
    public class ModalConfirmLayout : BaseLayout<IEnumerable<(string, string)>, string>
    {
        [SerializeField] private TMP_Text header;
        [SerializeField] private TMP_Text body;
        [SerializeField] private TMP_Text confirmText;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button closeZoneButton;
        [SerializeField] private RectTransform root;

        protected readonly Dictionary<string, TMP_Text> labels = new();

        public const string HEADER_LABEL = "HEADER_LABEL";
        public const string BODY_LABEL = "BODY_LABEL";
        public const string CONFIRM_LABEL = "CONFIRM_LABEL";

        public const string CLOSE_STATUS = "CLOSE_STATUS";
        public const string CONFIRM_STATUS = "CONFIRM_STATUS";

        protected virtual void Awake()
        {
            labels.Add(HEADER_LABEL, header);
            labels.Add(BODY_LABEL, body);
            labels.Add(CONFIRM_LABEL, confirmText);

            Observable.Merge(
                    closeButton.OnClickAsObservable(),
                    closeZoneButton.OnClickAsObservable())
                .Subscribe(_ => OnResponce.Execute(CLOSE_STATUS))
                .AddTo(this);

            confirmButton.OnClickAsObservable()
                .Subscribe(_ => OnResponce.Execute(CONFIRM_STATUS))
                .AddTo(this);
        }

        public override void Init(IEnumerable<(string, string)> input)
        {
            foreach (var inputItem in input)
                labels[inputItem.Item1].text = inputItem.Item2;
        }

        protected override void Show()
        {
            base.Show();
            UIUtils.UpdateContent(root);
        }
    }
}