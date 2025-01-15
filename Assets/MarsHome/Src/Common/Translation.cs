using System;
using System.Collections.Generic;
using UnityEngine;

namespace Suburb.Common
{
    public class Translation
    {
        public SystemLanguage Language { get; private set; }
        public Dictionary<string, string> Translations { get; private set; }

        public Translation(SystemLanguage language, Dictionary<string, string> translations)
        {
            Language = language;
            Translations = translations;
        }
    }
}
