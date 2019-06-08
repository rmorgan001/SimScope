using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using SimServer.Controls;
using SimServer.Domain;
using SimServer.Helpers;

namespace SimServer.Windows
{
    public sealed class MainWindowVM : ObservableObject
    {
        private readonly CancellationToken _cts;
        
        public MainWindowVM()
        {
            //token to cancel UI updates
            _cts = new CancellationToken();

            //subscribe to Scope property changes
            Scope.StaticPropertyChanged += PropertyChangedScope;

            // Deals with applications trying to open the setup dialog more than once. 
            OpenSetupDialog = Scope.OpenSetupDialog;

            // Name from mount data
            MountName = Scope.MountName;

            //viewmodels for selected tab
            ViewModels = new ObservableCollection<object>
            {
                new TelescopeVM(),
                new SettingsVM()
            };
        }

        /// <summary>
        /// Binding to the Tab DataContext
        /// </summary>
        public ObservableCollection<object> ViewModels { get; }

        /// <summary>
        /// Binding to the selected tab
        /// </summary>
        private int _selectedTab;
        public int SelectedTab
        {
            get => _selectedTab;
            set
            {
                if (_selectedTab == value) return;
                _selectedTab = value;
                if(value != 1) Scope.OpenSetupDialog = false;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// SetupDialog requests from driver
        /// </summary>
        private bool _openSetupDialog;
        private bool OpenSetupDialog
        {
            set
            {
                if (value == _openSetupDialog) return;
                _openSetupDialog = value;
                if (value) SelectedTab = 1;
            }
        }

        private string _mountName;
        public string MountName
        {
            get => _mountName;
            set
            {
                if (_mountName == value) return;
                _mountName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Subscription changes from Scope
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PropertyChangedScope(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                ThreadContext.InvokeOnUiThread(
                    delegate
                    {
                        switch (e.PropertyName)
                        {
                            case "OpenSetupDialog":
                                OpenSetupDialog = Scope.OpenSetupDialog;
                                break;
                        }
                    }, _cts);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
