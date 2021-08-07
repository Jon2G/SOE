using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SOEWeb.Shared;
using SOEWeb.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOEAWS.Processors
{
    public class DigesterResult<T>
    {
        public string Extra { get; set; }
        public T Value { get; set; }
        public APIResponseResult ResponseResult { get; set; }

        public DigesterResult(T Value, APIResponseResult ResponseResult, string Extra=null)
        {
            this.Extra = Extra;
            this.Value = Value;
            this.ResponseResult = ResponseResult;
        }
        public DigesterResult()
        {
            ResponseResult = APIResponseResult.NOT_EXECUTED;
        }

        public DigesterResult<T> Log(ILogger log)
        {
           log.LogError($"ResponseResult:[{ResponseResult}],Extra:[{Extra}],Value:[{Value}]");
           return this;
        }

        internal Response ToResponse() => new Response(this.ResponseResult,  this.Extra);
    }
}
