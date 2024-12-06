using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using SMSApi.Api.Response.ResponseResolver;

namespace SMSApi.Api.Response.Ping
{
    [DataContract]
    public record PingServiceResponse : IResponseCodeAwareResolver
    {
        [DataMember(Name = "authorized")] public readonly bool Authorized;

        [DataMember(Name = "unavailable")] public readonly IEnumerable<string> UnavailableServices;

        public Dictionary<int, Action<Stream>> HandleExceptionActions() => new();
    }
}


