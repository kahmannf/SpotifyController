using SpotifyControllerAPI.Model;
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
    public class PlayableListViewItemTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            string key = string.Empty;

            switch (item)
            {
                case Track track:
                    key = "TrackListViewItem";
                    break;
                case Playlist playlist:
                    key = "PlaylistListViewItem";
                    break;
                case Artist artist:
                    key = "ArtistListViewItem";
                    break;
                case Album album:
                    key = "AlbumListViewItem";
                    break;
                case AggregationSearchTrackItem searchItem:
                    key = "AggrSearchItem";
                    break;
            }

            if (!string.IsNullOrEmpty(key))
                return (DataTemplate)Application.Current.TryFindResource(key);
            else
                return base.SelectTemplate(item, container);
        }
    }
}
