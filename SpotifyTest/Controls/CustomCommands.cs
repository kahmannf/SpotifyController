using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpotifyController.Controls
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand AddToQueue = new RoutedUICommand
            (
                                "AddToQueue",
                                "AddToQueue",
                                typeof(CustomCommands)
            );

        public static readonly RoutedUICommand AddToSession = new RoutedUICommand
            (
                                "AddToSession",
                                "AddToSession",
                                typeof(CustomCommands)
            );

        public static readonly RoutedUICommand ViewItem = new RoutedUICommand
            (
                                "ViewItem",
                                "ViewItem",
                                typeof(CustomCommands)
            );

        public static readonly RoutedUICommand SetDeviceAsActive = new RoutedUICommand
            (
                                "SetDeviceAsActive",
                                "SetDeviceAsActive",
                                typeof(CustomCommands)
            );

        public static readonly RoutedUICommand CloseViewObjectTab = new RoutedUICommand
            (
                                "CloseViewObjectTab",
                                "CloseViewObjectTab",
                                typeof(CustomCommands)
            );

        public static readonly RoutedUICommand SwitchToTab = new RoutedUICommand
            (
                                "SwitchToTab",
                                "SwitchToTab",
                                typeof(CustomCommands)
            );

        public static readonly RoutedUICommand ViewAudioAnalysis = new RoutedUICommand
            (
                                "ViewAudioAnalysis",
                                "ViewAudioAnalysis",
                                typeof(CustomCommands)
            );
    }
}
