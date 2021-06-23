using System;
using System.Collections.Generic;
using System.Text;
using APIModels.Enums;

namespace APIModels
{
    public class Response
    {
        public APIResponseResult ResponseResult { get; set; }
        public string Message { get; set; }

        public Response(APIResponseResult ResponseResult,string Message)
        {
            this.ResponseResult = ResponseResult;
            this.Message = Message;
        }
        public Response()
        {
            
        }
        public static Response Error => new Response(APIResponseResult.INTERNAL_ERROR, "ERROR");
        public static Response From(Tuple<string, string> Tuple)
        {
            if (!Enum.TryParse( Tuple.Item1,out APIResponseResult result))
            {
                return new Response(APIResponseResult.INTERNAL_ERROR, "Oops algo salió mal, intente nuevamente");
            }
            return new Response(result, Tuple.Item2);
        }


    }
}
