using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSApi.Api
{
    public class ClientOAuth : ClientBase
    {
        private readonly string _token;

        public ClientOAuth(string token)
        {
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token));

            _token = token;
        }

        public override KeyValuePair<string, string> DefaultRequestHeaders =>
            new KeyValuePair<string, string>("Authorization", $"Bearer {_token}");
    }
}
