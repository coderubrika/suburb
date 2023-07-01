using Suburb.Screens;
using System;
using System.Collections;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Suburb.UI.Layouts
{
    public class ModalConfirmLayout : BaseLayout<(string, string), bool>
    {
        [SerializeField] private TMP_Text header;
        [SerializeField] private TMP_Text content;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button closeZoneButton;

        [Inject]
        public void Construct()
        {
            Observable.Merge(
                    closeButton.OnClickAsObservable(),
                    closeZoneButton.OnClickAsObservable())
                .Subscribe(_ => OnResponce.Execute(false))
                .AddTo(this);

            confirmButton.OnClickAsObservable()
                .Subscribe(_ => OnResponce.Execute(true))
                .AddTo(this);
        }

        public override void Init((string, string) inputData)
        {
            header.text = inputData.Item1;
            content.text = inputData.Item2;
        }
    }
}