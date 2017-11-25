using SpotifyControllerAPI.Model.Spotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyController.LoggedInWindowViewModel
{
    public abstract class ViewModelBaseViewObject : TabItemBaseViewModel
    {
        public ViewModelBaseViewObject(ViewModelLoggedIn parent) : base(parent) { }

        public override abstract void Update();

        public SpotifyBaseObject BaseViewSource { get; set; }
    }
}
