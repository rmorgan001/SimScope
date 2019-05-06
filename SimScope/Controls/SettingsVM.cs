using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using ASCOM.DeviceInterface;
using ASCOM.Utilities;
using SimServer.Domain;
using SimServer.Helpers;
using System.IO.Ports;

namespace SimServer.Controls
{
    public class SettingsVM : ObservableObject, IDisposable
    {
        #region Fields

        private readonly CancellationToken _cts;
        
        #endregion

        public SettingsVM()
        {
            try
            {
                using (new WaitCursor())
                {
                    //token to cancel UI updates
                    _cts = new CancellationToken();

                    //subscribe to Settings property changes
                    Settings.StaticPropertyChanged += PropertyChangedSettings;

                    //bindings for lookup lists
                    LoadLists();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
        }

        #region SettingsVM

        public void Dispose()
        {

        }

        /// <summary>
        /// Lists for the UI comboboxes
        /// </summary>
        private void LoadLists()
        {
            GuideRatesX = new List<double>(Numbers.InclusiveRange(.1, .9));
            GuideRatesY = new List<double>(Numbers.InclusiveRange(.1, .9));
            MaxSlewRates = new List<double>(Numbers.InclusiveRange(2.0, 5));
            HourAngleLimits = new List<double>(Numbers.InclusiveRange(0, 15, 1));
            Temperatures = new List<double>(Numbers.InclusiveRange(-50, 60, 1.0));
            DataBits = new List<int> {7,8};
        }

        /// <summary>
        /// Subscription changes from Scope
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
                            case "AlignmentMode":
                                AlignmentMode = Settings.AlignmentMode;
                                break;
                            case "ApertureArea":
                                ApertureArea = Settings.ApertureArea;
                                break;
                            case "ApertureDiameter":
                                ApertureDiameter = Settings.ApertureDiameter;
                                break;
                            case "Elevation":
                                Elevation = Settings.Elevation;
                                break;
                            case "EquatorialCoordinateType":
                                EquatorialCoordinateType = Settings.EquatorialCoordinateType;
                                break;
                            case "FocalLength":
                                FocalLength = Settings.FocalLength;
                                break;
                            case "GuideRateXPer":
                                GuideRateXPer = Settings.GuideRateXPer;
                                break;
                            case "GuideRateYPer":
                                GuideRateYPer = Settings.GuideRateYPer;
                                break;
                            case "HourAngleLimit":
                                HourAngleLimit = Settings.HourAngleLimit;
                                break;
                            case "KingRate":
                                KingRate = Settings.KingRate;
                                break;
                            case "Latitude":
                                Latitude = Settings.Latitude;
                                break;
                            case "Longitude":
                                Longitude = Settings.Longitude;
                                break;
                            case "LunarRate":
                                LunarRate = Settings.LunarRate;
                                break;
                            case "MaximumSlewRate":
                                MaximumSlewRate = Settings.MaximumSlewRate;
                                break;
                            case "SiderealRate":
                                SiderealRate = Settings.SiderealRate;
                                break;
                            case "SolarRate":
                                SolarRate = Settings.SolarRate;
                                break;
                            case "Temperature":
                                Temperature = Settings.Temperature;
                                break;
                            case "TrackingRate":
                                TrackingRate = Settings.TrackingRate;
                                break;
                        }
                    }, _cts);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region Binding Settings     

        public AlignmentModes AlignmentMode
        {
            get => Settings.AlignmentMode;
            set
            {
                Settings.AlignmentMode = value;
                OnPropertyChanged();
            }
        }
        
        public double ApertureArea
        {
            get => Settings.ApertureArea;
            set
            {
                Settings.ApertureArea = value;
                OnPropertyChanged();
            }
        }

        public double ApertureDiameter
        {
            get => Settings.ApertureDiameter;
            set
            {
                Settings.ApertureDiameter = value;
                OnPropertyChanged();
            }
        }

        public SerialSpeed BaudRate
        {
            get => Settings.BaudRate;
            set
            {
                Settings.BaudRate = value;
                OnPropertyChanged();
            }
        }

        public IList<string> ComPorts
        {
            get
            {
                var ports = new List<string>();
                foreach (var item in SerialPort.GetPortNames())
                {
                    if (string.IsNullOrEmpty(item)) continue;
                    ports.Add(item);
                }
                return ports;
            }
        }
        public string ComPort
        {
            get => Settings.ComPort;
            set
            {
                Settings.ComPort = value;
                OnPropertyChanged();
            }
        }

        public IList<int> DataBits { get; set; }
        public int DataBit
        {
            get => Settings.DataBit;
            set
            {
                Settings.DataBit = value;
                OnPropertyChanged();
            }
        }

        public bool DtrEnable
        {
            get => Settings.DTREnable;
            set
            {
                Settings.DTREnable = value;
                OnPropertyChanged();
            }
        }

        public double Elevation
        {
            get => Settings.Elevation;
            set
            {
                Settings.Elevation = value;
                OnPropertyChanged();
            }
        }

        public EquatorialCoordinateType EquatorialCoordinateType
        {
            get => Settings.EquatorialCoordinateType;
            set
            {
                Settings.EquatorialCoordinateType = value;
                OnPropertyChanged();
            }
        }

