using Kit;
using Microsoft.Extensions.Logging;
using SOEWeb.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SOEAWS.Services
{
    public static class TodoService
    {
        public static TodoBase Find(Guid ToDoGuid, ILogger logger,out string UserNick)
        {
            TodoBase todo = null;
            UserNick = string.Empty;
            string usernick = string.Empty;
            try
            {
                WebData.Connection.Read("SP_GET_TODO_BY_GUID", (reader) =>
                 {
                     todo = new TodoBase()
                     {
                         Guid = Guid.Parse(Convert.ToString(reader[0])),
                         Title = Convert.ToString(reader[1]),
                         Description = Convert.ToString(reader[2]),
                         Date = Convert.ToDateTime(reader[3]),
                         Time = TimeSpan.Parse(Convert.ToString(reader[4])),
                         Subject = new Subject()
                         {
                             Id = Convert.ToInt32(reader[5]),
                             IdTeacher = Convert.ToInt32(Convert.ToInt32(reader[6]))
                         }
                     };
                     usernick = Convert.ToString(reader[7]);
                 }, CommandType.StoredProcedure, new SqlParameter("GUID", ToDoGuid));
                UserNick = usernick;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Todo service");
            }
            return todo;
        }
    }
}
