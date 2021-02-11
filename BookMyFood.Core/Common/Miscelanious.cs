using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BookMyFood.Common
{
    public class Miscelanious
    {
        public static List<string> ResolveIP(string server)
        {
            List<string> s = new List<string>();
            server.Replace(@"http:\\", "");
            server.Replace(@"https:\\", "");
            IPAddress[] ipaddress = Dns.GetHostAddresses(server);

            foreach (var ip in ipaddress)
            {

                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    s.Add(ip.ToString());

            }
            return s;
        }
        public static string GetComputerName()
        {
            string compname = "";

            try
            {
                compname = Environment.MachineName;
            }
            catch (Exception)
            {

                return "";
            }

            return compname;
        }
    }
}
