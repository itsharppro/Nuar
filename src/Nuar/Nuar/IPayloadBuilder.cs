using Microsoft.AspNetCore.Http;

namespace Nuar
{
    public interface IPayloadBuilder
    {
         Task<string> BuildRawAsync(HttpRequest request);
        Task<T> BuildJsonAsync<T>(HttpRequest request) where T : class, new();
    }
}