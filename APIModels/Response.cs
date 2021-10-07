using Kit;
using Kit.Sql.Readers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SOEWeb.Shared.Enums;
using System;
using System.Data;
using System.Data.SqlClient;

namespace SOEWeb.Shared
{
    public class Response
    {
        public APIResponseResult ResponseResult { get; set; }
        public string Message { get; set; }
        public string Extra { get; set; }

        public Response(APIResponseResult ResponseResult, string Message, string Extra = "")
        {
            this.ResponseResult = ResponseResult;
            this.Message = Message;
            this.Extra = Extra;
        }
        public Response()
        {

        }
        public static Response Error => new Response(APIResponseResult.INTERNAL_ERROR, "ERROR");
        public static Response InvalidRequest => new Response(APIResponseResult.INVALID_REQUEST, "!Solicitud invalida!");
        public static Response NotExecuted => new Response(APIResponseResult.NOT_EXECUTED, "!Solicitud invalida/no ejecutada!");
        public static Response Offline => new Response(APIResponseResult.INTERNAL_ERROR, "El servicio web no esta dispobile por el momento.");

        public static Response FromSql(string sql, params SqlParameter[] parameters)
        {
            Response response = Response.NotExecuted;
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
                                 response= new Response(APIResponseResult.INTERNAL_ERROR,
                                     "Oops algo salió mal, intente nuevamente");
                             }

                             string extra = "";
                             if (reader.FieldCount > 2)
                             {
                                 extra = reader[2]?.ToString();
                             }

                             response = new Response(result, reader[1]?.ToString(), extra);
                         }
                         else
                         {
                             response = new Response(APIResponseResult.INTERNAL_ERROR, "ERROR", "Sql not read");
                         }
                     }, new CommandConfig() { CommandType = CommandType.StoredProcedure, ManualRead = true },parameters);
                }
            }
            catch (Exception ex)
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, "ERROR", ex?.ToString());
            }

            return response;
        }

        public Response SetExtra(string v)
        {
            this.Extra = v;
            return this;
        }
    }
}
