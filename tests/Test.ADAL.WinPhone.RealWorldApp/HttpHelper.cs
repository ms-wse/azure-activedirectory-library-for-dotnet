
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Test.ADAL.WinPhone.RealWorldApp
{
    class HttpHelper
    {
        public static async Task<string> GetResourceData(string uri, IDictionary<string, string> headerValues)
        {
            try
            {
                HttpWebRequest webrequest =
                    (HttpWebRequest) WebRequest.Create(uri);
                webrequest.Method = "GET";
                foreach (var key in headerValues.Keys)
                {
                    webrequest.Headers[key] = headerValues[key];
                }
                using (WebResponse response = await webrequest.GetResponseAsync())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        return await reader.ReadToEndAsync();
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
