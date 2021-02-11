using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BookMyFood.Common;
using BookMyFood.Model;

namespace BookMyFood.ServerFunction
{
    public class LeaderServer
    {
        
        public ServerStates ServerState { get; set; }
        
        private uint port;
        public uint Port
        {
            get => port == 0 ? 666 : port;
            set {
                if (value <= 65535)
                    port = value;
            }
        }

        public string ServerDNSName { get; set; }
        public string ServerIP { get; set; } /*ServerIP?? Dns.GetHostByName(ServerDNSName).AddressList[0].ToString();*/

        public string VisibleName
        {
            get;

            set;
        }

        public override string ToString()
        {
            return $"{VisibleName??ServerDNSName}/{ServerDNSName}/{ServerIP}/{ServerState}.";
        }
         
        private static LeaderServer _instance;
        public static LeaderServer Leader => _instance ?? (_instance = new LeaderServer());

        public static LeaderServer CurrentServer;
    } 
}
