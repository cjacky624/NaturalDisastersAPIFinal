﻿using System;
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
		public List<Earthquakes> EarthquakeList = new List<Earthquakes>();
		public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
			string APIText = "https://earthquake.usgs.gov/fdsnws/" +
				$"event/1/query?format=geojson&starttime=2000-03-04&&minmagnitude=6&";

			HttpWebRequest request = WebRequest.CreateHttp(APIText);
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();

			StreamReader rd = new StreamReader(response.GetResponseStream());
			string data = rd.ReadToEnd();
			rd.Close();

			JToken ColorData = JToken.Parse(data);
			List<JToken> TheColors = ColorData["features"].ToList();


			for (int i = 0; i < 2936; i++)
			{
				Earthquakes e = new Earthquakes();
				e.Color = TheColors[i]["properties"]["alert"].ToString();
				EarthquakeList.Add(e);

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