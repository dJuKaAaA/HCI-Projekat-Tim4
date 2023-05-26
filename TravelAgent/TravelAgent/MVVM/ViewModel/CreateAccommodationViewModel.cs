using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TravelAgent.Core;
using TravelAgent.MVVM.View.Popup;
using TravelAgent.Service;

namespace TravelAgent.MVVM.ViewModel
{
    public class CreateAccommodationViewModel : Core.CreationViewModel
    {
		private BitmapImage _image;

		public BitmapImage Image
		{
			get { return _image; }
			set { _image = value; OnPropertyChanged(); }
		}

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        private string _rating = "0";

        public string Rating
        {
            get { return _rating; }
            set
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        _rating = "0";
                    }

                    double.Parse(value);

                    if (_rating == "0" && value.Length > 1)
                    {
                        if (value[0] == '0')
                        {
                            value = value[1..];
                        }
                        else if (value[1] == '0')
                        {
                            value = value[0] + value[2..];
                        }
                    }
                    _rating = value; 
                    OnPropertyChanged(); 
                }
                catch (FormatException) { }
            }
        }

        private PickLocationPopup? _pickLocationPopup;

        private readonly Consts _consts;
        private readonly NavigationService _navigationService;

        public ICommand OpenLocationPickerCommand { get; }

        public CreateAccommodationViewModel(
            Consts consts,
            NavigationService navigationService,
            MapService mapService) : base(mapService)
        {
            string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            string uriString = $"pack://application:,,,/{assemblyName};component/Image/defaultimg.jpg";
            Uri imageUri = new Uri(uriString);

            Image = new BitmapImage(imageUri);

            _consts = consts;
            _navigationService = navigationService;

            _navigationService.NavigationCompleted += OnNavigationCompleted;

            OpenLocationPickerCommand = new RelayCommand(OnOpenLocationPicker, o => true);
            ClosePopupCommand = new RelayCommand(o => _pickLocationPopup?.Close(), o => _pickLocationPopup != null);

        }

        private void OnNavigationCompleted(object? sender, NavigationEventArgs e)
        {
            if (e.ViewModelType != typeof(CreateAccommodationViewModel))
            {
                _pickLocationPopup?.Close();
                _navigationService.NavigationCompleted -= OnNavigationCompleted;
            }
        }

        private void OnOpenLocationPicker(object o)
        {
            _pickLocationPopup?.Close();
            _pickLocationPopup = new PickLocationPopup()
            {
                DataContext = this
            };
            _pickLocationPopup.Show();
        }

    }
}
