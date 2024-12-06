using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSApi.Api
{
    public interface IClient
    {
        KeyValuePair<string, string> DefaultRequestHeaders { get; }

        string GetClientAgent();
    }
}
