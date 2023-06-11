using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgent.Core
{
    public class NavigationEventArgs : EventArgs
    {
        public Type ViewModelType { get; }
        public object? Extra { get; }

        public NavigationEventArgs(Type viewModelType)
        {
            ViewModelType = viewModelType;
        }

        public NavigationEventArgs(Type viewModelType, object? extra)
        {
            ViewModelType = viewModelType;
            Extra = extra;
        }
    }
}
