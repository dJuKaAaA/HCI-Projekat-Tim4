using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TravelAgent.Core;
using TravelAgent.Exception;
using Newtonsoft.Json.Linq;
using Microsoft.Maps.MapControl.WPF;
using System.Reflection.Metadata.Ecma335;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TravelAgent.MVVM.Model;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Data;

namespace TravelAgent.Service
{
    public class LocationResult
    {
        public string Name { get; set; }
    }

    public class MapService
    {
        private readonly HttpClient _httpClient;
        private readonly Consts _consts;

        public DependencyProperty RenderTransformProperty { get; private set; }

        public MapService(Consts consts)
        {
            _consts = consts;
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://dev.virtualearth.net/")
            };
        }

        public async Task<LocationModel> ReverseGeocode(double latitude, double longitude)
        {
            string apiEndpoint = $"REST/v1/Locations/{latitude},{longitude}?key={_consts.BingMapsApiKey}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiEndpoint);

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            string responseBodyStringified = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return GetLocationFromResponse(responseBodyStringified);
            }

            throw new LocationNotFoundException();

        }
        
        public async Task<LocationModel> Geocode(string address)
        {
            string apiEndpoint = $"REST/v1/Locations?q={address}&key={_consts.BingMapsApiKey}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiEndpoint);

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            string responseBodyStringified = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return GetLocationFromResponse(responseBodyStringified);
            }

            throw new LocationNotFoundException();
        }

        private LocationModel GetLocationFromResponse(string response)
        {
            try
            {
                JObject jsonObject = JObject.Parse(response);
                string address = jsonObject["resourceSets"][0]["resources"][0]["name"].ToString();

                JObject json = JObject.Parse(response);

                JArray geocodePoints = json["resourceSets"][0]["resources"][0]["geocodePoints"] as JArray;
                if (geocodePoints != null && geocodePoints.Count > 0)
                {
                    JArray coordinates = geocodePoints[0]["coordinates"] as JArray;
                    if (coordinates != null && coordinates.Count == 2)
                    {
                        double latitude = (double)coordinates[0];
                        double longitude = (double)coordinates[1];

                        return new LocationModel()
                        {
                            Latitude = latitude,
                            Longitude = longitude,
                            Address = address
                        };
                    }
                    else
                    {
                        throw new LocationNotFoundException();
                    }
                }
                else
                {
                    throw new LocationNotFoundException();
                }
            }
            catch
            {
                throw new LocationNotFoundException();
            }
        }

        public MapPolyline CreatePushpinLine(Location startLocation, Location endLocation)
        {
            // Create a MapPolyline object
            var polyline = new MapPolyline();

            // Set the stroke color and thickness
            polyline.Stroke = new SolidColorBrush(Colors.Red);
            polyline.StrokeThickness = 4;

            // Set the stroke dash array to create a dashed line effect
            var dashedArray = new DoubleCollection { 2 };
            polyline.StrokeDashArray = dashedArray;

            // Create a LocationCollection for the polyline's points
            var locations = new LocationCollection();
            locations.Add(startLocation);
            locations.Add(endLocation);

            // Set the polyline's locations
            polyline.Locations = locations;

            return polyline;
        }

        public Pushpin CreatePushpin(
            double latitude, 
            double longitude, 
            string toolTipText, 
            string tag, 
            string? iconPath = null,
            MouseButtonEventHandler? onClick = null)
        {
            Pushpin pushpin = new Pushpin();
            pushpin.Width = 50;
            pushpin.Height = 50;
            pushpin.ToolTip = toolTipText;
            pushpin.Tag = tag;
            pushpin.IsTabStop = false;

            if (onClick != null)
            {
                pushpin.MouseLeftButtonDown += onClick;
            }

            // Set the Location of the Pushpin
            pushpin.Location = new Location(latitude, longitude);

            // Create the custom style for the pushpin
            Style style = new Style(typeof(Pushpin));

            // Create the triggers for mouse enter and leave events
            Trigger mouseEnterTrigger = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
            mouseEnterTrigger.Setters.Add(new Setter(FrameworkElement.RenderTransformProperty, new ScaleTransform(1.2, 1.2)));
            mouseEnterTrigger.Setters.Add(new Setter(Panel.ZIndexProperty, 1));
            mouseEnterTrigger.Setters.Add(new Setter(Control.CursorProperty, Cursors.Hand));

            Trigger mouseLeaveTrigger = new Trigger { Property = UIElement.IsMouseOverProperty, Value = false };
            mouseLeaveTrigger.Setters.Add(new Setter(FrameworkElement.RenderTransformProperty, new ScaleTransform(1, 1)));
            mouseLeaveTrigger.Setters.Add(new Setter(Panel.ZIndexProperty, 0));

            // Add the triggers to the style
            style.Triggers.Add(mouseEnterTrigger);
            style.Triggers.Add(mouseLeaveTrigger);

            if (iconPath != null)
            {
                // Create an Image element as the content of the Pushpin
                Image image = new Image();
                string fullPath = Path.GetFullPath(iconPath); ;
                image.Source = new BitmapImage(new Uri(fullPath));
                // Customize image properties such as Width, Height, etc.

                // Set the custom content template for the pushpin style
                ControlTemplate template = new ControlTemplate(typeof(Pushpin));
                FrameworkElementFactory imageFactory = new FrameworkElementFactory(typeof(Image));
                imageFactory.SetBinding(Image.SourceProperty, new Binding("Source") { Source = image });
                // Customize other properties or add additional elements if needed
                template.VisualTree = imageFactory;

                // Apply the custom content template to the pushpin style
                style.Setters.Add(new Setter(Pushpin.TemplateProperty, template));

            }

            // Apply the style to the pushpin
            pushpin.Style = style;

            return pushpin;
        }

    }
}
