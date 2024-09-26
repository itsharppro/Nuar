namespace Nuar.Hooks
{
    public interface IHttpRequestHook
    {
        Task InvokeAsync(HttpRequestMessage request, ExecutionData data);
    }
}