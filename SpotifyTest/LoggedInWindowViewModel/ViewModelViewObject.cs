using SpotifyControllerAPI.Model.Spotify;
using SpotifyControllerAPI.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyController.LoggedInWindowViewModel
{
    public class ViewModelViewObject<TSpotifyObject> : ViewModelBaseViewObject, INotifyPropertyChanged
        where TSpotifyObject : SpotifyBaseObject, new()
    {
        #region INotifyPropertyChanged Member + NotifyPropertyChanged Method

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName]string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion

        private TSpotifyObject _viewSource;

        public TSpotifyObject ViewSource
        {
            get => _viewSource;
            set
            {
                BaseViewSource = value; 
                _viewSource = value;
                NotifyPropertyChanged("ViewSource");
                NotifyPropertyChanged("DisplayTypeName");
            }
        }

        public string DisplayTypeName
        {
            get
            {
                return ViewSource?.GetType()?.ToString()?.Split('.')?.LastOrDefault();
            }
        }


        public ViewModelViewObject(ViewModelLoggedIn parent, TSpotifyObject viewSource) : base(parent)
        {
            ViewSource = viewSource;
        }


        public override async void Update()
        {
            _parent.BlockUI();

            ViewSource = await DataLoader.GetInstance().GetItemFromHref<TSpotifyObject>(_viewSource.Href);

            if (_parent.TabItems.FirstOrDefault(x => x.ViewModel == this) is LoggedInWindowTabItem tabItem)
            {
                if (typeof(TSpotifyObject) == typeof(User))
                {
                    tabItem.Name = (ViewSource as User).UIDisplayName;
                }
                else
                {
                    tabItem.Name = ViewSource.Name;
                }
            }

            _parent.UnblockUI();
        }
    }
}
