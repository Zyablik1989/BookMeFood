using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using BookMyFood.Model;

namespace BookMyFood.ServerFunction
{
    [DataContract]
    [KnownType(typeof(ServerStatus))]
    [KnownType(typeof(BookMyFoodWCF.ServerUser))]
    [KnownType(typeof(Deliverer))]
    [KnownType(typeof(BookMyFoodWCF.UserOrder))]
    [KnownType(typeof(BookMyFood.Model.Item))]
    public class ServerStatus
    {
        private static ServerStatus _instance;
        public static ServerStatus Current => _instance ?? (_instance = new ServerStatus());

        public static double discount = 0;
        //[DataMember]
        public static List<BookMyFoodWCF.ServerUser> users = new List<BookMyFoodWCF.ServerUser>();

        [DataMember]
        public List<BookMyFoodWCF.ServerUser> Users { get => users; set => users = value; }

        [DataMember]
        public double Discount { get => discount; set => discount = value; }

        [DataMember]
        public Deliverer ServerDeliverer { get; set; }
        [DataMember]
        public ServerStates ServerState { get => LeaderServer.Leader.ServerState; set => LeaderServer.Leader.ServerState = value; }
    }
}
