using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nuar.Configuration
{
    public class Http
    {
        public int Retries { get; set; }
        public bool Exponential { get; set; }
        public double Interval { get; set; }
    }
}