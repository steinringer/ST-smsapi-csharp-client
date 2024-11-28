using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SMSApi.Api
{
    public readonly struct HttpResponseEntity
    {
        public readonly Task<Stream> Content;
        public readonly HttpStatusCode StatusCode;

        public HttpResponseEntity(Task<Stream> content, HttpStatusCode statusCode)
        {
            Content = content;
            StatusCode = statusCode;
        }
    }
}
