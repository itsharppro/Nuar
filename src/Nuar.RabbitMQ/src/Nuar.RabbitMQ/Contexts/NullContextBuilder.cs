using Nuar.RabbitMq;

namespace Nuar.RabbitMQ.Contexts
{
    internal sealed class NullContextBuilder : IContextBuilder
    {
        public object Build(ExecutionData executionData)
        {
            return new NullContext();
        }
    }
}