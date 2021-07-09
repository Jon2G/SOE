using System;
using System.Collections.Generic;
using System.Text;

namespace SOE.Enums
{
    [Flags]
    public enum ToDoStatus
    {
        Pending = 0, Done = 1, Archived = 2,
        Invalido = 3
    }
}
