﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using TravelAgent.Core;

namespace TravelAgent.Service
{
    public class NavigationService : ObservableObject
    {
        private ViewModel _currentViewModel;

        public ViewModel CurrentViewModel
        {
            get { return _currentViewModel; }
            private set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        private Func<Type, ViewModel> _viewModelFactory;

        public NavigationService(Func<Type, ViewModel> viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        public void NavigateTo<TViewModel>() where TViewModel : ViewModel
        {
            CurrentViewModel = _viewModelFactory.Invoke(typeof(TViewModel));
        }
    }
}
