namespace Nuar.Hooks
{
    public interface IHttpResponseHook
    {
        Task InvokeAsync(HttpResponseMessage response, ExecutionData data);
    }
}