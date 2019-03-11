using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NaturalDisastersAPIFinal.Models
{
	public class QuakeData
	{
		public string Place { get; set; }
		public string Alert { get; set; }
		public float LongitudeParsed { get; set; }
		public float LatitudeParsed { get; set; }
	
		public QuakeData()
		{

		}
	
	}
}