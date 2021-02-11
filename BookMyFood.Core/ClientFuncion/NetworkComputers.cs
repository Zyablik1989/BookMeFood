using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using BookMyFood.Common;
using BookMyFood.ServerFunction;



namespace BookMyFood.ClientFuncion
{
    public class ServerFinderArgs
    {
        public string Message { get; }
        public string State { get; }

        public ServerFinderArgs(string mes, string state)
        {
            Message = mes;
            State = state;
        }
    }

    

    public class NetworkComputers
    {//Определяем лог.
      


        private static void LogMessage(object sender, ServerFinderArgs e)
        {
            Log.Instance.W("Client",$"{e.State} | {e.Message}");
           
        }
        private static Stopwatch sw =new Stopwatch();

        private static List<string> PotentialServersList=new List<string>();
        private static List<string> CheckedServersList = new List<string>();

        private static int ProcessedServer = 0;
        private static bool IsScanning=false;
         
        public static List<LeaderServer> ServerList = new List<LeaderServer>();

        //public delegate void ServerFinderHandler(object sender, ServerFinderArgs e);

        public delegate void ServerFinderHandler(string ServerDNS);
        public delegate void ScanProcessHandler(decimal percent);


        public event ScanProcessHandler PercentPassed;
        public event ServerFinderHandler ServerListUpdated;
        private event ServerFinderHandler PotentialServerFound;



        private static BackgroundWorker bw=new BackgroundWorker();

        

       


        public void Scan()
        {
            if(IsScanning)
                return;
            if (!RESTConnectToServer.ProxyIsSet)
                RESTConnectToServer.SetProxy();
            bw.Dispose();
            IsScanning = true;
            bw=new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            ServerList.Clear();
            
            if (ServerListUpdated != null) ServerListUpdated("");
            bw.DoWork += ActualScanning;
            bw.RunWorkerCompleted += ScanStopping;
            

            bw.RunWorkerAsync();
            
            
        }






        private void ProcessedServerUpdate(int diff=0)
        {
            
            
                if (!bw.CancellationPending)
                {
                    ProcessedServer = ProcessedServer + diff;
                    decimal percent = 0M;

                   if(PotentialServersList.Count!=0)
                     percent = (((decimal) ProcessedServer / (decimal) PotentialServersList.Count) * 100M);
                        if (PercentPassed != null) PercentPassed(percent);
                
               


            }
else
            {
                if (PercentPassed != null) PercentPassed(0);
            }
            
        }


        private void ActualScanning(object sender, DoWorkEventArgs e)
        {
            Log.Instance.W(this,$"Leader Servers search started...");
            sw.Reset();
            sw.Start();

         
            PotentialServerFound = null;
            PotentialServerFound += PotentialServerAnalysis;
            


           
        
            PotentialServersList = new List<string>();
            CheckedServersList = new List<string>();
            ProcessedServer = 0;


         






            //ScanIPs();
            Parallel.Invoke(
                    () => { PotentialServersList.Add("Localhost"); if (PotentialServerFound != null) PotentialServerFound("Localhost"); },
                    () =>{ PotentialServersList.Add("127.0.0.1"); if (PotentialServerFound != null) PotentialServerFound("127.0.0.1");},
                 //() => VisibleComputers()
                 () => GetNetworkComputerNames()

                 ,() => ScanIPs()
                );





        }

       

        public void ScanCancel()
        {
            if (bw.WorkerSupportsCancellation == true)
            {
                // Cancel the asynchronous operation.
                IsScanning = false;
                bw.CancelAsync();
                Log.Instance.W(this,"Trying to cancel.");
            
            Log.Instance.W(this,"Scanning has been canceled");
            Log.Instance.W(this,$"Found {ServerList.Count} servers in {sw.Elapsed}");
            sw.Stop();
            }
            //PotentialServersList.Clear();
            
            ProcessedServerUpdate();
        }

