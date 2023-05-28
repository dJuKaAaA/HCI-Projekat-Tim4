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
    public class RestorauntSearchViewModel : Core.ViewModel
    {
        private HashSet<RestorauntSearchType> _searchTypes;

        private Model.RestorauntSearchModel _restorauntSearchModel = new Model.RestorauntSearchModel();

        public Model.RestorauntSearchModel RestorauntSearchModel
        {
            get { return _restorauntSearchModel; }
            set { _restorauntSearchModel = value; OnPropertyChanged(); }
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
                    _searchTypes.Add(RestorauntSearchType.Name);
                    NameVisibility = Visibility.Visible;
                }
                else
                {
                    if (_searchTypes.Contains(RestorauntSearchType.Name))
                    {
                        _searchTypes.Remove(RestorauntSearchType.Name);
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
                    _searchTypes.Add(RestorauntSearchType.Address);
                    AddressVisibility = Visibility.Visible;
                }
                else
                {
                    if (_searchTypes.Contains(RestorauntSearchType.Address))
                    {
                        _searchTypes.Remove(RestorauntSearchType.Address);
                    }
                    AddressVisibility = Visibility.Collapsed;
                }
            }
        }

        private Visibility _starsVisibility = Visibility.Collapsed;

        public Visibility StarsVisibility
        {
            get { return _starsVisibility; }
            set { _starsVisibility = value; OnPropertyChanged(); }
        }

        private bool _isStarsChecked;

        public bool IsStarsChecked
        {
            get { return _isStarsChecked; }
            set 
            { 
                _isStarsChecked = value; 
                OnPropertyChanged(); 
                if (_isStarsChecked)
                {
                    _searchTypes.Add(RestorauntSearchType.Stars);
                    StarsVisibility = Visibility.Visible;
                }
                else
                {
                    if (_searchTypes.Contains(RestorauntSearchType.Stars))
                    {
                        _searchTypes.Remove(RestorauntSearchType.Stars);
                    }
                    StarsVisibility = Visibility.Collapsed;
                }
            }
        }

        private readonly RestorauntService _restorauntService;

        public AllRestorauntsViewModel AllRestorauntsViewModel { get; set; }

        public ICommand SearchCommand { get; }
        public ICommand SelectStarsCommand { get; }
        public ICommand CloseCommand { get; }

        public RestorauntSearchViewModel(
            RestorauntService restorauntService)
        {
            _searchTypes = new HashSet<RestorauntSearchType>();

            _restorauntService = restorauntService;

            SearchCommand = new RelayCommand(OnSearch, CanSearch);
            SelectStarsCommand = new RelayCommand(OnSelectStars, o => true);
            CloseCommand = new RelayCommand(OnClose, o => true);
        }

        private void SetValuesToDefault()
        {
            IsNameChecked = false;
            IsAddressChecked = false;
            IsStarsChecked = false;
            _searchTypes.Clear();
            RestorauntSearchModel = new Model.RestorauntSearchModel();
        }

        private void OnSelectStars(object o)
        {
            int star;
            if (int.TryParse(o.ToString(), out star))
            {
                RestorauntSearchModel.Stars = star;
            }
        }

        private async void OnSearch(object o)
        {
            if (_searchTypes.Count > 0)
            {
                IEnumerable<Model.RestorauntModel> restoraunts = await _restorauntService.Search(_searchTypes, RestorauntSearchModel);
                AllRestorauntsViewModel.Restoraunts.Clear();
                foreach (Model.RestorauntModel restoraunt in restoraunts)
                {
                    AllRestorauntsViewModel.Restoraunts.Add(restoraunt);
                }
            }
            else
            {
                await AllRestorauntsViewModel.LoadAll();
            }

            OnClose(this);
        }

        private bool CanSearch(object o)
        {
            bool canSearch = true;
            if (IsNameChecked)
            {
                canSearch = canSearch && !string.IsNullOrWhiteSpace(RestorauntSearchModel.NameKeyword);
            }
            if (IsAddressChecked)
            {
                canSearch = canSearch && !string.IsNullOrWhiteSpace(RestorauntSearchModel.AddressKeyword);
            }

            return canSearch;
        }

        private void OnClose(object o)
        {
            Window currentWindow = Application.Current.Windows.OfType<RestorauntSearchPopup>().SingleOrDefault(w => w.IsActive);
            currentWindow?.Close();

            SetValuesToDefault();
        }
    }
}
