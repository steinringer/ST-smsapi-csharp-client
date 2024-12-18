﻿using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SMSApi.Api.Response.Deserialization;
using smsapi.Api.Response.Deserialization.Exception;
using System.Web;

namespace SMSApi.Api.Action
{
    public abstract class Action<T>
    {
        protected BaseJsonDeserializer BaseJsonDeserializer = new(); //TODO remove after further refactor
        private Proxy _proxy;

        protected abstract RequestMethod Method { get; }

        protected virtual ApiType ApiType()
        {
            return Action.ApiType.Legacy;
        }

        public T Execute()
        {
            Validate();
            return ProcessResponse(_proxy.Execute(Uri(), GetValues(), Files(), Method));
        }

        public async Task<T> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            Validate();
            return ProcessResponse(await _proxy.ExecuteAsync(Uri(), GetValues(), Files(), Method, cancellationToken));
        }

        public void Proxy(Proxy proxy)
        {
            this._proxy = proxy;
        }

        protected virtual Dictionary<string, Stream> Files()
        {
            return new Dictionary<string, Stream>();
        }

        protected virtual T ResponseToObject(HttpResponseEntity data) //TODO get rid of overriding
        {
            IDeserializer deserializer = ApiType() switch
            {
                Action.ApiType.Rest => new RestJsonResponseDeserializer(
                    new LegacyJsonResponseDeserializer(),
                    new ValidationErrorsResolver(new BaseJsonDeserializer()),
                    new TooManyRequestsErrorResolver(),
                    new AccessErrorResolver()
                ),
                Action.ApiType.Legacy => new LegacyJsonResponseDeserializer(),
                _ => throw new Exception("Unknown api type")
            };

            var deserializationResult = deserializer.Deserialize<T>(data);

            deserializationResult.ThrowErrors();

            return deserializationResult.Result;
        }

        protected abstract string Uri();

        protected virtual void Validate()
        {
        }

        protected virtual NameValueCollection Values()
        {
            return new NameValueCollection();
        }

        private T ProcessResponse(HttpResponseEntity responseEntity)
        {
            return ResponseToObject(responseEntity);
        }

        private NameValueCollection GetValues()
        {
            var values = Values();
            return values.Count > 0
                ? new NameValueCollection { { "format", "json" }, values }
                : HttpUtility.ParseQueryString(string.Empty);
        }
    }

}