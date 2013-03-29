using System.Collections.Generic;

namespace CrossGIS.Core.Services
{
    public class InMemoryStateService : IStateService
    {
        private IDictionary<string, object> _state; 
        public IDictionary<string, object> State
        {
            get { return _state ?? (_state = new Dictionary<string, object>()); }
        }
    }
}
