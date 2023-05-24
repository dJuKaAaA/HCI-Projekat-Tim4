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
    public class AllRestorauntsViewModel : Core.ViewModel
    {
        public ObservableCollection<RestorauntModel> AllRestoraunts { get; set; }

        private Visibility _toolbarVisibility;

        public Visibility ToolbarVisibility
        {
            get { return _toolbarVisibility; }
            set { _toolbarVisibility = value; OnPropertyChanged(); }
        }

        private readonly Service.RestorauntService _restorauntService;

        public AllRestorauntsViewModel(Service.RestorauntService restorauntService)
        {
            ToolbarVisibility = MainViewModel.SignedUser?.Type == Core.UserType.Agent ? Visibility.Visible : Visibility.Collapsed;
            AllRestoraunts = new ObservableCollection<RestorauntModel>();

            _restorauntService = restorauntService;

            LoadAll();
        }

        private async void LoadAll()
        {
            AllRestoraunts.Clear();
            IEnumerable<RestorauntModel> restoraunts = await _restorauntService.GetAll();
            foreach (RestorauntModel restoraunt in restoraunts)
            {
                AllRestoraunts.Add(restoraunt);
            }
        }
    }
}
