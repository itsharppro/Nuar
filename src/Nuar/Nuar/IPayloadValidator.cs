

using Microsoft.AspNetCore.Http;

namespace Nuar
{
    public interface IPayloadValidator
    {
        Task<bool> TryValidate(ExecutionData executionData, HttpResponse httpResponce);
        Task<IEnumerable<Error>> GetValidationErrorsAsync(PayloadSchema payloadSchema);
    }
}