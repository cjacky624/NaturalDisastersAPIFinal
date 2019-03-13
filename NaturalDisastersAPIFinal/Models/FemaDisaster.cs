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
        public string declarationDate { get; set; }
        public string disasterName { get; set; }
        public string incidentBeginDate { get; set; }
        public string incidentEndDate { get; set; }
        public string declarationType { get; set; }
        public string stateCode { get; set; }
        public string stateName { get; set; }
        public string incidentType { get; set; }
        public string entryDate { get; set; }
        public string updateDate { get; set; }
        public string closeoutDate { get; set; }
        public string hash { get; set; }
        public string lastRefresh { get; set; }
        public string id { get; set; }
        
       
//ihProgramDeclared
//iaProgramDeclared
//paProgramDeclared
//hmProgramDeclared
//state	"GA"
//declarationDate	"1953-05-02T00:00:00.000Z"
//fyDeclared	1953
//disasterType	"DR"
//incidentType	"Tornado"
//title	"TORNADO"
//incidentBeginDate	"1953-05-02T00:00:00.000Z"
//incidentEndDate	"1953-05-02T00:00:00.000Z"
//disasterCloseOutDate	"1954-06-01T00:00:00.000Z"
//declaredCountyArea	""
//placeCode	null
//hash	"e6f77c3a97c63d478bf14c9a58f60a0d"
//lastRefresh	"2018-02-09T14:38:26.149Z"
//id
    }
    public class MetaDataWrapper
    {
        //this is literally wrapping around out entity name of the FEMA Data
        public List<FemaDisaster> FemaWebDisasterDeclarations { get; set; }
    }
}