        private  void ScanStopping(object sender, RunWorkerCompletedEventArgs e)
        {
            //await Task.Run(() =>
            //{
            Log.Instance.W(this,"Leader Servers search completed.");
            IsScanning = false;
            sw.Stop();
            //Log.Instance.W(this,"Leader Servers search completed.");
            Log.Instance.W(this,$"Found {ServerList.Count} servers in {sw.Elapsed}");
            //sw.Stop();
            //});
            
            //ProcessedServer = 0;
            ProcessedServerUpdate(PotentialServersList.Count-ProcessedServer);
            //PotentialServersList.Clear();
        }

        private void PotentialServerIPAnalysis(IPAddress ip)
        {
            if (!bw.CancellationPending)
            {

                RestClientResponse answer = RESTConnectToServer.Get(ip.ToString(), ActionsEnum.CheckServer);
                Log.Instance.W(this,ip.ToString() +" - " +answer.AnswerStatus);
                if (!string.IsNullOrEmpty(answer.AnswerStatus) && answer.AnswerStatus == "OK".ToUpper())
                {
                    var NewServer = new LeaderServer()
                    {
                        ServerDNSName = answer.AnswerContent.Substring(0, 10),
                        ServerIP = ip.ToString(),
                        VisibleName = answer.AnswerContent.Substring(0, 10)
                    };

                    ServerList.Add(
                        NewServer
                    );
                    CheckedServersList.Add(ip.ToString());
                    if (ServerListUpdated != null) ServerListUpdated(ip.ToString());
                }
                
                
            }
            else
            {
                Log.Instance.W(this,"REST is Canceled");
            }
        }

        public static LeaderServer ReturnLeaderInfo(string ip)
        {
            
                //Task.Delay(5);
                RestClientResponse answer = RESTConnectToServer.Get(ip, ActionsEnum.CheckServer);


                if (!string.IsNullOrEmpty(answer.AnswerStatus) && answer.AnswerStatus == "OK".ToUpper())
                {
                    LeaderServer foundLeader =
                        Newtonsoft.Json.JsonConvert.DeserializeObject<LeaderServer>(answer.AnswerContent);
                    return null;
            }
                else
                {
                    return null;
                }


            

        }


        private async void PotentialServerAnalysis(string ServerDNS)
        {
            

                var j = PingHost(ServerDNS);
                Log.Instance.W(this,ServerDNS + " + " + j);
                if (j)
                {
                    //Checking for Server get answer
                    if (!CheckedServersList.Contains(ServerDNS) /*&&!PotentialServersList.Contains(ServerDNS*/)
                    {
                        if (!bw.CancellationPending)
                        {

                        await Task.Run(() =>
                        {
                            //Task.Delay(5);
                            RestClientResponse answer = RESTConnectToServer.Get(ServerDNS, ActionsEnum.CheckServer);


                            if (!string.IsNullOrEmpty(answer.AnswerStatus) && answer.AnswerStatus == "OK".ToUpper())
                            {
                                LeaderServer foundLeader=Newtonsoft.Json.JsonConvert.DeserializeObject<LeaderServer>(answer.AnswerContent);

                                string ip = ResolveIP(foundLeader.ServerDNSName)[0];

                                if (!ServerList.Any(x => x.VisibleName == foundLeader.VisibleName))
                                {
                                    
                                    
                                    ServerList.Add(
                                        foundLeader
                                    );


                                    Log.Instance.W(this, $" + server added {ServerDNS} - {ip}");
                                    if (ServerListUpdated != null) ServerListUpdated(foundLeader.ServerDNSName);
                                }
                                CheckedServersList.Add(foundLeader.ServerDNSName);
                            }
                            else
                            {
                                try
                                {
                                    CheckedServersList.Add(ServerDNS);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                    Log.Instance.W(this, $" Error occured when server suppose to be added: {e.Message}");
                                    
                                }
                                //TODO DELETE
                                Log.Instance.W(this,$" & not answering {ServerDNS}");

                            }
                           
                            
                        });
                    }
                       

                } //TODO DELETE
                    else
                    {
                        Log.Instance.W(this,$" @ already checked {ServerDNS}");
                    }
                    ProcessedServerUpdate(1);
            }
            

        }

