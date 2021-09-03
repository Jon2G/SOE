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
    public static class ReminderService
    {
        public static ReminderBase Find(Guid Guid, ILogger logger)
        {
            ReminderBase reminder = null;
            try
            {
                WebData.Connection.Read("SP_GET_REMINDER_BY_GUID", (reader) =>
                {
                    reminder = new ReminderBase()
                    {
                        Guid = Guid.Parse(Convert.ToString(reader[0])),
                        Title = Convert.ToString(reader[1]),
                        Date = Convert.ToDateTime(reader[2]),
                        Time = TimeSpan.Parse(Convert.ToString(reader[3]))
                    };
                    if (reader[4] != DBNull.Value)
                    {
                        reminder.Subject = new Subject()
                        {
                            Id = Convert.ToInt32(reader[4]),
                            IdTeacher = Convert.ToInt32(Convert.ToInt32(reader[5]))
                        };
                    }
                }, CommandType.StoredProcedure, new SqlParameter("GUID", Guid));
            }
            catch (Exception e)
            {
                logger.LogError(e, "ReminderService.Find");
            }

            return reminder;
        }
    }
}
