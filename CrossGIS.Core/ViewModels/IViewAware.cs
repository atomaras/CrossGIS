namespace CrossGIS.Core.ViewModels
{
    /// <summary>
    /// Abstraction used to bridge Navigation events in WP Pages
    /// and Activation events in WPF Windows
    /// </summary>
    public interface IViewAware
    {
        void OnViewLoaded();
        void OnActivate(bool isNew);
        void OnDeactivate(bool close);
        bool CanClose();
    }
}
