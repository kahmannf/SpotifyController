using SpotifyControllerAPI.Model.Spotify;
using SpotifyControllerAPI.Web.Authentication;
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
using System.Windows.Shapes;

namespace SpotifyController
{
    /// <summary>
    /// Interaktionslogik für LoggedInWindow.xaml
    /// </summary>
    public partial class LoggedInWindow : Window
    {
        private Authenticator _authenticator;

        public LoggedInWindow(Authenticator authenticator)
        {
            InitializeComponent();

            _authenticator = authenticator;

            this.Closing += LoggedInWindow_Closing;

            /* todo: reconsider this */
            this.Background = new LinearGradientBrush(Color.FromRgb(60, 30, 30), Color.FromRgb(100, 50, 50), 90);
            //this.Background = new LinearGradientBrush(Color.FromRgb(88, 88, 88), Color.FromRgb(88, 0, 0), 90);
        }

        private void LoggedInWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _authenticator.Dispose();
            _authenticator = null;

            this.Closing -= LoggedInWindow_Closing;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                (this.DataContext as ViewModelLoggedIn)?.UpdateSelectedTab();
            }
        }
        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (this.DataContext as ViewModelLoggedIn).SelectedTabItem = (sender as TextBlock).DataContext as LoggedInWindowTabItem;
        }


        #region Commands

        private void CommandBindingAddToSession_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.OriginalSource is FrameworkElement fe && fe.DataContext is SpotifyBaseObject;
        }

        private void CommandBindingAddToSession_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement fe && fe.DataContext is SpotifyBaseObject sbo)
            {
                (this.DataContext as ViewModelLoggedIn).Session.AddItemToBacklog(sbo);
            }
        }

        private void CommandBindingAddToQueue_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.OriginalSource is FrameworkElement fe && fe.DataContext is SpotifyBaseObject;
        }

        private async void CommandBindingAddToQueue_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement fe && fe.DataContext is SpotifyBaseObject sbo)
            {
                await (this.DataContext as ViewModelLoggedIn).Session.AddItemToQueue(sbo);
            }
        }

        private void CommandBindingViewItem_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.OriginalSource is FrameworkElement fe && fe.DataContext is SpotifyBaseObject;
        }

        private void CommandBindingViewItem_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement fe && fe.DataContext is SpotifyBaseObject sbo)
            {
                ViewModelLoggedIn vm = this.DataContext as ViewModelLoggedIn;

                /* unfortunately, the generic method doesnt recognize the derived type of the SpotifyBaseObject. 
                 * This turns in to a problem if the user tries to reload the view tab (because all derived properties will not be return from DataLoader.GetItemByHref<T>()).
                 * Because of that every type has to be handled explicit*/
                switch (sbo)
                {
                    case Playlist playlist:
                        vm.ViewSpotifyBaseObject(playlist);
                        break;
                    case Album album:
                        vm.ViewSpotifyBaseObject(album);
                        break;
                    case User user:
                        vm.ViewSpotifyBaseObject(user);
                        break;
                    case Artist artist:
                        vm.ViewSpotifyBaseObject(artist);
                        break;
                    /* not sure yet whether to implement a view for track or not */
                    //case Track track:
                    //    vm.ViewSpotifyBaseObject(track);
                    //    break;
                    default:
                        
                        throw new Exception($"A object with the type {sbo.GetType().ToString()} cannot be displayed");
                }
            }
        }
        

        private void CommandBindingSetDeviceAsActive_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.OriginalSource is FrameworkElement fe && fe.DataContext is Device;
        }

        private void CommandBindingSetDeviceAsActive_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement fe && fe.DataContext is Device device)
            {
                (this.DataContext as ViewModelLoggedIn).Session.SetActiveDevice(device.Id);
            }
            
        }

        private void CommandBindingCloseViewObjectTab_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.OriginalSource is FrameworkElement fe && fe.DataContext is LoggedInWindowTabItem tabitem && tabitem.Custom;
        }

        private void CommandBindingCloseViewObjectTab_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement fe && fe.DataContext is LoggedInWindowTabItem tabitem)
            {
                (this.DataContext as ViewModelLoggedIn).CloseTab(tabitem);
            }
        }

        private void CommandBindingSwitchToTab_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.OriginalSource is FrameworkElement fe && fe.DataContext is LoggedInWindowTabItem tabitem;
        }

        private void CommandBindingSwitchToTab_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement fe && fe.DataContext is LoggedInWindowTabItem tabitem)
            {
                (this.DataContext as ViewModelLoggedIn).SelectedTabItem = tabitem;
            }
        }

        private void CommandBindingViewAudioAnalysis_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.OriginalSource is FrameworkElement fe && fe.DataContext is Track;
        }

        private void CommandBindingViewAudioAnalysis_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement fe && fe.DataContext is Track track)
            {
                (this.DataContext as ViewModelLoggedIn).ViewAudioAnalysis(track);
            }
        }

        #endregion
    }
}
