﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TravelAgent.MVVM.Model;
using TravelAgent.MVVM.View.Popup;

namespace TravelAgent.MVVM.ViewModel
{
    public class AllTripsViewModel : Core.ViewModel
    {
        private readonly Service.NavigationService _navigationService;
        private readonly Service.TripService _tripService;

        public ObservableCollection<TripModel> AllTrips { get; set; }

        public ICommand OpenMapLocationDetailsViewCommand { get; }
        public ICommand OpenSeeDealPopupCommand { get; }

        public AllTripsViewModel(
            Service.NavigationService navigationService, 
            Service.TripService tripService)
        {
            _navigationService = navigationService;
            _tripService = tripService;

            OpenMapLocationDetailsViewCommand = new Core.RelayCommand(o => _navigationService.NavigateTo<MapLocationDetailsViewModel>(), o => true);
            OpenSeeDealPopupCommand = new Core.RelayCommand(OnOpenSeeDealPopup , o => true);

            LoadAll();
        }

        private void OnOpenSeeDealPopup(object o)
        {
            if (o is Button seeDealButton)
            {
                double tripId = double.Parse(seeDealButton.Tag.ToString());
                TripModel trip = AllTrips.FirstOrDefault(f => f.Id == tripId);
                SeeDealPopup popup = new SeeDealPopup(trip);
                popup.Show();
            }
        }

        private async void LoadAll()
        {
            AllTrips = new ObservableCollection<TripModel>();

            IEnumerable<TripModel> allTrip = await _tripService.GetAll();
            foreach (TripModel trip in allTrip)
            {
                AllTrips.Add(trip);
            }
        }

    }
}