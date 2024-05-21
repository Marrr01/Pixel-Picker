using System;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace WpfApp_
{
    internal class MainWindowVM : ViewModelBase
    {
        public BitmapImage Bitmap { get; private set; }

        #region Actual - значения изображения в окне
        private double actualHeight_;
        public double ActualHeight_
        {
            get => actualHeight_;
            set
            {
                actualHeight_ = value;
                OnPropertyChanged();
                OnPropertyChanged("SelectedActualVerticalValue");
                OnPropertyChanged("SelectedActualVerticalValueStr");
                OnPropertyChanged("ActualHeightStr");
            }
        }
        public string ActualHeightStr => $"Высота изображения в окне: {Math.Round(ActualHeight_, 0)}";

        private double actualWidth_;
        public double ActualWidth_
        {
            get => actualWidth_;
            set
            {
                actualWidth_ = value;
                OnPropertyChanged();
                OnPropertyChanged("SelectedActualHorizontalValue");
                OnPropertyChanged("SelectedActualHorizontalValueStr");
                OnPropertyChanged("ActualWidthStr");
            }
        }
        public string ActualWidthStr => $"Ширина изображения в окне: {Math.Round(ActualWidth_, 0)}";

        public double SelectedActualVerticalValue
        {
            get => Math.Round(map(Height, SelectedVerticalValue, ActualHeight_), 0);
            set
            {
                SelectedVerticalValue = map(ActualHeight_, value, Height);
                OnPropertyChanged("SelectedVerticalValue");
                OnPropertyChanged();
                OnPropertyChanged("SelectedActualVerticalValueStr");
            }
        }
        public string SelectedActualVerticalValueStr => $"Выбранное значение по вертикали в окне: {SelectedActualVerticalValue}";

        public double SelectedActualHorizontalValue
        {
            get => Math.Round(map(Width, SelectedHorizontalValue, ActualWidth_), 0);
            set
            {
                SelectedHorizontalValue = map(ActualWidth_, value, Width);
                OnPropertyChanged("SelectedHorizontalValue");
                OnPropertyChanged();
                OnPropertyChanged("SelectedActualHorizontalValueStr");
            }
        }
        public string SelectedActualHorizontalValueStr => $"Выбранное значение по горизонтали в окне: {SelectedActualHorizontalValue}";
        #endregion

        #region НЕ Actual - значения изображения в пикселях, как есть в файле
        public double Height { get => Bitmap.PixelHeight; }
        public string HeightStr => $"Высота изображения: {Height}";

        public double Width { get => Bitmap.PixelWidth; }
        public string WidthStr => $"Ширина изображения: {Width}";

        public double VerticalCenter { get => Math.Round(Height / 2, 0); }
        public double HorizontalCenter { get => Math.Round(Width / 2, 0); }

        private double selectedVerticalValue;
        public double SelectedVerticalValue
        {
            get => selectedVerticalValue;
            set
            {
                selectedVerticalValue = value;
                OnPropertyChanged("IsVerticalValueChangedStr");
                OnPropertyChanged("SelectedVerticalValueBox");
            }
        }
        public string SelectedVerticalValueStr => $"Выбранное значение по вертикали:";
        public string SelectedVerticalValueBox
        {
            get => SelectedVerticalValue.ToString();
            set
            {
                if (Regex.IsMatch(value, "^\\d+$|^$"))
                {
                    double number;
                    if (double.TryParse(value, out number))
                    {
                        if (number < 0) { SelectedVerticalValue = 0; }
                        else if (number > Height) { SelectedVerticalValue = Height; }
                        else { SelectedVerticalValue = number; }
                    }

                    if (string.IsNullOrEmpty(value)) { SelectedVerticalValue = 0; }

                    OnPropertyChanged("SelectedActualVerticalValue");
                    OnPropertyChanged("SelectedActualVerticalValueStr");
                }
            }
        }

        private double selectedHorizontalValue;
        public double SelectedHorizontalValue
        {
            get => selectedHorizontalValue;
            set
            {
                selectedHorizontalValue = value;
                OnPropertyChanged("IsHorizontalValueChangedStr");
                OnPropertyChanged("SelectedHorizontalValueBox");
            }
        }
        public string SelectedHorizontalValueStr => $"Выбранное значение по горизонтали:";
        public string SelectedHorizontalValueBox
        {
            get => SelectedHorizontalValue.ToString();
            set
            {
                if (Regex.IsMatch(value, "^\\d+$|^$"))
                {
                    double number;
                    if (double.TryParse(value, out number))
                    {
                        if (number < 0) { SelectedHorizontalValue = 0; }
                        else if (number > Width) { SelectedHorizontalValue = Width; }
                        else { SelectedHorizontalValue = number; }
                    }

                    if (string.IsNullOrEmpty(value)) { SelectedHorizontalValue = 0; }

                    OnPropertyChanged("SelectedActualHorizontalValue");
                    OnPropertyChanged("SelectedActualHorizontalValueStr");
                }
            }
        }

        public bool IsVerticalValueChanged => isValueChanged(SelectedVerticalValue, VerticalCenter);
        public string IsVerticalValueChangedStr { get => $"Значение по вертикали изменилось: {IsVerticalValueChanged}"; }

        public bool IsHorizontalValueChanged => isValueChanged(SelectedHorizontalValue, HorizontalCenter);
        public string IsHorizontalValueChangedStr { get => $"Значение по горизонтали изменилось: {IsHorizontalValueChanged}"; }
        #endregion

        private RelayCommand<object> setDefaultValuesCommand;
        public RelayCommand<object> SetDefaultValuesCommand
        {
            get
            {
                return setDefaultValuesCommand ??
                    (setDefaultValuesCommand = new RelayCommand<object>(obj =>
                    {
                        SelectedVerticalValue = VerticalCenter;
                        OnPropertyChanged("SelectedActualVerticalValue");
                        OnPropertyChanged("SelectedActualVerticalValueStr");

                        SelectedHorizontalValue = HorizontalCenter;
                        OnPropertyChanged("SelectedActualHorizontalValue");
                        OnPropertyChanged("SelectedActualHorizontalValueStr");
                    }));
            }
        }

        public MainWindowVM()
        {
            Bitmap = new BitmapImage(new Uri("join-kinds.png", UriKind.Relative));
        }

        private bool isValueChanged(double value1, double value2, double measurementError = 1)
        {
            return (value1 - measurementError) > value2 ||
                   (value1 + measurementError) < value2;
        }

        private double map(double diapason1max, double diapason1value, double diapason2max)
        {
            var diapason1Percent = diapason1max / 100;
            var diapason2Percent = diapason2max / 100;
            var percents = diapason1value / diapason1Percent;
            return Math.Round(diapason2Percent * percents, 0);
        }
    }
}
