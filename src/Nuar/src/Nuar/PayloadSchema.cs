using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Nuar
{
    public class PayloadSchema
    {
        public ExpandoObject Payload { get; }
        public string Schema { get; }
        
        public PayloadSchema(ExpandoObject payload, string schema)
        {
            Payload = payload;
            Schema = schema;
        }
    }
}