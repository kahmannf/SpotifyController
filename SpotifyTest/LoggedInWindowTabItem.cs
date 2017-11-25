using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace SpotifyController
{
    public class LoggedInWindowTabItem : INotifyPropertyChanged
    {
        public LoggedInWindowTabItem(bool custom = false)
        {
            Custom = custom;
        }

        public bool Custom { get; set; }

        private string _name;
        public string Name
        { get => _name;
            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }
        public Control Content { get; set; }
        public LoggedInWindowViewModel.TabItemBaseViewModel ViewModel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
