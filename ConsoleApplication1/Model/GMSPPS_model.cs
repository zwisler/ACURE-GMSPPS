using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMSPPStoGMS.Model
{
    public class GPMS_MissionModel
    {
        public string DefaultUrl = "http://gmspps.azurewebsites.net/";
        public string CostumMissionID { get; set; }
        public string URL { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public Position[] Positions { get; set; }
    }
    public class Position
    {
        public double LAT { get; set; }
        public double LON { get; set; }
    }
    public class MissionState
    {
        public int ID { get; set; }
        public int State { get; set; }
    }
}
