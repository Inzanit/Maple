﻿using MvvmScarletToolkit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace InsireBot
{
    public class TranslationManager : ObservableObject, ITranslationManager
    {
        //public event EventHandler LanguageChanged;

        public ITranslationProvider TranslationProvider { get; private set; }

        public CultureInfo CurrentLanguage
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
            set
            {
                if (value != Thread.CurrentThread.CurrentUICulture)
                {
                    Thread.CurrentThread.CurrentUICulture = value;
                    //LanguageChanged?.Invoke(this, EventArgs.Empty);
                    OnPropertyChanged();
                }
            }
        }

        public IEnumerable<CultureInfo> Languages
        {
            get
            {
                if (TranslationProvider != null)
                {
                    return TranslationProvider.Languages;
                }
                return Enumerable.Empty<CultureInfo>();
            }
        }

        public TranslationManager(ITranslationProvider provider)
        {
            TranslationProvider = provider;
            Thread.CurrentThread.CurrentCulture = Languages.First(p => p.TwoLetterISOLanguageName == "en");
        }

        public string Translate(string key)
        {
            if (TranslationProvider != null)
            {
                var translatedValue = TranslationProvider.Translate(key);
                if (!string.IsNullOrEmpty(translatedValue))
                    return translatedValue;
            }

            return $"!{key}!";
        }
    }
}