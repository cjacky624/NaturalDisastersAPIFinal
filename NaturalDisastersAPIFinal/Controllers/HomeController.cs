using NaturalDisastersAPIFinal.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NaturalDisastersAPIFinal.APIKey;


namespace NaturalDisastersAPIFinal.Controllers
{
    public class HomeController : Controller
    {

        public List<Earthquake> EarthquakeList = new List<Earthquake>();


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserLocation(string Location)
        {
            MyKey key = new MyKey();
            string APIkey = key.GetKey();
            string APIText = $"https://maps.googleapis.com/maps/api/geocode/json?components=" +
                $"locality:{Location}|country:US&key={APIkey}";
            HttpWebRequest request = WebRequest.CreateHttp(APIText);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader rd = new StreamReader(response.GetResponseStream());
            string data = rd.ReadToEnd();
            rd.Close();

            JToken UserLocation = JToken.Parse(data);
            List<JToken> ParsedLocation = UserLocation["results"].ToList();

            UserLocation User = new UserLocation();            
            
            string latitude = ParsedLocation[0]["geometry"]["location"]["lat"].ToString();
            float.TryParse(latitude, out float UserLat);            
            string longitude = ParsedLocation[0]["geometry"]["location"]["lng"].ToString();
            float.TryParse(longitude, out float UserLong);
            User.Location = ParsedLocation[0]["formatted_address"].ToString();

            User.Latitude = UserLat;
            User.Longitude = UserLong;

            Session["UserInfo"] = User;
            return (RedirectToAction("EarthquakeRisk", "Earthquake"));
           
        }


        public static List<string> GetUSStates()
        {
            return new List<string>(){
                "Alabama", "Alaska", "Arizona", "Arkansas", "California", "Colorado", "Connecticut", "Delaware",
                "Florida", "Georgia", "Hawaii", "Idaho", "Illinois", "Indiana", "Iowa", "Kansas", "Kentucky",
                "Louisiana", "Maine", "Maryland", "Massachusetts", "Michigan", "Minnesota", "Mississippi",
                "Missouri", "Montana", "Nebraska", "Nevada", "New Hampshire", "New Jersey", "New Mexico",
                "New York", "North Carolina", "North Dakota", "Ohio", "Oklahoma", "Oregon",  "Pennsylvania",
                "Rhode Island", "South Carolina", "South Dakota", "Tennessee", "Texas", "Utah", "Vermont",
                "Virginia", "Washington", "West Virginia", "Wisconsin", "Wyoming"
            };
        }




        public ActionResult About()
        {
            for (int year = 1970; year <= 2019; year++)
            {
                for (int month = 1; month <= 12; month++)
                {
                    string monthstring = "0" + month;
                    monthstring = monthstring.Substring(monthstring.Length - 2, 2);
                    string APIText = "https://earthquake.usgs.gov/fdsnws/" +
                        $"event/1/query?format=geojson&starttime=" + year + "-" + monthstring + "-01&endtime=" + year + "-" + monthstring + "-31&minmagnitude=3";
                    EarthquakeList.AddRange(CallEarthquakeAPI(APIText));
                }

            }
            //do multliplication of the Long and Lat for the radius  in the inner for loop. Then add all this data to the database.

            ViewBag.Count = EarthquakeList.Count;
            ViewBag.Results = EarthquakeList;


            return View();
        }

