using System.Collections.Generic;

namespace Nuar
{
    internal interface IExtensionProvider
    {
        IEnumerable<IEnabledExtension> GetAll();
    }
}