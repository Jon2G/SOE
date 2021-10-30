using Kit;
using SOEWeb.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SOEAWS.Services
{
    public class TeacherService
    {
        internal static Teacher GetById(int Id)
        {
            Teacher teacher = new Teacher() { Id = Id };
            WebData.Connection.Read("SP_GET_TEACHER_BY_ID",
                (reader) =>
                {
                    string Name = Convert.ToString(reader[1]);
                    teacher = new Teacher() { Id = Id, Name = Name, IsOffline = false };
                }
                , new CommandConfig() { CommandType = CommandType.StoredProcedure }
                , new SqlParameter("TEACHER_ID", Id));
            return teacher;
        }
    }
}
