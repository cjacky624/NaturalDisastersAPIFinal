//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NaturalDisastersAPIFinal.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Earthquake
    {
        public int EarthquakeID { get; set; }
        public Nullable<double> Longitude { get; set; }
        public Nullable<double> Latitude { get; set; }
        public string Alert { get; set; }
        public Nullable<int> Time { get; set; }
    }
}
