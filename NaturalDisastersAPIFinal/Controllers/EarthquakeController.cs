﻿using NaturalDisastersAPIFinal.Models;
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

            double FeltLowLongitude = User.Longitude - 2.3;
            double FeltHighLongitutde = User.Longitude + 2.3;
            double FeltLowLatitude = User.Latitude - 2.3;
            double FeltHighLatitude = User.Latitude + 2.3;


            List<Earthquake> userEarthquakes = new List<Earthquake>();
            foreach(Earthquake Eq in db.Earthquakes)
            {
                if(Eq.Latitude>=FeltLowLatitude && Eq.Latitude<=FeltHighLatitude &&
                    Eq.Longitude >= FeltLowLongitude && Eq.Longitude <= FeltHighLongitutde)
                {
                    if(Eq.Magnitude > 6 && Eq.Magnitude <= 8)
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

            //if (q.Magnitude >= 3 && q.Magnitude <= 4.00)
            //{
            //    DmgLowLongititude = q.Longitude * 0.998;
            //    DmgHighLongititude = q.Longitude * 1.002;
            //    DmgLowLatitude = q.Latitude * 0.998;
            //    DmgHighLatitude = q.Latitude * 1.002;
            //    FeltLowLongititude = q.Longitude - 0.9;
            //    FeltHighLongitutde = q.Longitude + 0.9;
            //    FeltLowLatitude = q.Latitude - 0.9;
            //    FeltHighLatitude = q.Latitude + 0.9;
            //}
            //else if (q.Magnitude > 4 && q.Magnitude <= 6.00)
            //{
            //    DmgLowLongititude = q.Longitude * 0.992;
            //    DmgHighLongititude = q.Longitude * 1.008;
            //    DmgLowLatitude = q.Latitude * 0.992;
            //    DmgHighLatitude = q.Latitude * 1.008;
            //    FeltLowLongititude = q.Longitude - 1.2;
            //    FeltHighLongitutde = q.Longitude + 1.2;
            //    FeltLowLatitude = q.Latitude - 1.2;
            //    FeltHighLatitude = q.Latitude + 1.2;
            //}


            return View();
        }
    }
}