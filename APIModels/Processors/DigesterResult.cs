using Microsoft.Extensions.Logging;
using SOEWeb.Shared.Enums;

namespace SOEWeb.Shared.Processors
{
    public class DigesterResult<T>
    {
        public string Extra { get; set; }
        public T Value { get; set; }
        public APIResponseResult ResponseResult { get; set; }

        public DigesterResult(T Value, APIResponseResult ResponseResult, string Extra = null)
        {
            this.Extra = Extra;
            this.Value = Value;
            this.ResponseResult = ResponseResult;
        }
        public DigesterResult()
        {
            this.ResponseResult = APIResponseResult.NOT_EXECUTED;
        }

        public DigesterResult<T> Log(ILogger log)
        {
            log.LogError($"ResponseResult:[{this.ResponseResult}],Extra:[{this.Extra}],Value:[{this.Value}]");
            return this;
        }

        public Response ToResponse() => new Response(this.ResponseResult,this.Value.ToString(), this.Extra);
    }
}
