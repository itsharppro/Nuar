using Microsoft.AspNetCore.Http;

namespace Nuar.Hooks
{
    public interface IResponseHook
    {
        Task InvokeAsync(HttpResponse response, ExecutionData data);
    }
}