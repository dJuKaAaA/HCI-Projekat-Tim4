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
    public class AccommodationSearchViewModel : Core.ViewModel
    {
        private HashSet<AccommodationSearchType> _searchTypes;

        private Model.AccommodationSearchModel _accommodationSearchModel = new Model.AccommodationSearchModel();

        public Model.AccommodationSearchModel AccommodationSearchModel
        {
            get { return _accommodationSearchModel; }
            set { _accommodationSearchModel = value; OnPropertyChanged(); }
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
                    _searchTypes.Add(AccommodationSearchType.Name);
                    NameVisibility = Visibility.Visible;
                }
                else
                {
                    if (_searchTypes.Contains(AccommodationSearchType.Name))
                    {
                        _searchTypes.Remove(AccommodationSearchType.Name);
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
                    _searchTypes.Add(AccommodationSearchType.Address);
                    AddressVisibility = Visibility.Visible;
                }
                else
                {
                    if (_searchTypes.Contains(AccommodationSearchType.Address))
                    {
                        _searchTypes.Remove(AccommodationSearchType.Address);
                    }
                    AddressVisibility = Visibility.Collapsed;
                }
            }
        }

        private Visibility _ratingVisibility = Visibility.Collapsed;

        public Visibility RatingVisibility
        {
            get { return _ratingVisibility; }
            set { _ratingVisibility = value; OnPropertyChanged(); }
        }

        private bool _isRatingChecked;

        public bool IsRatingChecked
        {
            get { return _isRatingChecked; }
            set 
            { 
                _isRatingChecked = value; 
                OnPropertyChanged(); 
                if (_isRatingChecked)
                {
                    _searchTypes.Add(AccommodationSearchType.Rating);
                    RatingVisibility = Visibility.Visible;
                }
                else
                {
                    if (_searchTypes.Contains(AccommodationSearchType.Rating))
                    {
                        _searchTypes.Remove(AccommodationSearchType.Rating);
                    }
                    RatingVisibility = Visibility.Collapsed;
                }
            }
        }

        private readonly AccommodationService _accommodationService;

        public AllAccommodationsViewModel AllAccommodationsViewModel { get; set; }

        public ICommand SearchCommand { get; }
        public ICommand CloseCommand { get; }

        public AccommodationSearchViewModel(
            AccommodationService accommodationService)
        {
            _searchTypes = new HashSet<AccommodationSearchType>();

            _accommodationService = accommodationService;

            SearchCommand = new RelayCommand(OnSearch, CanSearch);
            CloseCommand = new RelayCommand(OnClose, o => true);
        }

        private void SetValuesToDefault()
        {
            IsNameChecked = false;
            IsAddressChecked = false;
            IsRatingChecked = false;
            _searchTypes.Clear();
            AccommodationSearchModel = new Model.AccommodationSearchModel();
        }

        private async void OnSearch(object o)
        {
            if (_searchTypes.Count > 0)
            {
                IEnumerable<Model.AccommodationModel> accommodations = await _accommodationService.Search(_searchTypes, AccommodationSearchModel);
                AllAccommodationsViewModel.Accommodations.Clear();
                foreach (Model.AccommodationModel accommodation in accommodations)
                {
                    AllAccommodationsViewModel.Accommodations.Add(accommodation);
                }
            }
            else
            {
                await AllAccommodationsViewModel.LoadAll();
            }

            OnClose(this);
        }

        private bool CanSearch(object o)
        {
            bool canSearch = true;
            if (IsNameChecked)
            {
                canSearch = canSearch && !string.IsNullOrWhiteSpace(AccommodationSearchModel.NameKeyword);
            }
            if (IsAddressChecked)
            {
                canSearch = canSearch && !string.IsNullOrWhiteSpace(AccommodationSearchModel.AddressKeyword);
            }

            return canSearch;
        }

        private void OnClose(object o)
        {
            Window currentWindow = Application.Current.Windows.OfType<AccommodationSearchPopup>().SingleOrDefault(w => w.IsActive);
            currentWindow?.Close();

            SetValuesToDefault();
        }

    }
}
