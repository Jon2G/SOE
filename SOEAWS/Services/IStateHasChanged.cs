using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOEAWS.Services
{
    public interface IStateHasChanged
    {
        public void InvokeStateHasChanged();
    }
}
