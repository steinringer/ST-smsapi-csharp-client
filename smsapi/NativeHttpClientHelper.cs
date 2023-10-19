using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SMSApi.Api
{
    public static class NativeHttpClientHelper
    {
        public static Task<Stream> SendRequest(
            this HttpClient httpClient,
            RequestMethod method,
            string uri,
            NameValueCollection body = null,
            Dictionary<string, Stream> files = null
        )
        {
            HttpContent httpContent;

            switch (method)
            {
                case RequestMethod.GET:
                    return httpClient.GetStreamAsync(uri);
                case RequestMethod.POST:
                    httpContent = ConvertNameValueCollectionToHttpContent(body, files);

                    return httpClient.PostAsync(uri, httpContent).Result.Content.ReadAsStreamAsync();
                case RequestMethod.PUT:
                    httpContent = ConvertNameValueCollectionToHttpContent(body, files);

                    return httpClient.PutAsync(uri, httpContent).Result.Content.ReadAsStreamAsync();
                case RequestMethod.DELETE:
                    return httpClient.DeleteAsync(uri).Result.Content.ReadAsStreamAsync();
                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }
        }

        private static HttpContent ConvertNameValueCollectionToHttpContent(
            NameValueCollection collection,
            Dictionary<string, Stream> files = null
        )
        {
            var contentCollectionKeys = collection.AllKeys;
            
            var contentCollection = contentCollectionKeys
                .Select(key => new KeyValuePair<string, string>(key, collection[key]))
                .ToList();
            var formUrlEncodedContent = new FormUrlEncodedContent(contentCollection);

            if (files == null) return formUrlEncodedContent;

            var multipartContent = new MultipartFormDataContent();
            
            foreach (var keyValuePair in contentCollection)
            {
                multipartContent.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
            }
            
            files
                .ToList()
                .ForEach(pair => multipartContent.Add(new StreamContent(pair.Value), "file", pair.Key));

            return multipartContent;
        }
    }
}