        public double FocalLength
        {
            get => Settings.FocalLength;
            set
            {
                Settings.FocalLength = value;
                OnPropertyChanged();
            }
        }

        public IList<double> GuideRatesX { get; set; }
        public double GuideRateXPer
        {
            get => Settings.GuideRateXPer;
            set
            {
                Settings.GuideRateXPer = value;
                OnPropertyChanged();
            }
        }

        public IList<double> GuideRatesY { get; set; }
        public double GuideRateYPer
        {
            get => Settings.GuideRateYPer;
            set
            {
                Settings.GuideRateYPer = value;
                OnPropertyChanged();
            }
        }

        public SerialHandshake HandShake
        {
            get => Settings.HandShake;
            set
            {
                Settings.HandShake = value;
                OnPropertyChanged();
            }
        }

        public IList<double> HourAngleLimits { get; set; }
        public double HourAngleLimit
        {
            get => Settings.HourAngleLimit;
            set
            {
                Settings.HourAngleLimit = value;
                OnPropertyChanged();
            }
        }

        public double KingRate
        {
            get => Settings.KingRate;
            set
            {
                Settings.KingRate = value;
                OnPropertyChanged();
            }
        }

        public double Latitude
        {
            get => Settings.Latitude;
            set
            {
                Settings.Latitude = value;
                OnPropertyChanged();
            }
        }

        public double Longitude
        {
            get => Settings.Longitude;
            set
            {
                Settings.Longitude = value;
                OnPropertyChanged();
            }
        }

        public double LunarRate
        {
            get => Settings.LunarRate;
            set
            {
                Settings.LunarRate = value;
                OnPropertyChanged();
            }
        }

        public IList<double> MaxSlewRates { get; set; }
        public double MaximumSlewRate
        {
            get => Settings.MaximumSlewRate;
            set
            {
                Settings.MaximumSlewRate = value;
                OnPropertyChanged();
            }
        }

        public bool RTSEnable
        {
            get => Settings.RTSEnable;
            set
            {
                Settings.RTSEnable = value;
                OnPropertyChanged();
            }
        }

        public double SiderealRate
        {
            get => Settings.SiderealRate;
            set
            {
                Settings.SiderealRate = value;
                OnPropertyChanged();
            }
        }

        public double SolarRate
        {
            get => Settings.SolarRate;
            set
            {
                Settings.SolarRate = value;
                OnPropertyChanged();
            }
        }

        public IList<double> Temperatures { get; set; }
        public double Temperature
        {
            get => Settings.Temperature;
            set
            {
                Settings.Temperature = value;
                OnPropertyChanged();
            }
        }

        public DriveRates TrackingRate
        {
            get => Settings.TrackingRate;
            set
            {
                Settings.TrackingRate = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Button Commands

        private ICommand _clickResetCommand;
        public ICommand ClickResetCommand =>
            _clickResetCommand ?? (_clickResetCommand = new RelayCommand(
                ClickReset
            ));
        private void ClickReset(object param)
        {
            try
            {
                using (new WaitCursor())
                {
                    switch (param)
                    {
                        case "AlignmentMode":
                            AlignmentMode = AlignmentModes.algGermanPolar;
                            break;
                        case "EquatorialCoordinateType":
                            EquatorialCoordinateType = EquatorialCoordinateType.equLocalTopocentric;
                            break;
                        case "GuideRateXPer":
                            GuideRateXPer = 0.5;
                            break;
                        case "GuideRateYPer":
                            GuideRateYPer = 0.5;
                            break;
                        case "HourAngleLimit":
                            HourAngleLimit = 10;
                            break;
                        case "MaximumSlewRate":
                            MaximumSlewRate = 3.5;
                            break;
                        case "Temperature":
                            Temperature = 20;
                            break;
                        case "TrackingRate":
                            TrackingRate = DriveRates.driveSidereal;
                            break;
                        case "DtrEnable":
                            DtrEnable = false;
                            break;
                        case "RTSEnable":
                            RTSEnable = false;
                            break;
                        case "DataBit":
                            DataBit = 8;
                            break;
                        case "BaudRate":
                            BaudRate = SerialSpeed.ps9600;
                            break;
                        case "HandShake":
                            HandShake = SerialHandshake.None;
                            break;
                        case "ComPort":
                            ComPort = "COM1";
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private ICommand _clickSetParkCommand;
        public ICommand ClickSetParkCommand
        {
            get
            {
                return _clickSetParkCommand ?? (_clickSetParkCommand = new RelayCommand(
                           param => ClickSetPark()
                       ));
            }
        }
        private void ClickSetPark()
        {
            try
            {
                using (new WaitCursor())
                {
                    Scope.SetParkAxes();
                    MessageBox.Show("Park Set", "Settings", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private ICommand _clickSaveCloseCommand;
        public ICommand ClickSaveCloseCommand
        {
            get
            {
                return _clickSaveCloseCommand ?? (_clickSaveCloseCommand = new RelayCommand(
                           param => ClickSaveClose()
                       ));
            }
        }
        private void ClickSaveClose()
        {
            try
            {
                using (new WaitCursor())
                {
                    Settings.Save();
                    Scope.OpenSetupDialogFinished = true;
                    MessageBox.Show("Settings Saved","Settings",MessageBoxButton.OK,MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        
        #endregion
    }
}
