using System;
using TMPro;
using UnityEngine;
using Zenject;
using UniRx;

namespace Suburb.Common
{
    public class LocalizedText : MonoBehaviour
    {
        private LocalizationService localizationService;

        [SerializeField] private TMP_Text text;
        [SerializeField] private string index;

        private SystemLanguage systemLanguage;
        private IDisposable languageDisposable;

        [Inject]
        public void Construct(LocalizationService localizationService)
        {
            this.localizationService = localizationService;
        }

        public void Localize(string index)
        {
            if (string.IsNullOrEmpty(index))
                return;

            this.index = index;
            text.text = localizationService.GetLocalizedText(index);
        }

        private void Awake()
        {
            systemLanguage = localizationService.CurrentLanguage.Value;
            Localize(index);
        }

        private void OnEnable()
        {
            if (systemLanguage != localizationService.CurrentLanguage.Value)
            {
                systemLanguage = localizationService.CurrentLanguage.Value;
                Localize(index);
            }

            languageDisposable = localizationService.CurrentLanguage
                .Where(lang => lang != systemLanguage)
                .Subscribe(lang =>
                {
                    systemLanguage = lang;
                    Localize(index);
                })
                .AddTo(this);
        }

        private void OnDisable()
        {
            languageDisposable?.Dispose();
        }
    }
}