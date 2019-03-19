using NaturalDisastersAPIFinal.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using NaturalDisastersAPIFinal.APIKey;
using System;

namespace NaturalDisastersAPIFinal.Controllers
{

    public class HomeController : Controller
    {

        public List<EarthQuakeTable> EarthquakeList = new List<EarthQuakeTable>();


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserLocation(string Location, DateTime? StartDate = null, DateTime? EndDate = null)
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

            if (ParsedLocation.Count == 0)
            {
                ViewBag.TitleError = "Wrong Country";
                ViewBag.Error = "You gave a country out of bounds!";
                return View("Error");
            }
            ViewBag.Address = ParsedLocation[0]["formatted_address"].ToString();
            ViewBag.UserLat = ParsedLocation[0]["geometry"]["location"]["lat"].ToString();
            ViewBag.UserLong = ParsedLocation[0]["geometry"]["location"]["lng"].ToString();

            UserLocation User = new UserLocation();

            string latitude = ParsedLocation[0]["geometry"]["location"]["lat"].ToString();
            float.TryParse(latitude, out float UserLat);
            string longitude = ParsedLocation[0]["geometry"]["location"]["lng"].ToString();
            float.TryParse(longitude, out float UserLong);

          


            User.Location = ParsedLocation[0]["formatted_address"].ToString();
            User.Latitude = UserLat;
            User.Longitude = UserLong;

            Session["UserInfo"] = User;

            if (!StartDate.HasValue)
            { StartDate = DateTime.Now; }
            if (!EndDate.HasValue)
            { EndDate = DateTime.Now.AddDays(0.1); }//maybe change this later - not certain how we want to calculate risk if they just want to see a specific location.
            //DateTime date = StartDate;
            TimeSpan userTime = (DateTime)EndDate - (DateTime)StartDate;
            int userMonth = StartDate.Value.Month;  //code to grab the month out of the StartDate - what will we do if the user selects multiple months?

            Session["UserTime"] = userTime;

            return RedirectToAction("EarthquakeRisk", "Earthquake");
        }


        public ActionResult Earthquakes()
        {
            NaturalDisastersEntities db = new NaturalDisastersEntities();
            ViewBag.Results = db.EarthQuakeTables;
            return View();

        }

        public ActionResult Tornados() //this method is to populate the database table Tornado. we will need a method to search the database based off user input
        {
            int offset;


            //1174 FEMA disasters are Tornados since 1970 
            List<FemaDisaster> Tornados = new List<FemaDisaster>();
            for (offset = 0; offset <= 1000; offset += 1000)
            {
                string APIText = "https://www.fema.gov/api/open/v1/DisasterDeclarationsSummaries?$filter=(incidentType  eq 'Tornado') and incidentBeginDate gt '1970-01-01T00:00:00.000z'&$skip=" + offset;
                //string APIText = "https://www.fema.gov/api/open/v1/DisasterDeclarationsSummaries?$skip=" + offset;

                HttpWebRequest request = WebRequest.CreateHttp(APIText);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                StreamReader rd = new StreamReader(response.GetResponseStream());
                string data = rd.ReadToEnd();
                rd.Close();

                MetaDataWrapper Disasters = JsonConvert.DeserializeObject<MetaDataWrapper>(data);
                Tornados.AddRange(Disasters.DisasterDeclarationsSummaries.ToList());


                //.add only does one object, while .addRange does ALL the objects

            }
            for (int i = 0; i < Tornados.Count(); i++)
            {
                Tornados[i].declaredCountyArea = Tornados[i].declaredCountyArea.Replace("(", "").Replace(")", "");

            }



            NaturalDisastersEntities db = new NaturalDisastersEntities();

            List<Tornado> FematoTornadoObjects = new List<Tornado>();
            foreach (FemaDisaster incident in Tornados)
            {
                Tornado t = new Tornado();
                var latlong = db.Counties.Where(x => x.USPS == incident.state && x.NAME == incident.declaredCountyArea).Select(l => new { l.INTPTLAT, l.INTPTLONG }).FirstOrDefault();
                if (latlong == null)
                {
                    latlong = db.Counties.Where(x => x.USPS == incident.state && x.NAME == incident.declaredCountyArea + " County").Select(l => new { l.INTPTLAT, l.INTPTLONG }).FirstOrDefault();
                }
                if (latlong != null)
                {
                    t.Latitude = latlong.INTPTLAT;
                    t.Longitude = latlong.INTPTLONG;
                }

                t.Alert = incident.title;

                t.Time = Convert.ToDateTime(incident.incidentBeginDate);
                FematoTornadoObjects.Add(t);
            }

            ViewBag.TornadoCount = FematoTornadoObjects.Count();
            ViewBag.TornadoList = FematoTornadoObjects;

            return View();
        }

