using NaturalDisastersAPIFinal.APIKey;
using NaturalDisastersAPIFinal.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NaturalDisastersAPIFinal.Controllers
{
    public class EarthquakeController : Controller
    {
        // GET: Earthquake
        public ActionResult EarthquakeRisk()
        {
            NaturalDisastersEntities db = new NaturalDisastersEntities();

            UserLocation User = (UserLocation)Session["UserInfo"];
            TimeSpan userTime = (TimeSpan)Session["UserTime"];



            ViewBag.User = User;


            double FeltLowLongitude = User.Longitude - 5;
            double FeltHighLongitude = User.Longitude + 5;
            double FeltLowLatitude = User.Latitude - 5;
            double FeltHighLatitude = User.Latitude + 5;



            List<EarthQuakeTable> userEarthquakes = new List<EarthQuakeTable>();

            foreach (EarthQuakeTable Eq in db.EarthQuakeTables)
            {

                if(Eq.Latitude>=FeltLowLatitude && Eq.Latitude<=FeltHighLatitude &&
                    Eq.Longitude >= FeltLowLongitude && Eq.Longitude <= FeltHighLongitude)

                {

                    if (Eq.Magnitude > 6 && Eq.Magnitude <= 8)
                    {
                        FeltLowLongitude = User.Longitude - 2.5;

                        FeltHighLongitude = User.Longitude + 2.5;

                        FeltLowLatitude = User.Latitude - 2.5;
                        FeltHighLatitude = User.Latitude + 2.5;
                        if (Eq.Latitude >= FeltLowLatitude && Eq.Latitude <= FeltHighLatitude &&
                            Eq.Longitude >= FeltLowLongitude && Eq.Longitude <= FeltHighLongitude)
                        {
                            userEarthquakes.Add(Eq);
                        }
                    }
                    else if (Eq.Magnitude <= 6 && Eq.Magnitude > 4)
                    {

                        FeltLowLongitude = User.Longitude - 1.5;
                        FeltHighLongitude = User.Longitude + 1.5;
                        FeltLowLatitude = User.Latitude - 1.5;
                        FeltHighLatitude = User.Latitude + 1.5;

                        if (Eq.Latitude >= FeltLowLatitude && Eq.Latitude <= FeltHighLatitude &&
                            Eq.Longitude >= FeltLowLongitude && Eq.Longitude <= FeltHighLongitude)
                        {
                            userEarthquakes.Add(Eq);
                        }
                    }
                    else if (Eq.Magnitude <= 4)
                    {
                        FeltLowLongitude = User.Longitude - 1.2;

                        FeltHighLongitude = User.Longitude + 1.2;

                        FeltLowLatitude = User.Latitude - 1.2;
                        FeltHighLatitude = User.Latitude + 1.2;
                        if (Eq.Latitude >= FeltLowLatitude && Eq.Latitude <= FeltHighLatitude &&
                            Eq.Longitude >= FeltLowLongitude && Eq.Longitude <= FeltHighLongitude)
                        {
                            userEarthquakes.Add(Eq);
                        }
                    }
                    else
                    {
                        userEarthquakes.Add(Eq);
                    }
                }
            }
            ViewBag.Results = userEarthquakes;

            double totalQuakes = userEarthquakes.Count;
            double allQuakesUS = db.EarthQuakeTables.Count();
            double division = totalQuakes / allQuakesUS;
            double percent = division * 100;
            ViewBag.Chance = Math.Round(percent, 6);
            ViewBag.Count = totalQuakes;
		

			return View();

        }
        public ActionResult SpeedUpSearch()
        {
            NaturalDisastersEntities db = new NaturalDisastersEntities();

            UserLocation User = (UserLocation)Session["UserInfo"];

            TimeSpan userTime = (TimeSpan)Session["UserTime"];

            ViewBag.User = User;

            double FeltLowLongitude = User.Longitude - 5.0;
            double FeltHighLongitutde = User.Longitude + 5.0;
            double FeltLowLatitude = User.Latitude - 5.0;
            double FeltHighLatitude = User.Latitude + 5.0;



            List<EarthQuakeTable> userEarthquakes = new List<EarthQuakeTable>();
            userEarthquakes = db.EarthQuakeTables.Where(x => x.Magnitude > 8 && x.Latitude <= FeltHighLatitude && x.Latitude >= FeltLowLatitude && x.Longitude <= FeltHighLongitutde && x.Longitude >= FeltLowLongitude).ToList();

            FeltLowLongitude = User.Longitude - 2.5;
            FeltHighLongitutde = User.Longitude + 2.5;
            FeltLowLatitude = User.Latitude - 2.5;
            FeltHighLatitude = User.Latitude + 2.5;
            userEarthquakes.AddRange(db.EarthQuakeTables.Where(x => x.Magnitude > 6 && x.Magnitude <= 8 &&
                                x.Latitude <= FeltHighLatitude && x.Latitude >= FeltLowLatitude && x.Longitude <= FeltHighLongitutde && x.Longitude >= FeltLowLongitude).ToList());

            FeltLowLongitude = User.Longitude - 1.5;
            FeltHighLongitutde = User.Longitude + 1.5;
            FeltLowLatitude = User.Latitude - 1.5;
            FeltHighLatitude = User.Latitude + 1.5;
            userEarthquakes.AddRange(db.EarthQuakeTables.Where(x => x.Magnitude > 4 && x.Magnitude <= 6 &&
                                x.Latitude <= FeltHighLatitude && x.Latitude >= FeltLowLatitude && x.Longitude <= FeltHighLongitutde && x.Longitude >= FeltLowLongitude).ToList());


            FeltLowLongitude = User.Longitude - 1.2;
            FeltHighLongitutde = User.Longitude + 1.2;
            FeltLowLatitude = User.Latitude - 1.2;
            FeltHighLatitude = User.Latitude + 1.2;
            userEarthquakes.AddRange(db.EarthQuakeTables.Where(x => x.Magnitude <= 4 &&
                                x.Latitude <= FeltHighLatitude && x.Latitude >= FeltLowLatitude && x.Longitude <= FeltHighLongitutde && x.Longitude >= FeltLowLongitude).ToList());




            ViewBag.Results = userEarthquakes;
            double totalQuakes = userEarthquakes.Count;
            double allQuakesUS = 83944;
            double division = totalQuakes / allQuakesUS;
            double percent = division * 100;
            ViewBag.Chance = Math.Round(percent, 6);//felt earthquakes only
            List<EarthQuakeTable> dmgRisk = EarthquakeDamageRisk();
            double dmgPercent = Math.Round((100 * (dmgRisk.Count() / totalQuakes)), 6);
            ViewBag.dmgPercent = dmgPercent;
            ViewBag.Count = totalQuakes;
			double timesSafer = 3370 / totalQuakes;
			ViewBag.Safer = Math.Round(timesSafer, 2);
			double timesDanger = totalQuakes / 2;
			ViewBag.Danger = Math.Round(timesDanger, 2);

			Dictionary<string, double> stats = MonthStats(userEarthquakes, totalQuakes);
            ViewBag.MonthSafety = stats;


			return View();
        }

        public ActionResult Redirecting()
        {
            return View();
        }
        public List<EarthQuakeTable> EarthquakeDamageRisk()
        {
            NaturalDisastersEntities db = new NaturalDisastersEntities();
            UserLocation User = (UserLocation)Session["UserInfo"];
            ViewBag.User = User;
            double DmgLowLongitude = User.Longitude - 1.5;
            double DmgHighLongitude = User.Longitude + 1.5;
            double DmgLowLatitude = User.Latitude - 1.5;
            double DmgHighLatitude = User.Latitude + 1.5;
            List<EarthQuakeTable> userEarthquakesDmg = new List<EarthQuakeTable>();


            userEarthquakesDmg = db.EarthQuakeTables.Where(x => x.Magnitude > 8 && x.Latitude <= DmgHighLatitude && x.Latitude >= DmgLowLatitude
                                                        && x.Longitude <= DmgHighLongitude && x.Longitude >= DmgLowLongitude).ToList();


            DmgLowLongitude = User.Longitude - 0.25;
            DmgHighLongitude = User.Longitude + 0.25;
            DmgLowLatitude = User.Latitude - 0.25;
            DmgHighLatitude = User.Latitude + 0.25;
            userEarthquakesDmg.AddRange(db.EarthQuakeTables.Where(x => x.Magnitude > 4 && x.Magnitude <= 6 &&
                                                       x.Latitude <= DmgHighLatitude && x.Latitude >= DmgLowLatitude && x.Longitude <= DmgHighLongitude && x.Longitude >= DmgLowLongitude).ToList());

            DmgLowLongitude = User.Longitude - 1;
            DmgHighLongitude = User.Longitude + 1;
            DmgLowLatitude = User.Latitude - 1;
            DmgHighLatitude = User.Latitude + 1;
            userEarthquakesDmg.AddRange(db.EarthQuakeTables.Where(x => x.Magnitude > 6 && x.Magnitude <= 8 &&
                                                       x.Latitude <= DmgHighLatitude && x.Latitude >= DmgLowLatitude && x.Longitude <= DmgHighLongitude && x.Longitude >= DmgLowLongitude).ToList());
            List<EarthQuakeTable> dmgRisk = new List<EarthQuakeTable>();
            dmgRisk.AddRange(userEarthquakesDmg.Where(y => y.Magnitude >= 5));

            return dmgRisk;
        }


        public Dictionary<string, double> MonthStats(List<EarthQuakeTable> userEvents, double totalEvents)
        {

            var stats = (userEvents.GroupBy(o => new { Month = o.Time.Value.Month })
                                                    .Select(b => new { Month = b.Key.Month, Total = b.Count() }))
                                                    .OrderBy(c => c.Month)
                                                    .ToList();
            Dictionary<string, double> output = new Dictionary<string, double>();
            foreach (var month in stats)
            {
                string MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month.Month);
                double Percentage = Math.Round(((month.Total / totalEvents) * 100), 2);
                output.Add(MonthName, Percentage);
            }

            return output;
        }



		public ActionResult UserQuakes(string Location, DateTime? StartDate = null, DateTime? EndDate = null)
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
			{
				StartDate = DateTime.Now;
			}
			if (!EndDate.HasValue)
			{
				EndDate = DateTime.Now.AddDays(0.1);
			}//maybe change this later - not certain how we want to calculate risk if they just want to see a specific location.
			 //DateTime date = StartDate;
			TimeSpan userTime = (DateTime)EndDate - (DateTime)StartDate;
			int userMonth = StartDate.Value.Month;  //code to grab the month out of the StartDate - what will we do if the user selects multiple months?

			Session["UserTime"] = userTime;
			return View();
		}
   }
}

