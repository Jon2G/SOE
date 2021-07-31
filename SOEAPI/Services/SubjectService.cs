using Kit.Sql.Readers;
using SOEWeb.Shared;
using System;
using System.Data;
using System.Data.SqlClient;

namespace SOEWeb.Server.Services
{
    public static class SubjectService
    {
        internal static Subject GetById(int Id)
        {
            using (IReader reader = WebData.Connection.Read("SP_GET_SUBJECT_BY_ID", CommandType.StoredProcedure, 
                new SqlParameter("SUBJECT_ID", Id)))
            {
                if (reader.Read())
                {
                    string Name = Convert.ToString(reader[1]);
                    string Color = Convert.ToString(reader[2]);
                    string ColorDark = Convert.ToString(reader[3]);
                    return new Subject(Id, -1, Name, Color, ColorDark, null) { Guid = (Guid)reader[0] };
                }
            }
            return new Subject();
        }
    }
}
