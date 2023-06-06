using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TravelAgent.Core;
using TravelAgent.MVVM.View.Popup;
using TravelAgent.Service;

namespace TravelAgent.MVVM.ViewModel.Popup
{
    public class TouristAttractionSearchViewModel : Core.ViewModel
    {
        private HashSet<TouristAttractionSearchType> _searchTypes;

        private Model.TouristAttractionSearchModel _touristAttractionSearchModel = new Model.TouristAttractionSearchModel();

        public Model.TouristAttractionSearchModel TouristAttractionSearchModel
        {
            get { return _touristAttractionSearchModel; }
            set { _touristAttractionSearchModel = value; OnPropertyChanged(); }
        }

        private Visibility _nameVisibility = Visibility.Collapsed;

        public Visibility NameVisibility
        {
            get { return _nameVisibility; }
            set { _nameVisibility = value; OnPropertyChanged(); }
        }

        private bool _isNameChecked;

        public bool IsNameChecked
        {
            get { return _isNameChecked; }
            set 
            { 
                _isNameChecked = value; 
                OnPropertyChanged(); 
                if (_isNameChecked)
                {
                    _searchTypes.Add(TouristAttractionSearchType.Name);
                    NameVisibility = Visibility.Visible;
                }
                else
                {
                    if (_searchTypes.Contains(TouristAttractionSearchType.Name))
                    {
                        _searchTypes.Remove(TouristAttractionSearchType.Name);
                    }
                    NameVisibility = Visibility.Collapsed;
                }
            }
        }

        private Visibility _addressVisibility = Visibility.Collapsed;

        public Visibility AddressVisibility
        {
            get { return _addressVisibility; }
            set { _addressVisibility = value; OnPropertyChanged(); }
        }

        private bool _isAddressChecked;

        public bool IsAddressChecked
        {
            get { return _isAddressChecked; }
            set 
            {
                _isAddressChecked = value; 
                OnPropertyChanged(); 
                if (_isAddressChecked)
                {
                    _searchTypes.Add(TouristAttractionSearchType.Address);
                    AddressVisibility = Visibility.Visible;
                }
                else
                {
                    if (_searchTypes.Contains(TouristAttractionSearchType.Address))
                    {
                        _searchTypes.Remove(TouristAttractionSearchType.Address);
                    }
                    AddressVisibility = Visibility.Collapsed;
                }
            }
        }

        private Visibility _resetSearchVisibility = Visibility.Collapsed;

        public Visibility ResetSearchVisibility
        {
            get { return _resetSearchVisibility; }
            set { _resetSearchVisibility = value; OnPropertyChanged(); }
        }

        private readonly TouristAttractionService _touristAttractionService;

        public AllTouristAttractionsViewModel AllTouristAttractionsViewModel { get; set; }

        private bool _searchCommandRunning = false;

        public ICommand SearchCommand { get; }
        public ICommand ResetSearchCommand { get; }
        public ICommand CloseCommand { get; }

        public TouristAttractionSearchViewModel(
            TouristAttractionService touristAttractionService)
        {
            _searchTypes = new HashSet<TouristAttractionSearchType>();

            _touristAttractionService = touristAttractionService;

            SearchCommand = new RelayCommand(OnSearch, CanSearch);
            ResetSearchCommand = new RelayCommand(OnResetSearch, o => true);
            CloseCommand = new RelayCommand(OnClose, o => true);
        }

        private void SetValuesToDefault()
        {
            IsNameChecked = false;
            IsAddressChecked = false;
            _searchTypes.Clear();
            TouristAttractionSearchModel = new Model.TouristAttractionSearchModel();
        }

        private async void OnResetSearch(object o)
        {
            await AllTouristAttractionsViewModel.LoadAll();
            ResetSearchVisibility = Visibility.Collapsed;
            OnClose(this);
        }

        private async void OnSearch(object o)
        {
            _searchCommandRunning = true;
            
            if (_searchTypes.Count > 0)
            {
                IEnumerable<Model.TouristAttractionModel> touristAttractions = await _touristAttractionService.Search(_searchTypes, TouristAttractionSearchModel);
                AllTouristAttractionsViewModel.TouristAttractions.Clear();
                foreach (Model.TouristAttractionModel touristAttraction in touristAttractions)
                {
                    AllTouristAttractionsViewModel.TouristAttractions.Add(touristAttraction);
                }
                ResetSearchVisibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("No criteria selected!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _searchCommandRunning = false;
                return;
            }

            _searchCommandRunning = false;

            OnClose(this);
        }

        private bool CanSearch(object o)
        {
            bool canSearch = true;
            if (IsNameChecked)
            {
                canSearch = canSearch && !string.IsNullOrWhiteSpace(TouristAttractionSearchModel.NameKeyword);
            }
            if (IsAddressChecked)
            {
                canSearch = canSearch && !string.IsNullOrWhiteSpace(TouristAttractionSearchModel.AddressKeyword);
            }

            return canSearch && !_searchCommandRunning;
        }

        private void OnClose(object o)
        {
            Window currentWindow = Application.Current.Windows.OfType<TouristAttractionSearchPopup>().SingleOrDefault(w => w.IsActive);
            currentWindow?.Close();

            SetValuesToDefault();
        }
    }
}
