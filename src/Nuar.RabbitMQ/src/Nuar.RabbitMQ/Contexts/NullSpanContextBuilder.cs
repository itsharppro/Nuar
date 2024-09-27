namespace Nuar.RabbitMq.Contexts
{
    internal sealed class NullSpanContextBuilder : ISpanContextBuilder
    {
        public string Build(ExecutionData executionData)
        {
            return string.Empty;
        }
    }
}