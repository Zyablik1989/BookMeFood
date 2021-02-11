using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Text.RegularExpressions;
using BookMyFood.Common;
using BookMyFood.Model;
using BookMyFood.ServerFunction;
using BookMyFoodWCF;
using Newtonsoft.Json;
using NLog.LogReceiverService;
using RestSharp.Extensions;


namespace BookMyFoodWCF
{


    
// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ServiceChat" in code, svc and config file together.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ServiceChat : IServiceChat, IWebService
    {
        private int nextId = 1;

        //[JsonExtensionData]
        //public List<ServerUser> users = new List<ServerUser>();
        

        public int Connect(string name, bool isLeader)
        {
            ServerUser user = new ServerUser()
            {
                ID = nextId,
                Name = ServerStatus.users.Any(x=>x.Name==name)?  new Regex(@"\w*").Match(name) + " ("+nextId +")" : name,
                operationContext = OperationContext.Current,
                isLeader = isLeader,
                //Order = 
                //    new UserOrder() { Items = 
                //        new List<Item>() {
                //            new Item()
                //    {
                //        ID = 1,
                //        Name = "Дольчик",
                //        Description = "123",
                //        Price = 2444M,
                //        Quantity = 6
                //    },
                //            new Item()
                //            {
                //            ID = 2,
                //            Name = "Фетучине Фетучине ФетучинеФетучине Фетучине Фетучине Фетучине Фетучине Фетучине ФетучинеФетучине",
                //            Description = "Хавка",
                //            Price = 16.05M,
                //            Quantity = 30
                //        }

                //        }
                //}
            };
            ServerStatus.users.Add(user);
            //SendMsg(user.Name + " подключился к чату", 0);
            


            nextId++;
            return user.ID;
        }

        public void Disconnect(int id)
        {
            //var user = ServerStatus.users.FirstOrDefault(i => i.ID == id);
            var user = ServerStatus.users.FirstOrDefault(x => x.ID == id);
            //var user = ServerStatus.users[ServerStatus.users.FindIndex(ind => ind.ID == id)];
            if (user != null)
            {
                SendMsg(user.Name + " покинул чат", 0);
                ServerStatus.users.Remove(user);
                
            }

        }

        public void SendMsg(string msg, int id)
        {
            string date = DateTime.Now.ToLongTimeString();
            var user = ServerStatus.users.FirstOrDefault(i => i.ID == id);
            string userSender = string.Empty;
            
            if (user != null)
            {
                 userSender = ": " + user.Name + " ";
            

            string answer = date + userSender + msg;

            foreach (var addressee in ServerStatus.users)
            {

                addressee.operationContext.GetCallbackChannel<IServerChatCallBack>().MsgCallBack(answer);
            }
            }

        }

        public void SendUser(ServerUser user)
        {
            try
            {
                ServerStatus.users[ServerStatus.users.FindIndex(ind => ind.ID == user.ID)] = user;
                ServerStatus.users[ServerStatus.users.FindIndex(ind => ind.ID == user.ID)].operationContext = OperationContext.Current;
                ServerStatus.users[ServerStatus.users.FindIndex(ind => ind.ID == user.ID)].LastSeen = DateTime.Now;

                if (ServerStatus.users[ServerStatus.users.FindIndex(ind => ind.ID == user.ID)].Ready
                && ServerStatus.users[ServerStatus.users.FindIndex(ind => ind.ID == user.ID)].MissionForClient == 0
                    //&& ServerStatus.users[ServerStatus.users.FindIndex(ind => ind.ID == user.ID)].MissionForClient != 4
                    )
                {
                    ServerStatus.users[ServerStatus.users.FindIndex(ind => ind.ID == user.ID)].MissionForClient = 2;

                }
                if (ServerStatus.users[ServerStatus.users.FindIndex(ind => ind.ID == user.ID)].MissionForClient == 3)
                {
                    ServerStatus.users[ServerStatus.users.FindIndex(ind => ind.ID == user.ID)].MissionForClient = 4;
                }
                if (ServerStatus.users[ServerStatus.users.FindIndex(ind => ind.ID == user.ID)].MissionForClient == 5)
                {
                    ServerStatus.users[ServerStatus.users.FindIndex(ind => ind.ID == user.ID)].MissionForClient = 6;
                }
                if (ServerStatus.users[ServerStatus.users.FindIndex(ind => ind.ID == user.ID)].MissionForClient == 7)
                {
                    ServerStatus.users[ServerStatus.users.FindIndex(ind => ind.ID == user.ID)].MissionForClient = 8;
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                Log.Instance.W(this, "User not found");
            }
            
        }

        public ServerStatus GetServerStatus(bool ready = false)
        {
         return ServerStatus.Current;
        }
      
        public LeaderServer Check()
        {
            //var answer = Newtonsoft.Json.JsonConvert.SerializeObject(LeaderServer.Leader);
            return LeaderServer.Leader;
        }

        public List<ServerUser> UsersList()
        {
            //List<string> usersList = users.Select(x => x.ID+ x.Name).ToList();
            return ServerStatus.users;
        }

        public static void CheckForUserLastSeenTimeout()
        {
            if (ServerStatus.users.Count == 0)
                ServerMaintaining.host.Close();
            
                    DateTime ToTime = DateTime.Now - new TimeSpan(0,0,0,30);
                    ServerStatus.users.RemoveAll(x => x.LastSeen!=DateTime.MinValue && x.LastSeen < ToTime);

            
                   //DateTime LS = user.LastSeen;
                   // int result = DateTime.Compare(LS, ToTime);
                   // if (result < 0)
                   // {
                   //     ServerStatus.users.Remove(user);
                   // }
                
            
        }
    }
}