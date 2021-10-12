using Kit.Sql.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOEWeb.Shared.Interfaces
{
    public interface IApplicationData
    {
        public UserBase UserBase { get; }
        public SQLiteConnection LiteConnection { get; }
    }
}
