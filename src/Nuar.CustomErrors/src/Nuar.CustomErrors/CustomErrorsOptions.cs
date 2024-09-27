using Nuar.Extensions.CustomErrors.Core;

namespace Nuar.Extensions.CustomErrors
{
    public class CustomErrorsOptions : IOptions
    {
        public bool IncludeExceptionMessage { get; set; }

        public bool IncludeDetailedErrors { get; set; }

        public Dictionary<int, string> CustomErrorPages { get; set; } = new Dictionary<int, string>();

        public string LoggingLevel { get; set; } = "Error";

        public bool EnableErrorTracking { get; set; }

        public bool EnableRetryPolicy { get; set; }

        public Dictionary<string, EnvironmentErrorOptions> EnvironmentSettings { get; set; } = new Dictionary<string, EnvironmentErrorOptions>();
    }
}
