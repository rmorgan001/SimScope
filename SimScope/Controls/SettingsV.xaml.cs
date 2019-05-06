using System;
using System.Windows;
using System.Windows.Controls;

namespace SimServer.Controls
{
    /// <inheritdoc>
    ///     <cref></cref>
    /// </inheritdoc>
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsV
    {
        public SettingsV()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var tag = string.Empty;
            if (sender is Button button)
            {
                tag = button.Tag.ToString();
            }

            switch (tag)
            {
                case "ApertureArea":
                    ApertureArea.Text = "0.269";
                    Validation.ClearInvalid(ApertureArea.GetBindingExpression(TextBox.TextProperty) ?? throw new InvalidOperationException());
                    ApertureArea.Focus();
                    break;
                case "ApertureDiameter":
                    ApertureDiameter.Text = "0.2";
                    Validation.ClearInvalid(ApertureDiameter.GetBindingExpression(TextBox.TextProperty) ?? throw new InvalidOperationException());
                    ApertureDiameter.Focus();
                    break;
                case "FocalLength":
                    FocalLength.Text = "1.26";
                    Validation.ClearInvalid(FocalLength.GetBindingExpression(TextBox.TextProperty) ?? throw new InvalidOperationException());
                    FocalLength.Focus();
                    break;
                case "KingRate":
                    KingRate.Text = "15.0369";
                    Validation.ClearInvalid(KingRate.GetBindingExpression(TextBox.TextProperty) ?? throw new InvalidOperationException());
                    KingRate.Focus();
                    break;
                case "LunarRate":
                    LunarRate.Text = "14.685";
                    Validation.ClearInvalid(LunarRate.GetBindingExpression(TextBox.TextProperty) ?? throw new InvalidOperationException());
                    LunarRate.Focus();
                    break;
                case "SiderealRate":
                    SiderealRate.Text = "15.041";
                    Validation.ClearInvalid(SiderealRate.GetBindingExpression(TextBox.TextProperty) ?? throw new InvalidOperationException());
                    SiderealRate.Focus();
                    break;
                case "SolarRate":
                    SolarRate.Text = "15";
                    Validation.ClearInvalid(SolarRate.GetBindingExpression(TextBox.TextProperty) ?? throw new InvalidOperationException());
                    SolarRate.Focus();
                    break;
                case "Latitude":
                    Latitude.Text = "28.3558330535889";
                    Validation.ClearInvalid(Latitude.GetBindingExpression(TextBox.TextProperty) ?? throw new InvalidOperationException());
                    Latitude.Focus();
                    break;
                case "Longitude":
                    Longitude.Text = "-81.8272247314453";
                    Validation.ClearInvalid(Longitude.GetBindingExpression(TextBox.TextProperty) ?? throw new InvalidOperationException());
                    Longitude.Focus();
                    break;
                case "Elevation":
                    Elevation.Text = "50";
                    Validation.ClearInvalid(Elevation.GetBindingExpression(TextBox.TextProperty) ?? throw new InvalidOperationException());
                    Elevation.Focus();
                    break;
            }
        }
    }
}