        public static List<string> ResolveIP(string server)
        {
            List<string> s = new List<string>();
            IPAddress[] ipaddress = Dns.GetHostAddresses(server);

            foreach (var ip in ipaddress)
            {
                
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    s.Add(ip.ToString());
                
            }
            return s;
        }


        private async void VisibleComputers(bool workgroupOnly = false)
        {
          
            Func<string, IEnumerable<DirectoryEntry>> immediateChildren = key => new DirectoryEntry("WinNT:" + key)
                .Children
                .Cast<DirectoryEntry>();
            Func<IEnumerable<DirectoryEntry>, IEnumerable<string>> qualifyAndSelect =
                entries => entries.Where(c => c.SchemaClassName == "Computer")
                    .Select(Selector);

            var j = (
                !workgroupOnly ?
                    qualifyAndSelect(immediateChildren(String.Empty)
                        .SelectMany(d => d.Children.Cast<DirectoryEntry>()))
                    :
                    qualifyAndSelect(immediateChildren("//WORKGROUP"))
            ).ToList();
        }

        private string Selector(DirectoryEntry c)
        {
            //TODO!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! EVENT
            


                if (!PotentialServersList.Contains(c.Name.ToUpper().Trim()))
                {
                    Task.Run(() =>
                    {
                        if (!bw.CancellationPending)
                        {
                            PotentialServersList.Add(c.Name.ToUpper().Trim());
                            Log.Instance.W(this,"VisComp, added" + c.Name.ToUpper().Trim());
                            if (PotentialServerFound != null) PotentialServerFound(c.Name.ToUpper().Trim());
                        }
                    });
                
            }


            return c.Name;
        }

        #region C++Scanning props

        //declare the Netapi32 : NetServerEnum method import
        [DllImport("Netapi32", CharSet = CharSet.Auto,
             SetLastError = true),
         SuppressUnmanagedCodeSecurity]

        // The NetServerEnum API function lists all servers of the 
        // specified type that are visible in a domain.
        public static extern int NetServerEnum(
            string ServerNane, // must be null
            int dwLevel,
            ref IntPtr pBuf,
            int dwPrefMaxLen,
            out int dwEntriesRead,
            out int dwTotalEntries,
            int dwServerType,
            string domain, // null for login domain
            out int dwResumeHandle
        );

        //declare the Netapi32 : NetApiBufferFree method import
        [DllImport("Netapi32", SetLastError = true),
         SuppressUnmanagedCodeSecurity]

        // Netapi32.dll : The NetApiBufferFree function frees 
        // the memory that the NetApiBufferAllocate function allocates.         
        public static extern int NetApiBufferFree(
            IntPtr pBuf);

        //create a _SERVER_INFO_100 STRUCTURE
        [StructLayout(LayoutKind.Sequential)]
        public struct _SERVER_INFO_100
        {
            internal int sv100_platform_id;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string sv100_name;
        }

        #endregion

