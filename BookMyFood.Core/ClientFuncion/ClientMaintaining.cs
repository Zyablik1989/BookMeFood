using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BookMyFood.ClientUI;
using BookMyFood.Common;
using BookMyFood.ServerFunction;
using BookMyFood.ServiceChat;
using BookMyFoodWCF;
using NLog;
using NLog.Internal;
using IServiceChat = BookMyFoodWCF.IServiceChat;
using Item = BookMyFood.Model.Item;
using LeaderServer = BookMyFood.ServerFunction.LeaderServer;
using ServerStates = BookMyFood.ServerFunction.ServerStates;
using ServerStatus = BookMyFood.ServerFunction.ServerStatus;
using ServerUser = BookMyFoodWCF.ServerUser;
using UserOrder = BookMyFoodWCF.UserOrder;


namespace BookMyFood.ClientFuncion
{
   public  class ClientMaintaining /*: BookMyFood.ServiceChat.IServiceChatCallback*/
    {
        #region События
        public delegate void ClientStateHandler(ClientState state);
        public static event ClientStateHandler ClientStateChanged;
        public delegate void ClientUsersHandler(List<BookMyFood.ServiceChat.ServerUser> users);
        public static event ClientUsersHandler ClientUsersChanged;
        #endregion

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr h, string m, string c, int type);

        public static int ID = 0;
        private static bool Subscribed = false;

        public static void CurrentStateChanged(ClientState state)
        {
            
        }

            

        public static bool isSelfServer = false;

        public static bool isConnected = false;
        private static bool currentlyUpdating = false;
        public static Deliverer CurrentDeliverer = null;
        public static double CurrentDiscount = 0;

        private static BackgroundWorker bw = new BackgroundWorker();
        private static ServiceChat.ServiceChatClient client;

        public static BookMyFoodWCF.UserOrder currentOrder;
        public static List<ServiceChat.ServerUser> FinalUsersStatus = new List<ServiceChat.ServerUser>();

        public static NetworkComputers scan = new NetworkComputers();

        public static bool ClientReadiness;
        
       public  static bool IsRunningAsLocalAdmin()
        {
            WindowsIdentity cur = WindowsIdentity.GetCurrent();
            foreach (IdentityReference role in cur.Groups)
            {
                if (role.IsValidTargetType(typeof(SecurityIdentifier)))
                {
                    SecurityIdentifier sid = (SecurityIdentifier)role.Translate(typeof(SecurityIdentifier));
                    if (sid.IsWellKnown(WellKnownSidType.AccountAdministratorSid) || sid.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid))
                    {
                        return true;
                    }

                }
            }