        public static List<Earthquake> CallEarthquakeAPI(string APIText)
        {
            HttpWebRequest request = WebRequest.CreateHttp(APIText);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader rd = new StreamReader(response.GetResponseStream());
            string data = rd.ReadToEnd();
            rd.Close();

            JToken AllQuakes = JToken.Parse(data);
            List<JToken> ParsingQuakes = AllQuakes["features"].ToList();


            List<string> USStates = GetUSStates();
            List<Earthquake> outputList = new List<Earthquake>();
            for (int i = 0; i < ParsingQuakes.Count(); i++)
            {
                Earthquake e = new Earthquake();


                string Place = ParsingQuakes[i]["properties"]["place"].ToString();
                for (int l = 0; l < USStates.Count; l++)
                {
                    if (Place.Contains(USStates[l].ToString()))
                    {
                        e.Place = Place;
                        string Mag = ParsingQuakes[i]["properties"]["mag"].ToString();
                        decimal.TryParse(Mag, out decimal MagnitudeParsed);
                        e.Magnitude = MagnitudeParsed;
                        string longitude = ParsingQuakes[i]["geometry"]["coordinates"][0].ToString();
                        string latitude = ParsingQuakes[i]["geometry"]["coordinates"][1].ToString();
                        float.TryParse(longitude, out float longParsed);
                        float.TryParse(latitude, out float latParsed);
                        e.Longitude = longParsed;
                        e.Latitude = latParsed;
                        string UnixTime = ParsingQuakes[i]["properties"]["time"].ToString();
                        UnixTime = UnixTime.Substring(0, (UnixTime.Length - 3));
                        double finalUnix = double.Parse(UnixTime);

                        // Format our new DateTime object to start at the UNIX Epoch
                        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(finalUnix);
                        e.Time = dateTime;
                        outputList.Add(e);
                    }

                }
            }

            return outputList;
        }


        public ActionResult Contact()
        {
            int offset;

            //there is a total of 4105 disaster declarations as of 3-11-19
            List<FemaDisaster> Tornados = new List<FemaDisaster>();
            for (offset = 0; offset <= 4000; offset += 1000)
            {
                string APIText = "https://www.fema.gov/api/open/v1/FemaWebDisasterDeclarations?$skip=" + offset;

                HttpWebRequest request = WebRequest.CreateHttp(APIText);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                StreamReader rd = new StreamReader(response.GetResponseStream());
                string data = rd.ReadToEnd();
                rd.Close();

                MetaDataWrapper Disasters = JsonConvert.DeserializeObject<MetaDataWrapper>(data);
                Tornados.AddRange(Disasters.FemaWebDisasterDeclarations.Where(x => x.incidentType == "Tornado").ToList());

                //.add only does one object, while .addRange does ALL the objects
                //to populate the table Tornado, we will need to grab each item in the List and pull out the strings, etc that make up the Object Tornado and save to the DB
            }

            ViewBag.TornadoCount = Tornados.Count();
            ViewBag.TornadoList = Tornados;
            return View();
        }
        //we will need a sperate method for the other weathers.
        public static void AddToEQDatabase(List<Earthquake> EarthquakeList)
        {
            


            foreach (var q in EarthquakeList)
            {
                double DmgLowLongititude;
                double DmgHighLongititude;
                double DmgLowLatitude;
                double DmgHighLatitude;
                double FeltLowLongititude;
                double FeltHighLongitutde;
                double FeltLowLatitude;
                double FeltHighLatitude;

                //if (q.Magnitude >= 3 && q.Magnitude <= 4.00)
                //{
                //    DmgLowLongititude = q.Longitude * 0.998;
                //    DmgHighLongititude = q.Longitude * 1.002;
                //    DmgLowLatitude = q.Latitude * 0.998;
                //    DmgHighLatitude = q.Latitude * 1.002;
                //    FeltLowLongititude = q.Longitude - 0.9;
                //    FeltHighLongitutde = q.Longitude + 0.9;
                //    FeltLowLatitude = q.Latitude - 0.9;
                //    FeltHighLatitude = q.Latitude + 0.9;
                //}
                //else if (q.Magnitude > 4 && q.Magnitude <= 6.00)
                //{
                //    DmgLowLongititude = q.Longitude * 0.992;
                //    DmgHighLongititude = q.Longitude * 1.008;
                //    DmgLowLatitude = q.Latitude * 0.992;
                //    DmgHighLatitude = q.Latitude * 1.008;
                //    FeltLowLongititude = q.Longitude - 1.2;
                //    FeltHighLongitutde = q.Longitude + 1.2;
                //    FeltLowLatitude = q.Latitude - 1.2;
                //    FeltHighLatitude = q.Latitude + 1.2;
                //}
                
            }
            
        }
    }
}
