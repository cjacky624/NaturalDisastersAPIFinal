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
            TimeSpan userTime = (TimeSpan)Session["UserTime"];

            ViewBag.User = User;

            double FeltLowLongitude = User.Longitude - 5;
            double FeltHighLongitude = User.Longitude + 5;
            double FeltLowLatitude = User.Latitude - 5;
            double FeltHighLatitude = User.Latitude + 5;


            List<EarthQuakeTable> userEarthquakes = new List<EarthQuakeTable>();
            
            foreach(EarthQuakeTable Eq in db.EarthQuakeTables)
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

            double FeltLowLongitude = User.Longitude - 5;
            double FeltHighLongitutde = User.Longitude + 5;
            double FeltLowLatitude = User.Latitude - 5;
            double FeltHighLatitude = User.Latitude + 5;


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
            ViewBag.Chance = Math.Round(percent, 6);
            ViewBag.Count = totalQuakes;
            return View();
        }

        public ActionResult Redirecting()
        {
            return View();
        }
        public ActionResult EarthquakeDamageRisk()
        {
            NaturalDisastersEntities db = new NaturalDisastersEntities();
            UserLocation User = (UserLocation)Session["UserInfo"];
            ViewBag.User = User;

            double DmgLowLongitude = User.Longitude - 1.5;
            double DmgHighLongitude = User.Longitude + 1.5;
            double DmgLowLatitude = User.Latitude - 1.5;
            double DmgHighLatitude = User.Latitude + 1.5;

            List<EarthQuakeTable> userEarthquakesDmg = new List<EarthQuakeTable>();
            foreach (EarthQuakeTable EqDmg in db.EarthQuakeTables)
            {

                if (EqDmg.Latitude >= DmgLowLatitude && EqDmg.Latitude <= DmgHighLatitude &&
                    EqDmg.Longitude >= DmgLowLongitude && EqDmg.Longitude <= DmgHighLongitude)
                {
                    if (EqDmg.Magnitude >= 3 && EqDmg.Magnitude <= 4)
                    {
                        DmgLowLongitude = User.Longitude - 0.1;
                        DmgHighLongitude = User.Longitude + 0.1;
                        DmgLowLatitude = User.Latitude - 0.1;
                        DmgHighLatitude = User.Latitude + 0.1;
                    }

                    else if (EqDmg.Magnitude > 4 && EqDmg.Magnitude <= 6)
                    {
                        DmgLowLongitude = User.Longitude - 0.25;
                        DmgHighLongitude = User.Longitude + 0.25;
                        DmgLowLatitude = User.Latitude - 0.25;
                        DmgHighLatitude = User.Latitude + 0.25;
                    }
                    else if (EqDmg.Magnitude > 6 && EqDmg.Magnitude <= 8)
                    {
                        DmgLowLongitude = User.Longitude - 1;
                        DmgHighLongitude = User.Longitude + 1;
                        DmgLowLatitude = User.Latitude - 1;
                        DmgHighLatitude = User.Latitude + 1;
                    }
                }
            }
            return View();
        }
    }
}
