using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using BookMyFood.Model;
using BookMyFood.ServerFunction;

namespace BookMyFoodWCF
{



// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IServiceChat" in both code and config file together.
    [ServiceContract(CallbackContract = typeof(IServerChatCallBack))]
    public interface IServiceChat
    {

        [OperationContract]
        int Connect(string name, bool isLeader);

        [OperationContract]
        void Disconnect(int id);

        [OperationContract(IsOneWay = true)]
        void SendMsg(string msg, int id);

        [OperationContract]
        void SendUser(ServerUser user);

        //[OperationContract]
        //UserOrder OrderOfUser(int id);

        [OperationContract]
        ServerStatus GetServerStatus(bool ready = false);

        //[OperationContract]
        //void SendOrder(int id, UserOrder order);

        //[OperationContract]
        //List<ServerUser> UsersList();

        //[OperationContract]
        //LeaderServer Check();


    }

    public interface IServerChatCallBack
    {
        [OperationContract(IsOneWay = true)]
        void MsgCallBack(string msg);
    }


    
}