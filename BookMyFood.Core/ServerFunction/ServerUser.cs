using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Newtonsoft.Json;

namespace BookMyFoodWCF
{
    [DataContract]
    [KnownType(typeof(BookMyFood.ServerFunction.ServerStatus))]
    [KnownType(typeof(BookMyFoodWCF.ServerUser))]
    [KnownType(typeof(BookMyFoodWCF.UserOrder))]
    [KnownType(typeof(BookMyFood.Model.Item))]
    public class ServerUser
    {
        private static ServerUser _instance;
        public static ServerUser Current => _instance ?? (_instance = new ServerUser());

        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public UserOrder Order { get; set; }

        [DataMember]
        public bool Ready { get; set; }

        [DataMember]
        public bool isLeader { get; set; }

        [DataMember]
        public int MissionForClient { get; set; }

        [DataMember]
        public DateTime LastSeen { get; set; }

        public OperationContext operationContext { get; set; }

        public override string ToString()
        {
            return $"{ID}/{Name}";
        }
    }
}