        public ActionResult OtherFEMADisasters(string Disaster, string StateCode, string UserCounty)//method to call the others in the DisasterDeclarationSummaries API - available options (their count):
             //"Coastal Storm"(474), "Chemical"(9), "Dam/Levee Break" (6), "Drought" (1292), "Fire" (3040), "Fishing Losses" (42), "Flood" (9739), "Freezing" (301), "Hurricane" (10555),
             //"Mud/Landslide" (31), "Severe Ice Storm" (1990), "Severe Storm(s)" (16127), "Snow" (3659), "Tsunami" (9), "Typhoon" (135), "Volcano" (51- 47 of these are from Mt. Helens on 1980)
        {
            //will need to add the comment code below to the UserLocation() to grab this data and find the matching county incidents.
            //string StateCode = ParsedLocation[0]["address_components"][2]["short_name"].ToString();
            //string UserCounty = ParsedLocation[0]["address_components"][1]["short_name"].ToString();


            int offset;

            //there is a total of 49314 disaster declarations as of 3-17-19
            List<FemaDisaster> UniqueDisasters = new List<FemaDisaster>();
            //for loop size will change based on the disaster entered into the OtherFEMADisasters()
            for (offset = 0; offset <= 2000; offset += 1000)
            {
                string APIText = "https://www.fema.gov/api/open/v1/DisasterDeclarationsSummaries?$filter=(incidentType eq '" + Disaster + "') and (incidentBeginDate gt '1970-01-01T00:00:00.000z') and (state eq '"+StateCode+"' and declaredCountyArea eq '"+UserCounty +" County')&$skip=" + offset;
           
                HttpWebRequest request = WebRequest.CreateHttp(APIText);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                StreamReader rd = new StreamReader(response.GetResponseStream());
                string data = rd.ReadToEnd();
                rd.Close();

                MetaDataWrapper Disasters = JsonConvert.DeserializeObject<MetaDataWrapper>(data);
                UniqueDisasters.AddRange(Disasters.DisasterDeclarationsSummaries.ToList());

                //warning county returns null at times we will need to default to the state name for the long and lat

                //.add only does one object, while .addRange does ALL the objects

            }


            ViewBag.Disasters = UniqueDisasters;
            
                return View();
        }
    }
}



//public static List<Earthquake> CallEarthquakeAPI(string APIText)
//{
//	HttpWebRequest request = WebRequest.CreateHttp(APIText);
//	HttpWebResponse response = (HttpWebResponse)request.GetResponse();

//	StreamReader rd = new StreamReader(response.GetResponseStream());
//	string data = rd.ReadToEnd();
//	rd.Close();

//	JToken AllQuakes = JToken.Parse(data);
//	List<JToken> ParsingQuakes = AllQuakes["features"].ToList();


//	List<string> USStates = GetUSStates();
//	List<Earthquake> outputList = new List<Earthquake>();
//	for (int i = 0; i < ParsingQuakes.Count(); i++)
//	{
//		Earthquake e = new Earthquake();


//		string Place = ParsingQuakes[i]["properties"]["place"].ToString();
//		for (int l = 0; l < USStates.Count; l++)
//		{
//			if (Place.Contains(USStates[l].ToString()))
//			{
//				e.Place = Place;
//				string Mag = ParsingQuakes[i]["properties"]["mag"].ToString();
//				decimal.TryParse(Mag, out decimal MagnitudeParsed);
//				e.Magnitude = MagnitudeParsed;
//				string longitude = ParsingQuakes[i]["geometry"]["coordinates"][0].ToString();
//				string latitude = ParsingQuakes[i]["geometry"]["coordinates"][1].ToString();
//				float.TryParse(longitude, out float longParsed);
//				float.TryParse(latitude, out float latParsed);
//				e.Longitude = longParsed;
//				e.Latitude = latParsed;
//				string UnixTime = ParsingQuakes[i]["properties"]["time"].ToString();
//				UnixTime = UnixTime.Substring(0, (UnixTime.Length - 3));
//				double finalUnix = double.Parse(UnixTime);

//				// Format our new DateTime object to start at the UNIX Epoch
//				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(finalUnix);
//				e.Time = dateTime;
//				outputList.Add(e);


//				NaturalDisastersEntities db = new NaturalDisastersEntities();
//				db.Earthquakes.Add(e);
//				db.SaveChanges();
//			}

//		}
//	}

//	return outputList;
//}


//	for (int year = 1970; year <= 2019; year++)
//            {
//                for (int month = 1; month <= 12; month++)
//                {
//                    string monthstring = "0" + month;
//monthstring = monthstring.Substring(monthstring.Length - 2, 2);
//                    string APIText = "https://earthquake.usgs.gov/fdsnws/" +
//						$"event/1/query?format=geojson&starttime=" + year + "-" + monthstring + "-01&endtime=" + year + "-" + monthstring + "-31&minmagnitude=3";
//EarthquakeList.AddRange(CallEarthquakeAPI(APIText));


//                }

//            }

//public static List<string> GetUSStates()
//{
//	return new List<string>(){
//				"Alabama", "Alaska", "Arizona", "Arkansas", "California", "Colorado", "Connecticut", "Delaware",
//				"Florida", "Georgia", "Hawaii", "Idaho", "Illinois", "Indiana", "Iowa", "Kansas", "Kentucky",
//				"Louisiana", "Maine", "Maryland", "Massachusetts", "Michigan", "Minnesota", "Mississippi",
//				"Missouri", "Montana", "Nebraska", "Nevada", "New Hampshire", "New Jersey", "New Mexico",
//				"New York", "North Carolina", "North Dakota", "Ohio", "Oklahoma", "Oregon",  "Pennsylvania",
//				"Rhode Island", "South Carolina", "South Dakota", "Tennessee", "Texas", "Utah", "Vermont",
//				"Virginia", "Washington", "West Virginia", "Wisconsin", "Wyoming"
//			};
//}

