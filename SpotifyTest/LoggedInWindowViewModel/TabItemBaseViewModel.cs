using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyController.LoggedInWindowViewModel
{
    public abstract class TabItemBaseViewModel
    {
        public TabItemBaseViewModel(ViewModelLoggedIn parent)
        {
            _parent = parent;
        }

        protected ViewModelLoggedIn _parent;

        public abstract void Update();

        public void ReturnHome()
        {
            _parent.SelectedTabItem = _parent.TabItems.FirstOrDefault();
        }
    }
}
