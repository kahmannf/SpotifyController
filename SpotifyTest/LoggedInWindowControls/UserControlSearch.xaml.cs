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
    /// Interaktionslogik für UserControlSearch.xaml
    /// </summary>
    public partial class UserControlSearch : UserControl
    {
        public UserControlSearch()
        {
            InitializeComponent();
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (this.DataContext as ViewModelSearch).ReturnHome();
        }

        private void ButtonSimpleSearch_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as ViewModelSearch).StartSimpleSearch();
        }

        private void ButtonAdvancedSearch_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as ViewModelSearch).StartAdvancedSearch();
        }
    }
}
