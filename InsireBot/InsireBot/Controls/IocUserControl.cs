﻿using System.Windows.Controls;

namespace Maple
{
    public class IoCUserControl : UserControl, IIocFrameworkElement
    {
        public ITranslationManager TranslationManager { get; private set; }

        public IoCUserControl() : base()
        {
            // for compatibility
        }

        public IoCUserControl(ITranslationManager manager) : base()
        {
            TranslationManager = manager;
        }
    }
}
