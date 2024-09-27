namespace Nuar.RabbitMq
{
    public interface ISpanContextBuilder
    {
        string Build(ExecutionData executionData);
    }
}