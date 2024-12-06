using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SMSApi.Api
{
    public abstract class ClientBase : IClient
    {
        private readonly string _clientAgent = $"smsapi-csharp-client/{Assembly.GetExecutingAssembly().GetName().Version} {Environment.Version}";

        public abstract KeyValuePair<string, string> DefaultRequestHeaders { get; }

        public string GetClientAgent()
        {
            return _clientAgent;
        }
    }
}
