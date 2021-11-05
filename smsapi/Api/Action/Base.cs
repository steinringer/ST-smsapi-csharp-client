﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Web;
using SMSApi.Api.Response;

namespace SMSApi.Api.Action
{
    public abstract class Base<T>
    {
        private Proxy proxy;

        protected virtual RequestMethod Method => RequestMethod.POST;

        public void Client(IClient client)
        { }

        public T Execute()
        {
            Validate();

            T response;
            Stream data = proxy.Execute(Uri(), Values(), Files(), Method);

            HandleError(data);

            try
            {
                response = ResponseToObject(data);
            }
            catch (SerializationException e)
            {
                //Problem z prasowaniem json'a
                throw new HostException(e.Message + " /" + Uri(), HostException.E_JSON_DECODE);
            }

            data.Close();

            return response;
        }

        public async Task<T> ExecuteAsync()
        {
            Validate();

            T response;
            Stream data = await proxy.ExecuteAsync(Uri(), Values(), Files(), Method);

            HandleError(data);

            try
            {
                response = ResponseToObject(data);
            }
            catch (SerializationException e)
            {
                //Problem z prasowaniem json'a
                throw new HostException(e.Message + " /" + Uri(), HostException.E_JSON_DECODE);
            }

            data.Close();

            return response;
        }

        public void Proxy(Proxy proxy)
        {
            this.proxy = proxy;
        }

        protected TT Deserialize<TT>(Stream data)
        {
            TT result;
            if (data.Length > 0)
            {
                data.Position = 0;
                var serializer = new DataContractJsonSerializer(typeof(TT));
                result = (TT)serializer.ReadObject(data);
                data.Position = 0;
            }
            else
            {
                result = Activator.CreateInstance<TT>();
            }

            return result;
        }

        protected virtual Dictionary<string, Stream> Files()
        {
            return new Dictionary<string, Stream>();
        }

        protected virtual T ResponseToObject(Stream data)
        {
            return Deserialize<T>(data);
        }

        protected abstract string Uri();

        protected virtual void Validate()
        { }

        protected virtual NameValueCollection Values()
        {
            return HttpUtility.ParseQueryString(string.Empty);
        }

        /**
         * 101 Niepoprawne lub brak danych autoryzacji.
         * 102 Nieprawidłowy login lub hasło
         * 103 Brak punków dla tego użytkownika
         * 105 Błędny adres IP
         * 110 Usługa nie jest dostępna na danym koncie
         * 1000 Akcja dostępna tylko dla użytkownika głównego
         * 1001 Nieprawidłowa akcja
         */
        private static bool IsClientError(string code)
        {
            switch (code)
            {
                case "101":
                case "102":
                case "103":
                case "105":
                case "110":
                case "1000":
                case "1001":
                    return true;

                default:
                    return false;
            }
        }

        /**
         * 8 Błąd w odwołaniu
         * 666 Wewnętrzny błąd systemu
         * 999 Wewnętrzny błąd systemu
         * 201 Wewnętrzny błąd systemu
         */
        private static bool IsHostError(string code)
        {
            switch (code)
            {
                case "8":
                case "201":
                case "666":
                case "999":
                    return true;

                default:
                    return false;
            }
        }

        private void HandleError(Stream data)
        {
            data.Position = 0;

            try
            {
                var error = Deserialize<Error>(data);

                if (error.isError())
                {
                    if (IsHostError(error.Code))
                    {
                        throw new HostException(error.Message, error.Code);
                    }

                    if (IsClientError(error.Code))
                    {
                        throw new ClientException(error.Message, error.Code);
                    }

                    throw new ActionException(error.Message, error.Code);
                }
            }
            catch (SerializationException e)
            { }

            data.Position = 0;
        }
    }
}
