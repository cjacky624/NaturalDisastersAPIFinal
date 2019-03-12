using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using NaturalDisastersAPIFinal.APIKey;
using Newtonsoft.Json.Linq;

namespace NaturalDisastersAPIFinal.Models
{

	public class EarthquakeDAL
	{
		public List<Earthquakes> EarthquakeList = new List<Earthquakes>();
		public void GetData()
		{
			string APIText = "https://earthquake.usgs.gov/fdsnws/" +
				$"event/1/query?format=geojson&starttime=2000-03-04&&minmagnitude=6&";
			
			HttpWebRequest request = WebRequest.CreateHttp(APIText);
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();

			StreamReader rd = new StreamReader(response.GetResponseStream());
			string data = rd.ReadToEnd();
			rd.Close();
			
			JToken ColorData = JToken.Parse(APIText);
			List<JToken> TheColors = ColorData["features"].ToList();

			

		}
	}

}