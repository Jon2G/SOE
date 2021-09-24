using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SOE.FireBase
{
    public interface IActionResponse
    {
        Task Execute();
    }
}
