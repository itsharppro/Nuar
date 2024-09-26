using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nuar.Configuration
{
    public class LoadBalancer
    {
        public bool Enabled { get; set; }
        public string Url { get; set; }
    }
}