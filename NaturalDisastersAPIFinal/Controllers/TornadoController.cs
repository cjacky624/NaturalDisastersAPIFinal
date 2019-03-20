using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NaturalDisastersAPIFinal.APIKey;
using Newtonsoft.Json.Linq;
using NaturalDisastersAPIFinal.Models;

namespace NaturalDisastersAPIFinal.Controllers
{
    public class TornadoController : Controller
    {
        
        public ActionResult UserTornado(string Location, DateTime? StartDate = null, DateTime? EndDate = null)
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
				{ StartDate = DateTime.Now;
				}
				if (!EndDate.HasValue)
				{ EndDate = DateTime.Now.AddDays(0.1);
				}//maybe change this later - not certain how we want to calculate risk if they just want to see a specific location.
				 //DateTime date = StartDate;
				TimeSpan userTime = (DateTime)EndDate - (DateTime)StartDate;
				int userMonth = StartDate.Value.Month;  //code to grab the month out of the StartDate - what will we do if the user selects multiple months?

				Session["UserTime"] = userTime;

				return View();
				
        }

		public ActionResult SearchTornados()
		{
				NaturalDisastersEntities db = new NaturalDisastersEntities();
				


				UserLocation User = (UserLocation)Session["UserInfo"];
				

				ViewBag.User = User;

				double FeltLowLongitude = User.Longitude - 2.3;
				double FeltHighLongitutde = User.Longitude + 2.3;
				double FeltLowLatitude = User.Latitude - 2.3;
				double FeltHighLatitude = User.Latitude + 2.3;


				List<UpdatedTornado> userTornados = new List<UpdatedTornado>();
				userTornados = db.UpdatedTornadoes.Where(x => x.Latitude <= FeltHighLatitude && x.Latitude >= FeltLowLatitude && x.Longitude <= FeltHighLongitutde && x.Longitude >= FeltLowLongitude).ToList();

			ViewBag.Results = userTornados;
			double totalNados = userTornados.Count;
			double division = totalNados / 1160;
			double percent = division * 100;
			ViewBag.Chance = Math.Round(percent, 6);
			ViewBag.Count = totalNados;
			return View();
		}
    }
}