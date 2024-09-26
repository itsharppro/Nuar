using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nuar
{
    internal interface ISchemaValidator
    {
        Task<IEnumerable<Error>> VaidateAsync(string payload, string schema);
    }
}