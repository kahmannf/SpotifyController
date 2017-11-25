using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace SpotifyController
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged //I  know thats not correct, dont bother me. It was quick and it works
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher thredDispatcher = Dispatcher.CurrentDispatcher;

            Action<string> setUpdateMessage = (message) => 
            {
                thredDispatcher.BeginInvoke(
                (Action<string>)delegate (string s)
                {
                    StatusMessage = s;
                    //totally keeping to the pattern here
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StatusMessage"));
                },
                new object[] { message });
            };

            SpotifyControllerAPI.Web.Authentication.Authenticator authenticator = SpotifyControllerAPI.Web.Authentication.Authenticator.GetInstance();

            if (await authenticator.StartAuthenticatedContext(setUpdateMessage))
            {
                LoggedInWindow window = new LoggedInWindow(authenticator);

                window.Show();

                this.Close();

                ViewModelLoggedIn vm = new ViewModelLoggedIn();

                window.DataContext = vm;

                //vm.SelectedTabItem = vm.TabItems.First(x => x.Name == "start");
            }
        }

        public string HeaderText => "Clicking the Log In-Button will open your default \nwebbrowser and require you to sign in to Spotify";

        public string StatusMessage { get; set; }
    }
}
