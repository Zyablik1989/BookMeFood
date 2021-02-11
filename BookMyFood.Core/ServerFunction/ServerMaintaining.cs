using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using BookMyFood.ClientFuncion;
using BookMyFoodWCF;
using Timer = System.Timers.Timer;

namespace BookMyFood.ServerFunction
{
   public class ServerMaintaining
    {
        List<ServerUser> users = new List<ServerUser>();
        private static int ID = 0;
        private int nextId = 1;
        public static ServiceHost host = new ServiceHost(typeof(BookMyFoodWCF.ServiceChat));
        private static System.Timers.Timer aTimer;

        public static void ServerStart()
        {
            host = new ServiceHost(typeof(BookMyFoodWCF.ServiceChat));

            //restClient.Proxy = WebRequest.GetSystemWebProxy();
            //restClient.Proxy.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;

            try
            {
                host.Open();
                Console.WriteLine("Host is Started");
                LeaderServer.Leader.ServerState = ServerStates.DelivererSet;

                aTimer = new Timer(30000);
                // Hook up the Elapsed event for the timer. 
                aTimer.Elapsed += MonitoringStringAutoUpdating;
                aTimer.AutoReset = true;
                aTimer.Enabled = true;
            }
            catch (AddressAlreadyInUseException e)
            {
                throw;
            }

        }


        private static void MonitoringStringAutoUpdating(Object source, ElapsedEventArgs e)
        {
            BookMyFoodWCF.ServiceChat.CheckForUserLastSeenTimeout();

        }


        public static async Task ServerStop()
        {
            aTimer?.Dispose();
            if (host.State!=CommunicationState.Faulted)
            host?.BeginClose(null,null);


        }



}

}
