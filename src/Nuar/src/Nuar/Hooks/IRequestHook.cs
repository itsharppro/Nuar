using Microsoft.AspNetCore.Http;

namespace Nuar.Hooks
{
    public interface IRequestHook
    {
        Task InvokeAsync(HttpRequest request, ExecutionData data);
    }
}