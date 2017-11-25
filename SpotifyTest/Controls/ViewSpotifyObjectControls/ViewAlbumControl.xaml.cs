using SpotifyController.LoggedInWindowViewModel;
using SpotifyControllerAPI.Model.Spotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpotifyController.Controls.ViewSpotifyObjectControls
{
    /// <summary>
    /// Interaktionslogik für ViewAlbumControl.xaml
    /// </summary>
    public partial class ViewAlbumControl : UserControl
    {
        public ViewAlbumControl()
        {
            InitializeComponent();

            this.DataContextChanged += ViewAlbumControl_DataContextChanged;

        }
        private void ViewAlbumControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is ViewModelBaseViewObject viewBase && viewBase.BaseViewSource is Album album)
            {
                LayoutGrid.DataContext = new ViewAlbumViewModel(album);
            }
        }
    }
}
