using System;
using System.Collections.Generic;
using System.Text;
using APIModels.Enums;
using Kit.Sql.Readers;

namespace APIModels
{
    public class Response
    {
        public APIResponseResult ResponseResult { get; set; }
        public string Message { get; set; }
        public  string Extra { get; set; }

        public Response(APIResponseResult ResponseResult,string Message, string Extra="")
        {
            this.ResponseResult = ResponseResult;
            this.Message = Message;
            this.Extra = Extra;
        }
        public Response()
        {
            
        }
        public static Response Error => new Response(APIResponseResult.INTERNAL_ERROR, "ERROR");
        public static Response From(IReader reader)
        {
            if (reader.Read())
            {

                if (!Enum.TryParse(reader[0]?.ToString(), out APIResponseResult result))
                {
                    return new Response(APIResponseResult.INTERNAL_ERROR, "Oops algo salió mal, intente nuevamente");
                }

                string extra = "";
                if (reader.FieldCount > 2)
                {
                    extra = reader[2]?.ToString();
                }
                return new Response(result, reader[1]?.ToString(),extra);
            }
            return Error;
        }


    }
}
