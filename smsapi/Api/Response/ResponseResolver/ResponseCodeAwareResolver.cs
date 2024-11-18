using System;
using System.Collections.Generic;
using System.IO;

namespace SMSApi.Api.Response.ResponseResolver;

public interface IResponseCodeAwareResolver : IErrorResponse
{
#if !NETSTANDARD
    public Dictionary<int, Action<Stream>> HandleExceptionActions() => new();
#else
    public Dictionary<int, Action<Stream>> HandleExceptionActions();
#endif
}
