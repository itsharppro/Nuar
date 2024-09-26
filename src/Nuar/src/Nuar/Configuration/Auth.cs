using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nuar.Configuration
{
    public class Auth
    {
        public bool Enabled { get; set; }
        public bool Global { get; set; }
        public IDictionary<string, Policy> Policies { get; set; }
    }
}