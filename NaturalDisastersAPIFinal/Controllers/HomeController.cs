using NaturalDisastersAPIFinal.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using NaturalDisastersAPIFinal.APIKey;


namespace NaturalDisastersAPIFinal.Controllers
{
	public class HomeController : Controller
	{
		
		public List<EarthQuakeTable> EarthquakeList = new List<EarthQuakeTable>();


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
			return View();
		}
	

        public ActionResult Earthquakes()
        {
			NaturalDisastersEntities db = new NaturalDisastersEntities();
			ViewBag.Results = db.EarthQuakeTables;
			ViewBag.Count = db.EarthQuakeTables.Count();
			return View();
        }

	 public ActionResult Tornados()
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

