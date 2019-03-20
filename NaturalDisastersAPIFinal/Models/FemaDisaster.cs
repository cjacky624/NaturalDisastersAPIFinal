using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NaturalDisastersAPIFinal.Models
{
    public class FemaDisaster
    {
        //class to make pulling all disaster types easier later
        
        public int disasterNumber { get; set; }
        public string state { get; set; }
        public string incidentType { get; set; }
        public string title { get; set; }
        public string incidentBeginDate { get; set; }
        public string incidentEndDate { get; set; }
        public string declaredCountyArea { get; set; }
        
            
    }
    public class MetaDataWrapper
    {
        //this is literally wrapping around out entity name of the FEMA Data
   
        public List<FemaDisaster> DisasterDeclarationsSummaries { get; set; }
    }
}