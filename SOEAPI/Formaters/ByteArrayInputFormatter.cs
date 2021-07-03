using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace SOEAPI.Formaters
{
    public class ByteArrayInputFormatter : InputFormatter
    {
        public ByteArrayInputFormatter()
        {
            SupportedMediaTypes.Add(Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/octet-stream"));
        }

        protected override bool CanReadType(Type type)
        {
            return type == typeof(byte[]);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var stream = new MemoryStream();
            await context.HttpContext.Request.Body.CopyToAsync(stream);
            return await InputFormatterResult.SuccessAsync(stream.ToArray());
        }
    }
}
