﻿using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;

namespace InsireBot
{
    /// <summary>
    /// viewmodel for creating mediaitem from a string (path/url)
    /// </summary>
    public class CreateMediaItemViewModel : MediaItemsStore
    {
        private string _source;
        public string Source
        {
            get { return _source; }
            set
            {
                if (_source != value)
                {
                    _source = value;
                    RaisePropertyChanged(nameof(Source));
                }
            }
        }

        private DataParsingServiceResult _result;
        public DataParsingServiceResult Result
        {
            get { return _result; }
            private set
            {
                if (_result != value)
                {
                    _result = value;
                    RaisePropertyChanged(nameof(Result));
                    Items.AddRange(value.MediaItems);
                }
            }
        }

        public ICommand ParseCommand { get; private set; }

        public CreateMediaItemViewModel()
        {
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            ParseCommand = new RelayCommand(async () =>
            {
                BusyStack.Push();
                Result = await GlobalServiceLocator.Instance.DataParsingService
                                                            .Parse(Source)
                                                            .ContinueWith((task) =>
                                                            {
                                                                if (!BusyStack.Pull())
                                                                    throw new InsireBotException($"Couldn't pull from BusyStack. ({GetType()})");

                                                                if (task.Exception != null)
                                                                    throw task.Exception;

                                                                return task.Result;
                                                            });
            });
        }
    }
}
