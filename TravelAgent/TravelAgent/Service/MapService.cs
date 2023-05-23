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

        public async Task<string> ReverseGeocode(double latitude, double longitude)
        {
            string apiEndpoint = $"REST/v1/Locations/{latitude},{longitude}?key={_consts.BingMapsApiKey}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiEndpoint);

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            string responseBodyStringified = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                JObject jsonObject = JObject.Parse(responseBodyStringified);
                string name = jsonObject["resourceSets"][0]["resources"][0]["name"].ToString();

                return name;
            }

            // TODO: Make an actual error response exception
            throw new DatabaseResponseException("Placeholder exception");

        }
        
        public async Task<double[]> Geocode(string address)
        {
            string apiEndpoint = $"REST/v1/Locations?q={address}&key={_consts.BingMapsApiKey}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiEndpoint);

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            string responseBodyStringified = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                JObject json = JObject.Parse(responseBodyStringified);

                JArray geocodePoints = json["resourceSets"][0]["resources"][0]["geocodePoints"] as JArray;
                if (geocodePoints != null && geocodePoints.Count > 0)
                {
                    JArray coordinates = geocodePoints[0]["coordinates"] as JArray;
                    if (coordinates != null && coordinates.Count == 2)
                    {
                        double latitude = (double)coordinates[0];
                        double longitude = (double)coordinates[1];

                        return new double[] { latitude, longitude };
                    }
                    else
                    {
                        // TODO: Make an actual error response exception
                        throw new DatabaseResponseException("Placeholder exception");
                    }
                }
                else
                {
                    // TODO: Make an actual error response exception
                    throw new DatabaseResponseException("Placeholder exception");
                }
            }

            // TODO: Make an actual error response exception
            throw new DatabaseResponseException("Placeholder exception");

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
            // Create a new Pushpin
            Pushpin pushpin = new Pushpin();
            pushpin.Width = 50;
            pushpin.Height = 50;
            pushpin.ToolTip = toolTipText;
            pushpin.Tag = tag;

            if (onClick != null )
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

            // Apply the style to the pushpin
            pushpin.Style = style;

            return pushpin;
        }

    }
}
