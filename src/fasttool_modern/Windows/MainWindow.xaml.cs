using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Threading;
using AudioSwitcher.AudioApi.CoreAudio;
using System.Diagnostics;
using System.Threading.Tasks;
using fasttool_modern.Services;




namespace fasttool_modern
{
    public sealed partial class MainWindow : Window
    {
      
        public string selectedImage = "";

        SerialPortManager serialPortManager = SerialPortManager.Instance;
        public MainWindow()
        {
            this.InitializeComponent();
            NavView.SelectedItem = NavView.MenuItems[0];
            
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            serialPortManager.ConnectDevice();
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = args.SelectedItem as NavigationViewItem;
            var pageTag = selectedItem.Tag.ToString();

            switch (pageTag)
            {
                case "DashboardPage":
                    ContentFrame.Navigate(typeof(DashboardPage));
                    NavView.Header = "Dashboard";
                    break;

                case "ProfilesPage":
                    ContentFrame.Navigate(typeof(ProfilesPage));
                    NavView.Header = "Profiles";
                    break;

                case "ConnectionPage":
                    ContentFrame.Navigate(typeof(ConnectionPage));
                    NavView.Header = "Connection";
                    break;

                case "SettingsPage":
                    ContentFrame.Navigate(typeof(SettingsPage));
                    NavView.Header = "Settings";
                    break;
            }
        } 
    }
}
