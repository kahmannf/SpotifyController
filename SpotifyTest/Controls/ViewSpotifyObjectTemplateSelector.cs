using SpotifyController.LoggedInWindowViewModel;
using SpotifyControllerAPI.Model.Spotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SpotifyController.Controls
{
    public class ViewSpotifyObjectTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            string key = string.Empty;

            switch ((item as ViewModelBaseViewObject)?.BaseViewSource)
            {
                case Track track:
                    key = "";
                    break;
                case Playlist playlist:
                    key = "PlaylistViewTemplate";
                    break;
                case Artist artist:
                    key = "";
                    break;
                case Album album:
                    key = "AlbumViewTemplate";
                    break;
                case User user:
                    key = "";
                    break;
            }

            if (!string.IsNullOrEmpty(key))
                return (DataTemplate)Application.Current.TryFindResource(key);
            else
                return base.SelectTemplate(item, container);
        }
    }
}
