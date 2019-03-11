using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NaturalDisastersAPIFinal.Models;
using Newtonsoft.Json.Linq;

namespace NaturalDisastersAPIFinal.Controllers
{
	public class HomeController : Controller
	{
		//Lists
		public List<Earthquakes> EarthquakeList = new List<Earthquakes>();
		//Lists
		public List<string> USStates = new List<string>()
		{
			"Alabama", "Alaska", "Arizona", "Arkansas", "California", "Colorado", "Connecticut", "Delaware",
			"Florida", "Georgia", "Hawaii", "Idaho", "Illinois", "Indiana", "Iowa", "Kansas", "Kentucky",
			"Louisiana", "Maine", "Maryland", "Massachusetts", "Michigan", "Minnesota", "Mississippi",
			"Missouri", "Montana", "Nebraska", "Nevada", "New Hampshire", "New Jersey", "New Mexico",
			"New York", "North Carolina", "North Dakota", "Ohio", "Oklahoma", "Oregon",  "Pennsylvania",
			"Rhode Island", "South Carolina", "South Dakota", "Tennessee", "Texas", "Utah", "Vermont",
			"Virginia", "Washington", "West Virginia", "Wisconsin", "Wyoming", "off the coast of Oregon"
		};

		public List<QuakeData> AllLocations = new List<QuakeData>();

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult About()
		{
			string APIText = "https://earthquake.usgs.gov/fdsnws/" +
				$"event/1/query?format=geojson&starttime=1930-01-01&&minmagnitude=6&";

			HttpWebRequest request = WebRequest.CreateHttp(APIText);
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();

			StreamReader rd = new StreamReader(response.GetResponseStream());
			string data = rd.ReadToEnd();
			rd.Close();

			JToken ColorData = JToken.Parse(data);
			List<JToken> TheColors = ColorData["features"].ToList();

			for (int i = 0; i < TheColors.Count(); i++)
			{
				QuakeData q = new QuakeData();

				q.Alert = TheColors[i]["properties"]["alert"].ToString();
				q.Place = TheColors[i]["properties"]["place"].ToString();
				AllLocations.Add(q);


				for (int l = 0; l < USStates.Count; l++)
				{
					if (q.Place.Contains(USStates[l].ToString()))
					{

						Earthquakes e = new Earthquakes();
						e.Color = TheColors[i]["properties"]["place"].ToString();
						EarthquakeList.Add(e);
					}
				}


			}
			ViewBag.Results = EarthquakeList;

			return View();
		}










		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}








//if (TheColors[i]["properties"]["place"].ToString().Contains(JustAlaska.ToString()))
//				{
//					Earthquakes e = new Earthquakes();
//e.Color = TheColors[i]["properties"]["alert"].ToString();
//EarthquakeList.Add(e);
//				}