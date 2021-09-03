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
        public static TodoBase Find(Guid ToDoGuid,ILogger logger)
        {
            TodoBase todo = null;
            try
            {
                using (SqlConnection con = WebData.Connection)
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_GET_TODO_BY_GUID", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    })
                    {

                        cmd.Parameters.Add(new SqlParameter("GUID", ToDoGuid));
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
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
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               logger.LogError(ex,"Todo service");
            }
            return todo;
        }
    }
}
