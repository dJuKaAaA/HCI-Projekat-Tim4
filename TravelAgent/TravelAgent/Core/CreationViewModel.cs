using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TravelAgent.Exception;
using TravelAgent.MVVM.Model;
using TravelAgent.Service;

namespace TravelAgent.Core
{
    public class CreationViewModel : Core.ViewModel
    {
        public bool IsLocationChaned { get; private set; }

        private LocationModel? _location;

        public LocationModel? Location
        {
            get { return _location; }
            set 
            { 
                _location = value; 

                if (_location != null)
                {
                    LocationLabelVisibility = Visibility.Visible;
                    NALocationVisibility = Visibility.Collapsed;
                }
                else
                {
                    LocationLabelVisibility = Visibility.Collapsed;
                    NALocationVisibility = Visibility.Visible;
                }

                OnPropertyChanged(); 
            }
        }

        private Visibility _naLocationVisibility = Visibility.Visible;

        public Visibility NALocationVisibility
        {
            get { return _naLocationVisibility; }
            set { _naLocationVisibility = value; OnPropertyChanged(); }
        }

        private Visibility _locationLabelVisibility = Visibility.Collapsed;

        public Visibility LocationLabelVisibility
        {
            get { return _locationLabelVisibility; }
            set { _locationLabelVisibility = value; OnPropertyChanged(); }
        }

        private string _address;

        public string Address
        {
            get { return _address; }
            set { _address = value; OnPropertyChanged(); }
        }

        private BitmapImage? _image;

		public BitmapImage? Image
		{
			get { return _image; }
			set { _image = value; OnPropertyChanged(); }
		}

        public EventHandler<LocationModel> AddressSearched;

        public MapService MapService { get; }

        public ICommand SearchLocationFromAddressCommand { get; }
        public ICommand PickImageCommand { get; }
        public ICommand ClosePopupCommand { get; protected set; }

        public CreationViewModel(MapService mapService)
        {
            MapService = mapService;

            SearchLocationFromAddressCommand = new RelayCommand(OnSearchLocationFromAddress, o => !string.IsNullOrWhiteSpace(Address));
            PickImageCommand = new RelayCommand(OnPickImage, o => true);
        }

        public void OnPickImage(object o)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.gif)|*.jpg; *.jpeg; *.png; *.gif|All Files (*.*)|*.*";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            dialog.Title = "Select an Image";
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                string filename = dialog.FileName;
                Uri uriSource = new Uri(filename);
                Image = new BitmapImage(uriSource);
            }
        }

        public async void OnSearchLocationFromAddress(object o)
        {
            try
            {
                Location = await MapService.Geocode(Address);
                AddressSearched?.Invoke(this, Location);
            }
            catch (LocationNotFoundException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
