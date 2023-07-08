using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UniRx;

namespace Suburb.Common
{
    public class LocalizationService
    {
        private readonly LanguagesRepository languagesRepository;

        private readonly Dictionary<SystemLanguage, Dictionary<string, string>> translations = new();

        private const string LANGUAGE = "LANGUAGE";

        public ReactiveProperty<SystemLanguage> CurrentLanguage { get; } = new();

        public LocalizationService(LanguagesRepository languagesRepository)
        {
            this.languagesRepository = languagesRepository;

            foreach (TranslationAsset asset in languagesRepository.Items)
                translations[asset.Language] = JsonConvert.DeserializeObject<Dictionary<string, string>>(asset.TranslationsJson.text);

            string currentLangString = PlayerPrefs.GetString(LANGUAGE, Application.systemLanguage.ToString());
            CurrentLanguage.Value = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), currentLangString);
        }

        public void SetLanguage(SystemLanguage language)
        {
            if (CurrentLanguage.Value == language || !translations.ContainsKey(language))
                return;

            CurrentLanguage.Value = language;
        }

        public string GetLocalizedText(string index)
        {
            return translations[CurrentLanguage.Value][index];
        }
    }
}