            return false;
        }

        //public static bool GetConnectionStatus()
        //{
        //    return isConnected;
        //}

        public static void Join(string Name, bool SelfServer = false)
        {
            //if (ClientStateChanged==null)
            if (!Subscribed)
            {
                ClientStateChanged += CurrentStateChanged;
                Subscribed = true;
            }
            

            //if (bw.DoWork==null)
            //{
            //    bw.DoWork += Update;
            //}
            //bw.Dispose();
            bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += RepetableUpdate;
            //bw.DoWork += Timer; 
                ClientStateChanged?.Invoke(ClientState.Connecting);

            if (SelfServer)
            {

                client = new BookMyFood.ServiceChat.ServiceChatClient(
                    new System.ServiceModel.InstanceContext(new ChatClientHandler()));

                isSelfServer = true;
                //ClientStateChanged?.Invoke(ClientState.Connecting);

            }
            else
            {

                client = new BookMyFood.ServiceChat.ServiceChatClient(
                    new System.ServiceModel.InstanceContext(new ChatClientHandler())
                    , "WSDualHttpBinding_IServiceChat",
                    $"http://{LeaderServer.Leader.ServerIP}:{LeaderServer.Leader.Port}/Chat");
                isSelfServer = false;
                Log.Instance.W(client, $"Client Endpoint http://{LeaderServer.Leader.ServerIP}:{LeaderServer.Leader.Port}/");
                //Log.Instance.W(client, $"Client Endpoint net.tcp://{LeaderServer.Leader.ServerIP}:{LeaderServer.Leader.Port}/");
                //client.Endpoint.Address = new EndpointAddress($"net.tcp://{LeaderServer.Leader.ServerIP}:{LeaderServer.Leader.Port}/");
                //client 
                //    = new ServiceChatClient(
                //    new InstanceContext(new ChatClientHandler()), "NetTcpBinding_IServiceChat", 
                //    $"net.tcp://{LeaderServer.Leader.ServerIP}:{LeaderServer.Leader.Port}/");
                //ServiceChatClient<IServiceChat> cf =
                //    new ServiceChatClient<IServiceChat>(
                //        new Uri("http://localhost:8090/MyService/EmployeeService"));
                //IServiceChat client = cf.CreateChannel();
                //var d = client.GetEmployee(1);

               
            }


            try
                {
                Log.Instance.W(client, "Trying to connect " );
                ID = client.Connect(Name,isSelfServer);
                Log.Instance.W(client, "Connected ");

                isConnected = true;
                    FinalUsersStatus = new List<ServiceChat.ServerUser>();

                if (SelfServer)
                    ClientStateChanged?.Invoke(ClientState.DelivererSet);
                else
                    ClientStateChanged?.Invoke(ClientState.DelivererWait);
            }
            catch (EndpointNotFoundException exception)
            {
                isSelfServer = false;
                 MessageBox((IntPtr)0, exception.Message, ClientUIPresenter.GetString("Timeout")
                     , 0);
            }
            catch (System.ServiceModel.Security.SecurityNegotiationException exception)
            {
                isSelfServer = false;
                MessageBox((IntPtr)0, ClientUIPresenter.GetString("Thedomainnameyouhaveusedisnotavailableinthisnetwork")
                                      + Environment.NewLine + exception.Message +
                                      Environment.NewLine + exception.InnerException.InnerException, ClientUIPresenter.GetString("NoDomainName")
                    , 0);
            }
            catch (Exception exception)
            {
                isSelfServer = false;
                MessageBox((IntPtr)0, ClientUIPresenter.GetString("Anerroroccuredwhiletryingtoconnect")
                                      + Environment.NewLine + exception.Message, ClientUIPresenter.GetString("Commonerror")
                    , 0);
            }

            bw.RunWorkerAsync();
            //Update();

        }


        public static async Task Disconnect(bool byMission = false)
        {
            if (bw.WorkerSupportsCancellation)
            {
                bw.CancelAsync();
            }
            
            bw.Dispose();
            //if (bw.WorkerSupportsCancellation)
            //{
            //}

            //isSelfServer = false;
            if (isConnected)
            {
                if (client.State != CommunicationState.Faulted)
                {
                    try
                    {
                       await client.DisconnectAsync(ID);
                    }
                    catch (Exception e)
                    {
                        Log.Instance.W(client, "Error appeared when you've tried to send message. Try again. " + e.Message);
                    }
                }

            }

            //bw = new BackgroundWorker();
                isConnected = false;
                //isSelfServer = false;
                client = null;
                ID = 0;

                currentlyUpdating = false;
                currentOrder = null;
                scan = new NetworkComputers();
                
                ClientReadiness = false;
                LeaderServer.CurrentServer = null;
                
                ServerUser.Current.ID = 0;
                ServerUser.Current.LastSeen = DateTime.MinValue;
                ServerUser.Current.isLeader = false;
                ServerUser.Current.operationContext = null;
                ServerUser.Current.MissionForClient = 0;
                ServerUser.Current.Name = string.Empty;
                ServerUser.Current.Order = null;
                ServerUser.Current.Ready = false;
                CurrentDeliverer = null;

            if (!byMission)
            {

                ClientStateChanged?.Invoke(ClientState.Offline);
            }
        }

        public static async Task SendMessage(string msg)
        {
            if (isConnected)
            {
                try
                {
                    client.SendMsgAsync(msg,ID);
                }
                catch (System.ServiceModel.CommunicationObjectFaultedException e)
                {
                    Log.Instance.W(client,"Error appeared when you've tried to send message. Try again. "+ e.Message);
                 
                }
            }
        }

        private static void RepetableUpdate(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (!isConnected)
                 break;

                if (!bw.CancellationPending)
                {
                    Update();
                    Thread.Sleep(500);
                }
                else
                {
                    break;
                }
            }
        }

        public static async Task Update()
        {


            if (!currentlyUpdating)
            {
                currentlyUpdating = true;
                //1
                ServiceChat.ServerStatus serverInfo = null;
                if (isConnected )
                {
                    if (client != null)
                    {

                        {
                            try
                            {
                                serverInfo = await client.GetServerStatusAsync(ClientReadiness);

                            }
                            catch (Exception e)
                            {
                                Log.Instance.W(client ?? new object{}, "An error occurred while the client was trying to update its data");
                                Log.Instance.W(client ?? new object { }, e.Message);
                                
                                return;
                            }
                        }

                    }
                   
                }

                if (serverInfo!=null  && serverInfo.ServerDeliverer!=null )
                {
                     //CurrentDeliverer.ID != serverInfo.ServerDeliverer.ID
                   CurrentDeliverer = serverInfo.ServerDeliverer;
                    //if (isSelfServer)
                    //{
                    //    //LeaderServer.CurrentServer.ServerState == ServerStates.DelivererSet
                    //    ClientStateChanged?.Invoke(ClientState.DelivererSet);
                    //}
                    //else
                    //{
                    //    ClientStateChanged?.Invoke(ClientState.DelivererWait);
                    //}

                }

                //2
                if (serverInfo != null)
                {
                    if (LeaderServer.CurrentServer == null)
                    {
                        LeaderServer.CurrentServer = new LeaderServer();
                    }
                        LeaderServer.CurrentServer.ServerState = (ServerStates) serverInfo.ServerState;
                        


                    ClientUsersChanged?.Invoke(serverInfo.Users);
                }
                else
                {
                    //LeaderServer.CurrentServer = null;
                }

               
                //3
                //client.SendUser(ServerUser.Current);
                if (serverInfo!=null)
                {

                    ServerUser.Current.ID = ID;

                    var CurrentUser = serverInfo.Users.FirstOrDefault(x => x.ID == ID);
                    if (CurrentUser == null || CurrentUser.MissionForClient == 1)
                    {
                        bw.CancelAsync();
                        Disconnect();
                        
                        currentlyUpdating = false;
                        return;
                    }

                    CurrentDiscount = serverInfo.Discount;

                    if (ClientReadiness)
                        FinalUsersStatus = serverInfo.Users;

                    switch (serverInfo.ServerState)
                    {
                        case ServiceChat.ServerStates.DelivererSet:
                            if (isSelfServer)
                            {
                                //LeaderServer.CurrentServer.ServerState == ServerStates.DelivererSet
                                ClientStateChanged?.Invoke(ClientState.DelivererSet);
                            }
                            else
                            {
                                ClientStateChanged?.Invoke(ClientState.DelivererWait);
                            }
                            break;
                        case ServiceChat.ServerStates.OrdersSet:
                            ClientStateChanged?.Invoke(ClientState.OrderSet);
                            break;
                        case ServiceChat.ServerStates.ReadyToCalculate:

                            //if (FinalUsersStatus.Count <= serverInfo.Users.Count)
                            //    FinalUsersStatus = serverInfo.Users;
                            ClientReadiness = true;
                            //ClientStateChanged?.Invoke(ClientState.WaitingForCalculation);
                            //if (isSelfServer && ServerStatus.Current.Users.Count(x => !x.Ready) == 0
                            //                 && ServerStatus.Current.Users.Count(x => x.MissionForClient!=2) == 0)
                            //{
                            //    ServerStatus.Current.ServerState = ServerStates.Verifying;
                            //}
                            if (CurrentUser.MissionForClient == 2)
                            {
                                FinalUsersStatus = serverInfo.Users;
                                CurrentUser.MissionForClient = 3;
                            }

                            if (isSelfServer
                                && (ServerStatus.Current.Users.Count(x => x.MissionForClient == 4))
                                    == ServerStatus.Current.Users.Count())
                            {
                                ServerStatus.Current.ServerState = ServerStates.Verifying;
                            }

                                break;

                        case ServiceChat.ServerStates.Verifying:
                            //if(FinalUsersStatus.Count<= serverInfo.Users.Count)
                            //FinalUsersStatus = serverInfo.Users;
                            //isConnected = false;
                            //FinalUsersStatus = serverInfo.Users;
                            ClientStateChanged?.Invoke(ClientState.Calculated);
                            if (CurrentUser.MissionForClient==4)
                            CurrentUser.MissionForClient = 5;

                            //if (isSelfServer
                            //    && (ServerStatus.Current.Users.Count(x => x.MissionForClient == 6))
                            //        == ServerStatus.Current.Users.Count()
                            //)
                            //{
                            //    foreach (var user in ServerStatus.users)
                            //    {
                            //        user.MissionForClient = 7;
                            //        CurrentUser.MissionForClient = 7;
                            //        //ServerStatus.Current.ServerState = ServerStates.Verifying;
                            //    }
                            //}

                            if (CurrentUser.MissionForClient == 6)
                            {
                                await Task.Run(() => { Thread.Sleep(25000); });
                                await ClientMaintaining.Disconnect(true);
                                //FinalUsersStatus = serverInfo.Users;
                                //CurrentUser.MissionForClient = 3;
                            }

                            if (isSelfServer && CurrentUser.MissionForClient == 6)
                            {
                                await Task.Run(() => { Thread.Sleep(15000); });
                                await ServerFunction.ServerMaintaining.ServerStop();
                            }

                                //await Task.Run(() => { Thread.Sleep(10000); });
                                //await ClientMaintaining.Disconnect(true);
                                //if (isSelfServer
                                //&& (ServerStatus.Current.Users.Count(x => x.MissionForClient == 8)
                                //    == ServerStatus.Current.Users.Count())
                            //)
                            //{
                            //    await Task.Run(() => { Thread.Sleep(3000); });
                            //    await ServerFunction.ServerMaintaining.ServerStop();
                            //}
                            //return;
                            break;
                    }
                    
                    ServerUser.Current.Name = CurrentUser.Name;
                    ServerUser.Current.isLeader = isSelfServer;
                    ServerUser.Current.Ready = ClientReadiness;

                    if (ServerUser.Current.Order==null || ServerUser.Current.Order.Items == null)
                        ServerUser.Current.Order = new UserOrder() { Items = new List<Item>() };

                    var s = ServerUser.Current;
                    var o = new List<ServiceChat.Item>();
                    if (s.Order.Items != null)
                    {
                        foreach (var i in s.Order.Items)
                        {
                            o.Add(new ServiceChat.Item()
                            {
                                ID = i.ID,
                                Quantity = i.Quantity,
                                Price = i.Price,
                                Name = i.Name,
                                Description = i.Description
                            });
                        }
                    }

                    try
                    {
                        var c = new ServiceChat.ServerUser()
                        {
                            ID = s.ID,
                            isLeader = s.isLeader,
                            Name = s.Name,
                            Ready = s.Ready,
                            MissionForClient = CurrentUser.MissionForClient,
                            Order = new ServiceChat.UserOrder() {Items = o}
                        };
                         await client.SendUserAsync(c);
                    }
                    catch (Exception e)
                    {
                        Log.Instance.W(client, ClientUIPresenter.GetString("ErrorappearedwhenyouvetriedtosenduserdataTryagain")
                                               + e.Message);
                        currentlyUpdating = false;
                    }
                }

                currentlyUpdating = false;
            }

        }

        public static string GetMonitoringInfo()
        {
            string s = "";

            if (client == null)
            {
                s += ClientUIPresenter.GetString("Disconnected")
                    ;
                return s;
            }

            if (isConnected  )
            {
                var leader = LeaderServer.Leader;
                if (leader != null )
                {
                    s += ClientUIPresenter.GetString("Connectedto")
                        ;
                    if (isSelfServer)
                    {
                        s += ClientUIPresenter.GetString("Myself")
                            ;
                        s += " | ";

                        switch (LeaderServer.Leader.ServerState)
                        {
                            case ServerStates.DelivererSet:
                                s += ClientUIPresenter.GetString("ServerStatusSelectingStore")
                                    ;
                                break;
                            case ServerStates.OrdersSet:
                                s += ClientUIPresenter.GetString("ServerStatusPickinganitemstoorder")
                                    ;
                                break;
                            case ServerStates.ReadyToCalculate:
                                s += ClientUIPresenter.GetString("ServerStatusGatheringparticipantsreadinessinfo")
                                    ;
                                break;
                            case ServerStates.Verifying:
                                s += ClientUIPresenter.GetString("ServerStatusAnorderswerecalculated")
                                    ;
                                break;
                        }
                        s += " | ";
                        int ready = ServerStatus.users.Count(x => x.Ready);
                        var all = ServerStatus.users.Count;
                        s += ClientUIPresenter.GetString("Participantsreadiness")
                             + ready + ClientUIPresenter.GetString("outof")
                             + all;
                        s += " | ";
                    }
                    else
                    {
                        s+= string.IsNullOrEmpty(leader.VisibleName) ? leader.VisibleName : leader.ServerDNSName;
                        s += " | ";

                        if (LeaderServer.CurrentServer != null)
                        {



                            switch (LeaderServer.CurrentServer.ServerState)
                            {
                                case ServerStates.DelivererSet:
                                    s += ClientUIPresenter.GetString("ServerStatusSelectingStore");
                                    break;
                                case ServerStates.OrdersSet:
                                    s += ClientUIPresenter.GetString("ServerStatusPickinganitemstoorder");
                                    break;
                                case ServerStates.ReadyToCalculate:
                                    s += ClientUIPresenter.GetString("ServerStatusGatheringparticipantsreadinessinfo");
                                    break;
                                case ServerStates.Verifying:
                                    s += ClientUIPresenter.GetString("ServerStatusAnorderswerecalculated");
                                    break;
                            }

                            s += " | ";
                        }
                    }



                    return s;


                }
                else
                {
                    s += ClientUIPresenter.GetString("Disconnected");
                    return s;
                }
             }

            else
            {
                s += ClientUIPresenter.GetString("Disconnected");
                return s;
            }





        }
    }

    public class ChatClientHandler : IServiceChatCallback
    {
        public delegate void ChatMessageHandler(string chatMessage);

        public static event ChatMessageHandler ChatMessageCame;

        public static List<object> ChatBoxContainment = new List<object>();

        public void MsgCallBack(string msg)
        {
            ChatBoxContainment.Add(msg);
            if (ChatMessageCame != null) ChatMessageCame(msg);
        }
    }
}
