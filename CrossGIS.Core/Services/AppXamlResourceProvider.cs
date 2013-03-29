using System.Windows;

namespace CrossGIS.Core.Services
{
    public class AppXamlResourceProvider : IXamlResourceProvider
    {
        public T Resolve<T>(string resourceName)
        {
            return (T) Application.Current.Resources[resourceName];
        }
    }
}
