using System;
using System.Collections.Generic;

namespace CrossGIS.Core.Services
{
    public interface IStateService
    {
        IDictionary<String, Object> State { get; } 
    }
}
