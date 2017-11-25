using SpotifyController.LoggedInWindowViewModel;
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

namespace SpotifyController.LoggedInWindowControls
{
    /// <summary>
    /// Interaktionslogik für UserControlStart.xaml
    /// </summary>
    public partial class UserControlStart : UserControl
    {
        public UserControlStart()
        {
            InitializeComponent();
        }

        private void ButtonSession_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as ViewModelStart).SwitchTab("session");
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (this.DataContext as ViewModelStart).SwitchTab((sender as TextBlock).DataContext as LoggedInWindowTabItem);
        }

        private void ButtonPlayerNext_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as ViewModelStart).NextTrack();
        }
    }
}
