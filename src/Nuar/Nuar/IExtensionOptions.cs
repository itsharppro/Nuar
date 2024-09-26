using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nuar
{
    public interface IExtensionOptions
    {
        int? Order { get; set; }
        bool? Enabled { get; set; }
    }
}