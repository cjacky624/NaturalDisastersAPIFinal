using NaturalDisastersAPIFinal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

            ViewBag.User = User;

            double FeltLowLongitude = User.Longitude - 5;
            double FeltHighLongitude = User.Longitude + 5;
            double FeltLowLatitude = User.Latitude - 5;
            double FeltHighLatitude = User.Latitude + 5;


            List<Earthquake> userEarthquakes = new List<Earthquake>();
            foreach(Earthquake Eq in db.Earthquakes)
            {
                if(Eq.Latitude>=FeltLowLatitude && Eq.Latitude<=FeltHighLatitude &&
                    Eq.Longitude >= FeltLowLongitude && Eq.Longitude <= FeltHighLongitude)
                {
                    if(Eq.Magnitude > 6 && Eq.Magnitude <= 8)
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
                    else if(Eq.Magnitude <= 6 && Eq.Magnitude > 4)
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
                    else if(Eq.Magnitude <= 4)
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
			double allQuakesUS = db.Earthquakes.Count();
			double division = totalQuakes / allQuakesUS;
			double percent = division * 100;
			ViewBag.Chance = Math.Round(percent, 6);
			ViewBag.Count = totalQuakes;

                return View();
        }

        public ActionResult EarthquakeDamageRisk()
        {
            NaturalDisastersEntities db = new NaturalDisastersEntities();
            UserLocation User = (UserLocation)Session["UserInfo"];
            ViewBag.User = User;

            double DmgLowLongitude = User.Longitude - 5;
            double DmgHighLongitude = User.Longitude + 5;
            double DmgLowLatitude = User.Latitude - 5;
            double DmgHighLatitude = User.Latitude + 5;

            List<Earthquake> userEarthquakes = new List<Earthquake>();
            foreach (Earthquake Eq in db.Earthquakes)
            {

                if (Eq.Latitude >= DmgLowLatitude && Eq.Latitude <= DmgHighLatitude &&
                    Eq.Longitude >= DmgLowLongitude && Eq.Longitude <= DmgHighLongitude)
                {
                    if (Eq.Magnitude >= 3 && User.Magnitude <= 4.00)
                    {
                        DmgLowLongitude = User.Longitude - 0.1;
                        DmgHighLongitude = User.Longitude + 0.1;
                        DmgLowLatitude = User.Latitude - 0.1;
                        DmgHighLatitude = User.Latitude + 0.1;
                    }

                    else if (Eq.Magnitude > 4 && Eq.Magnitude <= 6.00)
                    {
                        DmgLowLongitude = User.Longitude - 0.25;
                        DmgHighLongitude = User.Longitude + 0.25;
                        DmgLowLatitude = User.Latitude - 0.25;
                        DmgHighLatitude = User.Latitude + 0.25;
                    }
                    else if (Eq.Magnitude > 6.01 && Eq.Magnitude <= 8.00)
                    {
                        DmgLowLongitude = User.Longitude - 2.5;
                        DmgHighLongitude = User.Longitude + 2.5;
                        DmgLowLatitude = User.Latitude - 2.5;
                        DmgHighLatitude = User.Latitude + 2.5;
                    }
                }
            }
            return View();
        }
    }
}