        public async void GetNetworkComputerNames()
        {
            List<string> networkComputerNames = new List<string>();
            const int MAX_PREFERRED_LENGTH = -1;
            int SV_TYPE_WORKSTATION = 1;
            int SV_TYPE_SERVER = 2;
            IntPtr buffer = IntPtr.Zero;
            IntPtr tmpBuffer = IntPtr.Zero;
            int entriesRead = 0;
            int totalEntries = 0;
            int resHandle = 0;
            int sizeofINFO = Marshal.SizeOf(typeof(_SERVER_INFO_100));

            try
            {
                int ret = NetServerEnum(null, 100, ref buffer,
                    MAX_PREFERRED_LENGTH,
                    out entriesRead,
                    out totalEntries, SV_TYPE_WORKSTATION |
                                      SV_TYPE_SERVER, null, out
                    resHandle);
                //if the returned with a NERR_Success 
                //(C++ term), =0 for C#
                if (ret == 0)
                {
                    //loop through all SV_TYPE_WORKSTATION and SV_TYPE_SERVER PC's
                    for (int i = 0; i < totalEntries; i++)
                    {
                        tmpBuffer = new IntPtr((int)buffer +
                                               (i * sizeofINFO));

                        //Have now got a pointer to the list of SV_TYPE_WORKSTATION and SV_TYPE_SERVER PC's
                        _SERVER_INFO_100 svrInfo = (_SERVER_INFO_100)
                            Marshal.PtrToStructure(tmpBuffer,
                                typeof(_SERVER_INFO_100));

                        //add the Computer name to the List
                        //TODO!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! EVENT
                        if (!PotentialServersList.Contains(svrInfo.sv100_name.ToUpper().Trim()))
                        {

                            await Task.Run(() =>
                            {
                                if (!bw.CancellationPending)
                                {
                                    PotentialServersList.Add(svrInfo.sv100_name.ToUpper().Trim());
                                    Log.Instance.W(this,"C++, added" + svrInfo.sv100_name.ToUpper().Trim());
                                    if (PotentialServerFound != null)
                                        PotentialServerFound(svrInfo.sv100_name.ToUpper().Trim());
                                    //ProcessedServerUpdate(1);
                                }
                            });
                        }


                        networkComputerNames.Add(svrInfo.sv100_name);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                //The NetApiBufferFree function frees the allocated memory
                NetApiBufferFree(buffer);
            }

            //return networkComputerNames;
        }


        public void ScanIPs()
        {

            List<IPAddress> ips = new List<IPAddress>();

            var interfaces = NetworkInterface.GetAllNetworkInterfaces().ToList();
            foreach (var networkInterface in interfaces)
            {
                var addresses = networkInterface.GetIPProperties().UnicastAddresses;
                foreach (var address in addresses)
                {
                    if (address.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        var mask = address.IPv4Mask;
                        if (mask.Equals(
                            new IPAddress(new byte[] {255, 255, 255, 0})
                        ))
                        {
                           
                            byte[] byteIP = address.Address.GetAddressBytes();
                            for (byte i = 1; i < 255; i++)
                            {
                                var ip = new IPAddress(new byte[]
                                    {
                                        byteIP[0],
                                        byteIP[1],
                                        byteIP[2],
                                        i
                                    }
                                );

                                ips.Add(ip);

                            }
                        }
                    }
                }
            }
            
            PotentialServersList.AddRange(ips.ConvertAll(input => input.ToString()));
            
             ips.AsParallel()
                                 
                                 .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .WithMergeOptions(ParallelMergeOptions.NotBuffered)
                                //.WithDegreeOfParallelism(20)
                                .ForAll( x =>
                                 {
                                     
                                     if (!bw.CancellationPending)
                                     {
          
                                         var j = PingHost(x.ToString());
                                         Log.Instance.W(this,x.ToString() + " + " + j.ToString());
                                         if (!bw.CancellationPending)
                                         {

                                             if (j)
                                             {

                                               
                                                 RestClientResponse answer =
                                                     RESTConnectToServer.Get(x.ToString(), ActionsEnum.CheckServer);
                                                 if (!string.IsNullOrEmpty(answer.AnswerStatus) &&
                                                     answer.AnswerStatus == "OK".ToUpper())
                                                 {

                                                     LeaderServer foundLeader =
                                                         Newtonsoft.Json.JsonConvert.DeserializeObject<LeaderServer>(
                                                             answer.AnswerContent);

                                                     if (!ServerList.Any(y=>y.VisibleName==foundLeader.VisibleName))
                                                         
                                                     {
                                                         ServerList.Add(
                                                             foundLeader
                                                         );
                                                         if (ServerListUpdated != null)
                                                             ServerListUpdated(foundLeader.ServerDNSName);
                                                     }

                                                     CheckedServersList.Add(x.ToString());

                                                 }
                                             }
                                         }

                                         
                                     }

                                     ProcessedServerUpdate(1);
                                 });


             
        }


        public static bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                
                PingReply reply = pinger.Send(nameOrAddress,30);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }
    }
    }



