using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nuar
{
    internal interface IPolicyManager
    {
        IDictionary<string, string> GetClaims(string policy);
    }
}