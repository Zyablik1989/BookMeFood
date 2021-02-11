using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Web;
using BookMyFood.ServerFunction;

namespace BookMyFoodWCF
{
    [ServiceContract]
    public interface IWebService
    {


        [WebGet( RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        LeaderServer Check();

        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<ServerUser> UsersList();


        //[WebInvoke(Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        //List<string> UsersList();

        //[WebGet( RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        //[OperationContract]
        //List<ServerUser> UsersList();
        //[OperationContract]
        //[WebGet/*(UriTemplate = "Check/{value}")*/] Method = "GET"
        //string Check();
    }
}
