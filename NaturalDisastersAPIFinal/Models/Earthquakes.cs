using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NaturalDisastersAPIFinal.Models
{

	public class Earthquakes
	{
		public string Magnitude { get; set; }
		public string Place { get; set; }
		public float Latitude { get; set; }
		public float Longitude { get; set; }
		public DateTime Time { get; set; }
	}
    

}