namespace CrossGIS.Core
{
    public interface IXamlResourceProvider
    {
        T Resolve<T>(string resourceName);
    }
}
