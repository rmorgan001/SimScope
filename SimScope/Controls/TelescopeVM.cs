using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using ASCOM.DeviceInterface;
using ASCOM.Utilities;
using SimServer.Domain;
using SimServer.Helpers;

namespace SimServer.Controls
{
    public class TelescopeVM : ObservableObject, IDisposable
    {
        #region fields

        private readonly Util _util = new Util();
        private readonly CancellationToken _cts;
        
        #endregion
        
        public TelescopeVM()
        {
            try
            {
                using (new WaitCursor())
                {
                    //token to cancel UI updates
                    _cts = new CancellationToken();

                    //Tracks application connections
                    AppCount = Server.AppCount;

                    //subscribe to Scope property changes
                    Scope.StaticPropertyChanged += PropertyChangedScope;

                    //subscribe to Server property changes
                    Server.StaticPropertyChanged += PropertyChangedServer;

                    //subscribe to Settings property changes
                    Settings.StaticPropertyChanged += PropertyChangedSettings;

                    UpdateUI();

                    // initial view items
                    AtPark = Settings.AtPark;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
        }

        #region Viewmodel

        public void Dispose()
        {
            _util?.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        private static bool CheckParked()
        {
            if (!Settings.AtPark) return false;
            MessageBox.Show("Mount is Parked.  Unpark to move.", "Park Error", MessageBoxButton.OK,MessageBoxImage.Warning);
            return true;
        }

        /// <summary>
        /// Subscription to changes from Scope
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
                            case "LimitAlarm":
                                LimitAlarm = Scope.LimitAlarm;
                                break;
                            case "IsHome":
                                UpdateUI();
                                break;
                            case "Tracking":
                                Tracking = Scope.Tracking;
                                break;
                            case "PierSide":
                                PierSide = Scope.PierSide;
                                break;
                        }
                    }, _cts);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Subscription to changes from Server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PropertyChangedServer(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                ThreadContext.InvokeOnUiThread(
                    delegate
                    {
                        switch (e.PropertyName)
                        {
                            case "AppCount":
                                AppCount = Server.AppCount;
                                break;
                        }
                    }, _cts);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Subscription to changes from Server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PropertyChangedSettings(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                ThreadContext.InvokeOnUiThread(
                    delegate
                    {
                        switch (e.PropertyName)
                        {
                            case "AtPark":
                                AtPark = Settings.AtPark;
                                break;
                        }
                    }, _cts);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateUI()
        {
            Lst = _util.HoursToHMS(Scope.SiderealTime);
            Ra = _util.HoursToHMS(Scope.RaConverted, "h ", ":", "", 2);
            Dec = _util.DegreesToDMS(Scope.DecConverted, "° ", ":", "", 2);
            Az = _util.DegreesToDMS(Scope.Azimuth, "° ", ":", "", 2);
            Alt = _util.DegreesToDMS(Scope.Altitude, "° ", ":", "", 2);
            RaPosition = Scope.RaPosition;
            DecPosition = Scope.DecPosition;
            RaSteps = Scope.RaSteps;
            DecSteps = Scope.DecSteps;
            IsSlewing = Scope.IsSlewing;
            IsHome = Scope.IsHome;
        }

        private ICommand _clickparkcommand;
        public ICommand ClickParkCommand
        {
            get
            {
                return _clickparkcommand ?? (_clickparkcommand = new RelayCommand(
                           param => ClickPark()
                       ));
            }
        }
        private static void ClickPark()
        {
            try
            {
                using (new WaitCursor())
                {
                    var parked = Settings.AtPark;
                    if (parked)
                    {
                        Settings.AtPark = false;
                        Scope.Tracking = true;
                    }
                    else
                    {
                        Scope.GoToPark();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private ICommand _clickhomecommand;
        public ICommand ClickHomeCommand
        {
            get
            {
                return _clickhomecommand ?? (_clickhomecommand = new RelayCommand(
                           param => ClickHome()
                       ));
            }
        }
        private static void ClickHome()
        {
            try
            {
                using (new WaitCursor())
                {
                    if (CheckParked()) return;
                    Scope.GoToHome();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
       
        #endregion

        #region Bindings

        private string _alt;
        public string Alt
        {
            get => _alt;
            set
            {
                if (_alt == value) return;
                _alt = value;
                OnPropertyChanged();
            }
        }

        private int _appCount;
        public int AppCount
        {
            get => _appCount;
            set
            {
                if (_appCount == value) return;
                _appCount = value;
                OnPropertyChanged();
            }
        }

        public bool AtPark
        {
            get => Settings.AtPark;
            set
            {
                ParkButtonContent = value ? "UnPark" : "Park";
                OnPropertyChanged();
            }
        }

        private string _az;
        public string Az
        {
            get => _az;
            set
            {
                if (_az == value) return;
                _az = value;
                OnPropertyChanged();
            }
        }

        private string _dec;
        public string Dec
        {
            get => _dec;
            set
            {
                if (_dec == value) return;
                _dec = value;
                OnPropertyChanged();
            }
        }

        private string _decPosition;
        public string DecPosition
        {
            get => _decPosition;
            set
            {
                if (_decPosition == value) return;
                _decPosition = value;
                OnPropertyChanged();
            }
        }

        private int _decSteps;
        public int DecSteps
        {
            get => _decSteps;
            set
            {
                if (_decSteps == value) return;
                _decSteps = value;
                OnPropertyChanged();
            }
        }

        private bool _flipew;
        public bool FlipEW
        {
            get => _flipew;
            set
            {
                if (_flipew == value) return;
                _flipew = value;
                OnPropertyChanged();
            }
        }

        private bool _flipns;
        public bool FlipNS
        {
            get => _flipns;
            set
            {
                if (_flipns == value) return;
                _flipns = value;
                OnPropertyChanged();
            }
        }

        public int HcSpeed
        {
            get => (int)Settings.HcSpeed;
            set
            {
                if ((int)Settings.HcSpeed == value) return;
                Settings.HcSpeed = (SlewSpeed)value;
                OnPropertyChanged();
            }
        }

        private bool _limitAlarm;
        public bool LimitAlarm
        {
            get => _limitAlarm;
            set
            {
                _limitAlarm = value;
                OnPropertyChanged();
            }
        }

        private bool _ishome;
        public bool IsHome
        {
            get => _ishome;
            set
            {
                if (IsHome == value) return;
                _ishome = value;
                OnPropertyChanged();
            }
        }

        private bool _isslewing;
        public bool IsSlewing
        {
            get => _isslewing;
            set
            {
                if (_isslewing == value) return;
                _isslewing = value;
                OnPropertyChanged();
            }
        }

        private string _lst;
        public string Lst
        {
            get => _lst;
            set
            {
               if (_lst == value) return;
                _lst = value;
                OnPropertyChanged();
            }
        }

        private string _parkbuttoncontent;
        public string ParkButtonContent
        {
            get => _parkbuttoncontent;
            set
            {
                _parkbuttoncontent = value;
                OnPropertyChanged();
            }
        }

        private PierSide _pierSide;
        public PierSide PierSide
        {
            get => _pierSide;
            set
            {
                if (_pierSide == value) return;
                _pierSide = value;
                OnPropertyChanged();
            }
        }

        private string _ra;
        public string Ra
        {
            get => _ra;
            set
            {
                if (_ra == value) return;
                _ra = value;
                OnPropertyChanged();
            }
        }

        private string _raPosition;
        public string RaPosition
        {
            get => _raPosition;
            set
            {
                if (_raPosition == value) return;
                _raPosition = value;
                OnPropertyChanged();
            }
        }

        private int _raSteps;
        public int RaSteps
        {
            get => _raSteps;
            set
            {
                if (_raSteps == value) return;
                _raSteps = value;
                OnPropertyChanged();
            }
        }

        private bool _tracking;
        public bool Tracking
        {
            get => _tracking;
            set
            {
                if (CheckParked()) return;
                if (_tracking == value) return;
                _tracking = value;
                Scope.Tracking = value;
                OnPropertyChanged();
            }
        }
        
        #endregion

        #region Hand Buttons

        private ICommand _hcDownClickUpCommand;
        public ICommand HcDownClickUpCommand
        {
            get
            {
                return _hcDownClickUpCommand ?? (_hcDownClickUpCommand = new RelayCommand(param => DownClickUp()));
            }
            set => _hcDownClickUpCommand = value;
        }
        private void DownClickUp()
        {
            try
            {
                if (CheckParked()) return;
                StartSlew(FlipNS ? SlewDirection.SlewDown : SlewDirection.SlewUp);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private ICommand _hcUpClickUpCommand;
        public ICommand HcUpClickUpCommand
        {
            get
            {
                return _hcUpClickUpCommand ?? (_hcUpClickUpCommand = new RelayCommand(param => UpClickUp()));
            }
            set => _hcUpClickUpCommand = value;
        }
        private void UpClickUp()
        {
            try
            {
                StartSlew(SlewDirection.SlewNone);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private ICommand _hcDownClickRightCommand;
        public ICommand HcDownClickRightCommand
        {
            get
            {
                return _hcDownClickRightCommand ?? (_hcDownClickRightCommand = new RelayCommand(param => DownClickRight()));
            }
            set => _hcDownClickRightCommand = value;
        }
        private void DownClickRight()
        {
            try
            {
                if (CheckParked()) return;
                StartSlew(FlipEW ? SlewDirection.SlewLeft : SlewDirection.SlewRight);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private ICommand _hcUpClickRightCommand;
        public ICommand HcUpClickRightCommand
        {
            get
            {
                return _hcUpClickRightCommand ?? (_hcUpClickRightCommand = new RelayCommand(param => UpClickRight()));
            }
            set => _hcUpClickRightCommand = value;
        }
        private void UpClickRight()
        {
            try
            {
                StartSlew(SlewDirection.SlewNone);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private ICommand _hcDownClickLeftCommand;
        public ICommand HcDownClickLeftCommand
        {
            get
            {
                return _hcDownClickLeftCommand ?? (_hcDownClickLeftCommand = new RelayCommand(param => DownClickLeft()));
            }
            set => _hcDownClickLeftCommand = value;
        }
        private void DownClickLeft()
        {
            try
            {
                if (CheckParked()) return;
                StartSlew(FlipEW ? SlewDirection.SlewRight : SlewDirection.SlewLeft);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private ICommand _hcUpClickLeftCommand;
        public ICommand HcUpClickLeftCommand
        {
            get
            {
                return _hcUpClickLeftCommand ?? (_hcUpClickLeftCommand = new RelayCommand(param => UpClickLeft()));
            }
            set => _hcUpClickLeftCommand = value;
        }
        private void UpClickLeft()
        {
            try
            {
                StartSlew(SlewDirection.SlewNone);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private ICommand _hcUpClickDownCommand;
        public ICommand HcUpClickDownCommand
        {
            get
            {
                return _hcUpClickDownCommand ?? (_hcUpClickDownCommand = new RelayCommand(param => UpClickDown()));
            }
            set => _hcUpClickDownCommand = value;
        }
        private void UpClickDown()
        {
            try
            {
                StartSlew(SlewDirection.SlewNone);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private ICommand _hcDownClickDownCommand;
        public ICommand HcDownClickDownCommand
        {
            get
            {
                return _hcDownClickDownCommand ?? (_hcDownClickDownCommand = new RelayCommand(param => DownClickDown()));
            }
            set => _hcDownClickDownCommand = value;
        }
        private void DownClickDown()
        {
            try
            {
                if (CheckParked()) return;
                StartSlew(FlipNS ? SlewDirection.SlewUp : SlewDirection.SlewDown);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private ICommand _hcDownClickStopCommand;
        public ICommand HcDownClickStopCommand
        {
            get
            {
                return _hcDownClickStopCommand ?? (_hcDownClickStopCommand = new RelayCommand(param => DownClickStop()));
            }
            set => _hcDownClickStopCommand = value;
        }
        private static void DownClickStop()
        {
            try
            {
                Scope.AbortSlew();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void StartSlew(SlewDirection direction)
        {
            if (Settings.AtPark)
            {
                return;
            }

            if (Settings.AlignmentMode == AlignmentModes.algAltAz) return;
            switch (direction)
            {
                case SlewDirection.SlewEast:
                case SlewDirection.SlewRight:
                    Scope.HcMoves((SlewSpeed)HcSpeed, SlewDirection.SlewEast);
                    break;
                case SlewDirection.SlewWest:
                case SlewDirection.SlewLeft:
                    Scope.HcMoves((SlewSpeed)HcSpeed, SlewDirection.SlewWest);
                    break;
                case SlewDirection.SlewNorth:
                case SlewDirection.SlewUp:
                    Scope.HcMoves((SlewSpeed)HcSpeed, SlewDirection.SlewNorth);
                    break;
                case SlewDirection.SlewSouth:
                case SlewDirection.SlewDown:
                    Scope.HcMoves((SlewSpeed)HcSpeed, SlewDirection.SlewSouth);
                    break;
                case SlewDirection.SlewNone:
                    Scope.HcMoves((SlewSpeed)HcSpeed, SlewDirection.SlewNone);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}
