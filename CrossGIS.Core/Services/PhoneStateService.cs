#if WINDOWS_PHONE
using System.Collections.Generic;
using Microsoft.Phone.Shell;

namespace CrossGIS.Core.Services
{
    public class PhoneStateService : IStateService
    {
        public IDictionary<string, object> State
        {
            get { return PhoneApplicationService.Current.State; }
        }
    }
}
#endif
