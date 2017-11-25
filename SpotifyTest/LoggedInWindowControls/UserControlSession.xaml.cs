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
    /// Interaktionslogik für UserControlSession.xaml
    /// </summary>
    public partial class UserControlSession : UserControl
    {
        public UserControlSession()
        {
            InitializeComponent();
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (this.DataContext as ViewModelSession).ReturnHome();
        }

        private void ButtonHighjackSession_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as ViewModelSession).HighjackSession(); 
        }

        private void ButtonStartSession_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as ViewModelSession).StartSession();
        }

        private void ButtonStopSession_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as ViewModelSession).StopSession();
        }
    }
}
