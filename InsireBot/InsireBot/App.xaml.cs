﻿using DryIoc;
using Maple.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Maple
{
    public partial class App : Application
    {
        private IContainer _container;

        protected override async void OnStartup(StartupEventArgs e)
        {
            _container = await DependencyInjectionFactory.Get();

            var localizationService = _container.Resolve<ILocalizationService>();
            var log = _container.Resolve<IMapleLog>();

            InitializeResources(localizationService);
            InitializeLocalization();

            var shell = await GetShell(localizationService, log);
            shell.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await SaveState();
            ExitInternal(e);
        }

        /// <summary>
        /// Gets the shell control.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        /// <remarks>
        /// order matters alot here, so be careful when modifying this
        /// </remarks>
        private async Task<Shell> GetShell(ILocalizationService service, IMapleLog log)
        {
            using (var vm = _container.Resolve<ISplashScreenViewModel>())
            {
                var shell = _container.Resolve<Shell>();
                var screen = _container.Resolve<SplashScreen>();

                shell.Loaded += (o, args) => screen.Close();
                screen.Show();

                await Task.WhenAll(LoadApplicationData());

                log.Info(Localization.Properties.Resources.AppStart);
                await Task.Delay(TimeSpan.FromSeconds(1));

                return shell;
            }
        }

        /// <summary>
        /// Initializes the resources.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <remarks>
        /// injecting the translation manager into a SharedResourcedictionary,
        /// so that hopefully all usages of the translation extension can be resolved inside of ResourceDictionaries
        /// </remarks>
        private void InitializeResources(ILocalizationService service)
        {
            var url = new Uri("/Maple;component/Resources/Style.xaml", UriKind.RelativeOrAbsolute);
            var styles = new IoCResourceDictionary(service, url);

            Resources.MergedDictionaries.Add(styles);
        }

        private void InitializeLocalization()
        {
            Thread.CurrentThread.CurrentCulture = Core.Properties.Settings.Default.StartUpCulture;
        }

        private IList<Task> LoadApplicationData()
        {
            var tasks = new List<Task>();

            foreach (var item in _container.Resolve<IEnumerable<ILoadableViewModel>>())
                tasks.Add(item.LoadAsync());

            return tasks;
        }

        private async Task SaveState()
        {
            var tasks = new List<Task>();

            var log = _container.Resolve<IMapleLog>();
            log.Info(Localization.Properties.Resources.SavingState);

            foreach (var item in _container.Resolve<IEnumerable<ILoadableViewModel>>())
                tasks.Add(item.SaveAsync());

            await Task.WhenAll(tasks);

            log.Info(Localization.Properties.Resources.SavedState);
        }

        private void ExitInternal(ExitEventArgs e)
        {
            _container.Dispose();
            base.OnExit(e);
        }
    }
}
