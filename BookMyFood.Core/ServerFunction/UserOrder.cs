using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BookMyFoodWCF
{
    [DataContract]
    [KnownType(typeof(BookMyFood.ServerFunction.ServerStatus))]
    [KnownType(typeof(ServerUser))]
    [KnownType(typeof(UserOrder))]
    [KnownType(typeof(BookMyFood.Model.Item))]
    public class UserOrder
    {
        [DataMember]
        public List<BookMyFood.Model.Item> Items { get; set; }

    }
}
