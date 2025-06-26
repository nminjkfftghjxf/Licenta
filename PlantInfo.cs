using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plante2._3
{
    public class PlantInfo
    {
        public string? Name { get; set; }
        public string? CommonName { get; set; }
        public string? Family { get; set; }
        public string? Cycle { get; set; }
        public string? Watering { get; set; }
        public string? Sunlight { get; set; }

        public DateTime Time { get; set; }

        public string? Image { get; set; }
        public string? Account { get; set; }


        public string? Origin { get; set; }
        public string? Type { get; set; }
        public string? Wateringmark { get; set; }
        public string? PruningMonth { get; set; }
        public string? Attracts { get; set; }
        public string? FloweringSeason { get; set; }
        public bool? PH { get; set; }
        public bool? PP { get; set; }
        public string? WateringPeriod { get; set; }
        public string? EdibleP { get; set; }


    }
}
