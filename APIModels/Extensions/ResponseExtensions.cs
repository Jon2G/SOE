using Kit;
using Kit.Services.Web;
using Kit.Sql.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

// ReSharper disable once CheckNamespace
namespace SOEWeb.Shared
{
    public static class ResponseExtensions
    {
        public static Response FromSql(string sql, params SqlParameter[] parameters)
            => FromSql<string>(sql, parameters);

        public static Response<T> FromSql<T>(string sql, params SqlParameter[] parameters)
        {
            Response<T> response = Response<T>.NotExecuted;
            try
            {
                using (SqlConnection con = WebData.Connection)
                {
                    con.Read(sql, (reader) =>
                    {
                        if (reader.Read())
                        {

                            if (!Enum.TryParse(reader[0]?.ToString(), out APIResponseResult result))
                            {
                                response = new Response<T>(APIResponseResult.INTERNAL_ERROR,
                                    "Oops algo salió mal, intente nuevamente");
                            }

                            T extra = default(T);
                            if (reader.FieldCount > 2)
                            {
                                extra = Sqlh.Parse<T>(reader[2]);
                            }

                            response = new Response<T>(result, reader[1]?.ToString(), extra);
                        }
                        else
                        {
                            response = new Response<T>(APIResponseResult.NOT_READ, "ERROR");
                        }
                    }, new CommandConfig() { CommandType = CommandType.StoredProcedure, ManualRead = true }, parameters);
                }
            }
            catch (Exception ex)
            {
                return new Response<T>(APIResponseResult.INTERNAL_ERROR, ex?.ToString());
            }

            return response;
        }
    }
}
