using Nuar.Configuration;

namespace Nuar
{
    internal interface IUpstreamBuilder
    {
        string Build(Module module, Route route);
    }
}