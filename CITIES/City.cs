﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITIES
{
    public class City
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public ulong? Population { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
