using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnectionCartographer
{
    /**
     * Obtains the host's public IP address. Note that this takes several seconds to complete.
     */
    class GetPublicIP
    {
        // This method was copied from user r.zarei at https://stackoverflow.com/questions/3253701/get-public-external-ip-address
        public static string getPublicIP()
        {
            string url = "http://checkip.dyndns.org";
            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            string response = sr.ReadToEnd().Trim();
            string[] a = response.Split(':');
            string a2 = a[1].Substring(1);
            string[] a3 = a2.Split('<');
            string a4 = a3[0];
            return a4;
        }
    }
}
