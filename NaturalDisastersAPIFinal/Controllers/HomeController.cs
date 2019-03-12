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

namespace NaturalDisastersAPIFinal.Controllers
{
    public class HomeController : Controller
    {

        public List<Earthquakes> EarthquakeList = new List<Earthquakes>();

        public List<string> USStates = new List<string>()
        {
            "Alabama", "Alaska", "Arizona", "Arkansas", "California", "Colorado", "Connecticut", "Delaware",
            "Florida", "Georgia", "Hawaii", "Idaho", "Illinois", "Indiana", "Iowa", "Kansas", "Kentucky",
            "Louisiana", "Maine", "Maryland", "Massachusetts", "Michigan", "Minnesota", "Mississippi",
            "Missouri", "Montana", "Nebraska", "Nevada", "New Hampshire", "New Jersey", "New Mexico",
            "New York", "North Carolina", "North Dakota", "Ohio", "Oklahoma", "Oregon",  "Pennsylvania",
            "Rhode Island", "South Carolina", "South Dakota", "Tennessee", "Texas", "Utah", "Vermont",
            "Virginia", "Washington", "West Virginia", "Wisconsin", "Wyoming"
        };

        public List<QuakeData> AllLocations = new List<QuakeData>();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            string APIText = "https://earthquake.usgs.gov/fdsnws/" +
                $"event/1/query?format=geojson&starttime=1970-01-01&&minmagnitude=6&";

            HttpWebRequest request = WebRequest.CreateHttp(APIText);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader rd = new StreamReader(response.GetResponseStream());
            string data = rd.ReadToEnd();
            rd.Close();

            JToken AllQuakes = JToken.Parse(data);
            List<JToken> ParsingQuakes = AllQuakes["features"].ToList();

            for (int i = 0; i < ParsingQuakes.Count(); i++)
            {
                QuakeData q = new QuakeData();

                q.Alert = ParsingQuakes[i]["properties"]["alert"].ToString();
                q.Place = ParsingQuakes[i]["properties"]["place"].ToString();
                string longitude = ParsingQuakes[i]["geometry"]["coordinates"][0].ToString();
                string latitude = ParsingQuakes[i]["geometry"]["coordinates"][1].ToString();
                float.TryParse(longitude, out float longParsed);
                float.TryParse(latitude, out float latParsed);
                q.LongitudeParsed = longParsed;
                q.LatitudeParsed = latParsed;
                AllLocations.Add(q);
                string UnixTime = ParsingQuakes[i]["properties"]["time"].ToString();

                string finalUnix = UnixTime;
                if (EarthquakeList.Count < 137)
                {
                    finalUnix = UnixTime.Substring(0, 10);
                }

                if (EarthquakeList.Count >= 137)
                {
                    finalUnix = UnixTime.Substring(0, 9);
                }
                if (EarthquakeList.Count >= 340)
                {
                    finalUnix = UnixTime.Substring(0, 8);
                }
                if (EarthquakeList.Count >= 363)
                {
                    finalUnix = UnixTime.Substring(0, 7);
                }
                ulong.TryParse(finalUnix, out ulong UnixInLong);


                // Format our new DateTime object to start at the UNIX Epoch
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

                // Add the timestamp (number of seconds since the Epoch) to be converted
                dateTime = dateTime.AddSeconds(UnixInLong);
                                                          

                for (int l = 0; l < USStates.Count; l++)
                {
                    if (q.Place.Contains(USStates[l].ToString()))
                    {

                        Earthquakes e = new Earthquakes();
                        e.Place = q.Place;
                        e.Alert = q.Alert;
                        e.Longitude = q.LongitudeParsed;
                        e.Latitude = q.LatitudeParsed;
                        e.Time = dateTime;
                        EarthquakeList.Add(e);
                    }
                }


            }
            ViewBag.Results = EarthquakeList;


            return View();
        }
                                 


        //aka the Tornado  data
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
    }
}







