using Kit;
using Kit.Sql.Readers;
using SOEWeb.Shared;
using System;
using System.Data;
using System.Data.SqlClient;

namespace SOEAWS.Services
{
    public static class SubjectService
    {
        internal static Subject GetById(int Id)
        {
            Subject subject = null;
            WebData.Connection.Read("SP_GET_SUBJECT_BY_ID",
               (reader) =>
               {
                   string Name = Convert.ToString(reader[1]);
                   string Color = Convert.ToString(reader[2]);
                   string ColorDark = Convert.ToString(reader[3]);
                   subject = new Subject(Id, -1, Name, Color, ColorDark, null) { Guid = (Guid)reader[0] };
               }
               , new CommandConfig() { CommandType = CommandType.StoredProcedure }
               , new SqlParameter("SUBJECT_ID", Id));
            return subject;
        }
    }
}
