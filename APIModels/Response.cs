using Kit.Sql.Readers;
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

        public static Response FromSql(string sql, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection con = WebData.Con())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, con) { CommandType = CommandType.StoredProcedure })
                    {
                        if (parameters != null)
                            cmd.Parameters.AddRange(parameters);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {

                                if (!Enum.TryParse(reader[0]?.ToString(), out APIResponseResult result))
                                {
                                    return new Response(APIResponseResult.INTERNAL_ERROR,
                                        "Oops algo salió mal, intente nuevamente");
                                }

                                string extra = "";
                                if (reader.FieldCount > 2)
                                {
                                    extra = reader[2]?.ToString();
                                }

                                return new Response(result, reader[1]?.ToString(), extra);
                            }
                            else
                            {
                                return new Response(APIResponseResult.INTERNAL_ERROR, "ERROR", "Sql not read");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, "ERROR", ex?.ToString());
            }
        }
        //public static Response From(IReader reader)
        //{
        //    if (reader.Read())
        //    {

        //        if (!Enum.TryParse(reader[0]?.ToString(), out APIResponseResult result))
        //        {
        //            return new Response(APIResponseResult.INTERNAL_ERROR, "Oops algo salió mal, intente nuevamente");
        //        }

        //        string extra = "";
        //        if (reader.FieldCount > 2)
        //        {
        //            extra = reader[2]?.ToString();
        //        }
        //        return new Response(result, reader[1]?.ToString(), extra);
        //    }
        //    return Error;
        //}


    }
}
