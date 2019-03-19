using NaturalDisastersAPIFinal.APIKey;
using NaturalDisastersAPIFinal.Models;
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
    public class EarthquakeController : Controller
    {
        // GET: Earthquake
        public ActionResult EarthquakeRisk()
        {
            NaturalDisastersEntities db = new NaturalDisastersEntities();
            
            UserLocation User = (UserLocation)Session["UserInfo"];
            TimeSpan userTime = (TimeSpan)Session["UserTime"];

            ViewBag.User = User;

            double FeltLowLongitude = User.Longitude - 2.3;
            double FeltHighLongitutde = User.Longitude + 2.3;
            double FeltLowLatitude = User.Latitude - 2.3;
            double FeltHighLatitude = User.Latitude + 2.3;


            List<EarthQuakeTable> userEarthquakes = new List<EarthQuakeTable>();
            
            foreach(EarthQuakeTable Eq in db.EarthQuakeTables)
            {
                if (Eq.Latitude>=FeltLowLatitude && Eq.Latitude<=FeltHighLatitude &&
                    Eq.Longitude >= FeltLowLongitude && Eq.Longitude <= FeltHighLongitutde)
                {
                    
                    if (Eq.Magnitude > 6 && Eq.Magnitude <= 8)
                    {
                        FeltLowLongitude = User.Longitude - 1.5;
                        FeltHighLongitutde = User.Longitude + 1.5;
                        FeltLowLatitude = User.Latitude - 1.5;
                        FeltHighLatitude = User.Latitude + 1.5;
                        if (Eq.Latitude >= FeltLowLatitude && Eq.Latitude <= FeltHighLatitude &&
                            Eq.Longitude >= FeltLowLongitude && Eq.Longitude <= FeltHighLongitutde)
                        {
                            userEarthquakes.Add(Eq);
                        }
                    }
                    else if(Eq.Magnitude <= 6 && Eq.Magnitude > 4)
                    {
                        FeltLowLongitude = User.Longitude - 1.2;
                        FeltHighLongitutde = User.Longitude + 1.2;
                        FeltLowLatitude = User.Latitude - 1.2;
                        FeltHighLatitude = User.Latitude + 1.2;
                        if (Eq.Latitude >= FeltLowLatitude && Eq.Latitude <= FeltHighLatitude &&
                            Eq.Longitude >= FeltLowLongitude && Eq.Longitude <= FeltHighLongitutde)
                        {
                            userEarthquakes.Add(Eq);
                        }
                    }
                    else if(Eq.Magnitude <= 4)
                    {
                        FeltLowLongitude = User.Longitude - 0.9;
                        FeltHighLongitutde = User.Longitude + 0.9;
                        FeltLowLatitude = User.Latitude - 0.9;
                        FeltHighLatitude = User.Latitude + 0.9;
                        if (Eq.Latitude >= FeltLowLatitude && Eq.Latitude <= FeltHighLatitude &&
                            Eq.Longitude >= FeltLowLongitude && Eq.Longitude <= FeltHighLongitutde)
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
            //if (q.Magnitude >= 3 && q.Magnitude <= 4.00)
            //{
            //    DmgLowLongititude = q.Longitude * 0.998;
            //    DmgHighLongititude = q.Longitude * 1.002;
            //    DmgLowLatitude = q.Latitude * 0.998;
            //    DmgHighLatitude = q.Latitude * 1.002;
        

            //else if (q.Magnitude > 4 && q.Magnitude <= 6.00)
            //{
            //    DmgLowLongititude = q.Longitude * 0.992;
            //    DmgHighLongititude = q.Longitude * 1.008;
            //    DmgLowLatitude = q.Latitude * 0.992;
            //    DmgHighLatitude = q.Latitude * 1.008;
            

            return View();
        }


        public ActionResult SpeedUpSearch()
        {
            NaturalDisastersEntities db = new NaturalDisastersEntities();

           

			UserLocation User = (UserLocation)Session["UserInfo"];
			TimeSpan userTime = (TimeSpan)Session["UserTime"];

            ViewBag.User = User;
            
            double FeltLowLongitude = User.Longitude - 2.3;
            double FeltHighLongitutde = User.Longitude + 2.3;
            double FeltLowLatitude = User.Latitude - 2.3;
            double FeltHighLatitude = User.Latitude + 2.3;


            List<EarthQuakeTable> userEarthquakes = new List<EarthQuakeTable>();
            userEarthquakes = db.EarthQuakeTables.Where(x => x.Magnitude>8 && x.Latitude <= FeltHighLatitude && x.Latitude >= FeltLowLatitude && x.Longitude <= FeltHighLongitutde && x.Longitude>=FeltLowLongitude).ToList();

            FeltLowLongitude = User.Longitude - 1.5;
            FeltHighLongitutde = User.Longitude + 1.5;
            FeltLowLatitude = User.Latitude - 1.5;
            FeltHighLatitude = User.Latitude + 1.5;
            userEarthquakes.AddRange(db.EarthQuakeTables.Where(x => x.Magnitude > 6 && x.Magnitude<=8 && 
                                x.Latitude <= FeltHighLatitude && x.Latitude >= FeltLowLatitude && x.Longitude <= FeltHighLongitutde && x.Longitude >= FeltLowLongitude).ToList());

            FeltLowLongitude = User.Longitude - 1.2;
            FeltHighLongitutde = User.Longitude + 1.2;
            FeltLowLatitude = User.Latitude - 1.2;
            FeltHighLatitude = User.Latitude + 1.2;
            userEarthquakes.AddRange(db.EarthQuakeTables.Where(x => x.Magnitude > 4 && x.Magnitude <= 6 && 
                                x.Latitude <= FeltHighLatitude && x.Latitude >= FeltLowLatitude && x.Longitude <= FeltHighLongitutde && x.Longitude >= FeltLowLongitude).ToList());


            FeltLowLongitude = User.Longitude - 0.9;
            FeltHighLongitutde = User.Longitude + 0.9;
            FeltLowLatitude = User.Latitude - 0.9;
            FeltHighLatitude = User.Latitude + 0.9;
            userEarthquakes.AddRange(db.EarthQuakeTables.Where(x => x.Magnitude <= 4 &&
                                x.Latitude <= FeltHighLatitude && x.Latitude >= FeltLowLatitude && x.Longitude <= FeltHighLongitutde && x.Longitude >= FeltLowLongitude).ToList());



         
            ViewBag.Results = userEarthquakes;
            double totalQuakes = userEarthquakes.Count;
            double allQuakesUS = 83944;
            double division = totalQuakes / allQuakesUS;
            double percent = division * 100;
            ViewBag.Chance = Math.Round(percent, 6);
            ViewBag.Count = totalQuakes;
            return View();
        }

		public ActionResult Redirecting()
	  	{
        return View();
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