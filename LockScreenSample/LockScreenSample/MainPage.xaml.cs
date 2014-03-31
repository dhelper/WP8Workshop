using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Windows.Phone.System.UserProfile;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LockScreenSample.Resources;

namespace LockScreenSample
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}

        private int _count;

        private void UpdateData_OnClick(object sender, RoutedEventArgs e)
        {
            ShellTile.ActiveTiles.First().Update(
                new FlipTileData
                {
                    Count = _count++,
                    WideBackContent = "You are here!",
                    SmallBackgroundImage = new Uri(@"Assets\Tiles\FlipCycleTileSmall.png", UriKind.Relative),
                    BackgroundImage = new Uri(@"Assets\Tiles\FlipCycleTileMedium.png", UriKind.Relative),
                    BackBackgroundImage = new Uri(@"Assets\Tiles\FlipCycleTileMedium.png", UriKind.Relative)
                });
        }

        private async void ChangeImage_OnClick(object sender, RoutedEventArgs e)
        {
            if (!LockScreenManager.IsProvidedByCurrentApplication)
            {
                await LockScreenManager.RequestAccessAsync();
            }

            if (LockScreenManager.IsProvidedByCurrentApplication)
            {
                var value = new Uri(@"ms-appx:///assets/me.jpg", UriKind.Absolute);
                LockScreen.SetImageUri(value);
            }
        }

        private async void GotoSettings_OnClick(object sender, RoutedEventArgs e)
        {
            var op = await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-lock:"));
        }
    }
}