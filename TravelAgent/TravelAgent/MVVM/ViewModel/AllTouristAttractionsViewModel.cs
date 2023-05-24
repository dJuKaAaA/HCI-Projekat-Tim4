using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TravelAgent.MVVM.Model;

namespace TravelAgent.MVVM.ViewModel
{
    public class AllTouristAttractionsViewModel : Core.ViewModel
    {
        public ObservableCollection<TouristAttractionModel> AllTouristAttractions { get; set; }

        private Visibility _toolbarVisibility;

        public Visibility ToolbarVisibility
        {
            get { return _toolbarVisibility; }
            set { _toolbarVisibility = value; OnPropertyChanged(); }
        }

        private readonly Service.TouristAttractionService _touristAttractionService;

        public AllTouristAttractionsViewModel(Service.TouristAttractionService touristAttractionService)
        {
            ToolbarVisibility = MainViewModel.SignedUser?.Type == Core.UserType.Agent ? Visibility.Visible : Visibility.Collapsed;
            AllTouristAttractions = new ObservableCollection<TouristAttractionModel>();

            _touristAttractionService = touristAttractionService;

            LoadAll();
        }

        private async void LoadAll()
        {
            AllTouristAttractions.Clear();
            IEnumerable<TouristAttractionModel> touristAttractions = await _touristAttractionService.GetAll();
            foreach (TouristAttractionModel touristAttraction in touristAttractions)
            {
                AllTouristAttractions.Add(touristAttraction);
            }
        }

    }
}
