namespace Nuar.RabbitMq
{
    public interface IContextBuilder
    {
        object Build(ExecutionData executionData);
    }
}