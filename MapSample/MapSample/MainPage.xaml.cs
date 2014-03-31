using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Shell;
using Windows.Devices.Geolocation;
using System.Device.Location;

namespace MapSample
{
    public partial class MainPage : PhoneApplicationPage
    {
        private readonly Geolocator _geolocator;
        private readonly MapLayer _locationLayer;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            BuildLocalizedApplicationBar();

            _geolocator = new Geolocator();
            _locationLayer = new MapLayer();
            MyMap.Layers.Add(_locationLayer);
        }

        // Sample code for building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();

            var currentLocation = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative))
            {
                Text = "Current"
            };
            currentLocation.Click += ShowCurrentLocation;
            ApplicationBar.Buttons.Add(currentLocation);

            var trackLocation = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative))
            {
                Text = "Track"
            };
            trackLocation.Click += TrackLocationOnMap;
            ApplicationBar.Buttons.Add(trackLocation);

            var zoomIn = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative))
            {
                Text = "Zoom in"
            };
            zoomIn.Click += (o,e) => MyMap.ZoomLevel++;
            ApplicationBar.Buttons.Add(zoomIn);

            var zoomOut = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative))
            {
                Text = "Zoom out"
            };
            zoomOut.Click += (o,e) => MyMap.ZoomLevel--;
            ApplicationBar.Buttons.Add(zoomOut);           
        }

       
        private async void ShowCurrentLocation(object sender, EventArgs e)
        {
            var geoposition = await _geolocator.GetGeopositionAsync(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(1));

            ShowLocation(geoposition);
        }
        
        private void TrackLocationOnMap(object sender, EventArgs e)
        {
            _geolocator.DesiredAccuracy = PositionAccuracy.High;
            _geolocator.MovementThreshold = 25; // The units are meters.

            _geolocator.StatusChanged += HandleStatusChanged;
            _geolocator.PositionChanged += HandlePositionChanged;
        }
        private void HandleStatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            string status = "";

            switch (args.Status)
            {
                case PositionStatus.Disabled:
                    // the application does not have the right capability or the location master switch is off
                    status = "location is disabled in phone settings";
                    break;
                case PositionStatus.Initializing:
                    // the geolocator started the tracking operation
                    status = "initializing";
                    break;
                case PositionStatus.NoData:
                    // the location service was not able to acquire the location
                    status = "no data";
                    break;
                case PositionStatus.Ready:
                    // the location service is generating geopositions as specified by the tracking parameters
                    status = "ready";
                    break;
                case PositionStatus.NotAvailable:
                    status = "not available";
                    // not used in WindowsPhone, Windows desktop uses this value to signal that there is no hardware capable to acquire location information
                    break;
                case PositionStatus.NotInitialized:
                    // the initial state of the geolocator, once the tracking operation is stopped by the user the geolocator moves back to this state

                    break;
            }

            Dispatcher.BeginInvoke(() =>
            {
                Status.Text = status;
            });
        }

        private void HandlePositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            ShowLocation(args.Position);
        }        

        private void ShowLocation(Geoposition geoposition)
        {
            var fixedCoordinates = CoordinateConverter.ConvertGeocoordinate(geoposition.Coordinate);

            Dispatcher.BeginInvoke(() =>
            {
                ShowLocationOnMap(fixedCoordinates);

                CenterOnLocation(geoposition, fixedCoordinates);
            });
        }

        private void CenterOnLocation(Geoposition geoposition, GeoCoordinate fixedCoordinates)
        {
            Longitude.Text = geoposition.Coordinate.Longitude.ToString();
            Latitude.Text = geoposition.Coordinate.Latitude.ToString();

            MyMap.Center = fixedCoordinates;
        }

        private void ShowLocationOnMap(GeoCoordinate fixedCoordinates)
        {
            var myCircle = new Ellipse
            {
                Fill = new SolidColorBrush(Colors.Green),
                Height = 10,
                Width = 10,
                Opacity = 50
            };

            var myLocationOverlay = new MapOverlay
            {
                Content = myCircle,
                PositionOrigin = new Point(0.5, 0.5),
                GeoCoordinate = fixedCoordinates
            };

            _locationLayer.Add(myLocationOverlay);
        }
    }

    public static class CoordinateConverter
    {
        public static GeoCoordinate ConvertGeocoordinate(Geocoordinate geocoordinate)
        {
            return new GeoCoordinate
                (
                geocoordinate.Latitude,
                geocoordinate.Longitude,
                geocoordinate.Altitude ?? Double.NaN,
                geocoordinate.Accuracy,
                geocoordinate.AltitudeAccuracy ?? Double.NaN,
                geocoordinate.Speed ?? Double.NaN,
                geocoordinate.Heading ?? Double.NaN
                );
        }
    }
}