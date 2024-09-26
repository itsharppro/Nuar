namespace Nuar.Extensions
{
    public sealed class EnabledExtension : IEnabledExtension
    {
        public IExtension Extension { get; }
        public IExtensionOptions Options { get; }

        public EnabledExtension(IExtension extension, IExtensionOptions options)
        {
            Extension = extension;
            Options = options;
        }
    }
}