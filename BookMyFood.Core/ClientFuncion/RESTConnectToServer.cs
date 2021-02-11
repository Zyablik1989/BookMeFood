using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BookMyFood.Common;
using BookMyFood.ServerFunction;
using RestSharp;

namespace BookMyFood.ClientFuncion
{
    public class RestClientResponse
    {

        public string AnswerStatus { get; }
        public string AnswerContent { get; }

        public RestClientResponse(string answerStatus="", string answerContent="")
        {
            AnswerStatus = answerStatus;
            AnswerContent = answerContent;
        }
        
    }

    public class RESTConnectToServer
    {
        //Setting log up.
       

        public static bool ProxyIsSet=false;
        private static RestClient restClient = new RestClient();
        private static RestRequest restRequest = new RestRequest(Method.GET);

        public static void SetProxy()
        {

            try
            {

                restClient.Proxy = WebRequest.GetSystemWebProxy();
                restClient.Proxy.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
               
               
                Log.Instance.W("Client","Rest proxy is set up");
                ProxyIsSet = true;
            }
            catch (Exception e)
            {

                ProxyIsSet = true;
                Log.Instance.W("Client",$"Rest proxy error, {e.Message}");

            }
        }
        

        public static RestClientResponse Get(string URL,  ActionsEnum action=0, string port = "666")
        {
            string route = string.Empty;
            switch (action)
            {
                case ActionsEnum.CheckServer:
                route = "check"; break;
                case ActionsEnum.UsersListRetrieving:
                    route = "UsersList"; break;
                case ActionsEnum.InfoRetrieving:
                    route = "Info"; break;
                  
            }

            //TEST
            //restClient = new RestClient("http://reqres.in/");
            //restRequest = new RestRequest("/api/users?page=2", Method.Get);
            IRestResponse restResponse = null;
            try
            {
                restClient.BaseUrl = new Uri( $"http://{URL}:{port}/{route}");
                restRequest = new RestRequest(Method.GET);
                restRequest.Timeout = 15000;
                restRequest.AddHeader("Accept", "application/json");
                restRequest.RequestFormat = DataFormat.Json;

                
                restResponse = restClient.Execute(restRequest);
                
                return new RestClientResponse(restResponse.StatusCode.ToString(), restResponse.Content);
            }
            catch (Exception e)
            {

                if (restResponse != null)
                {  
                    Log.Instance.W("Client",$"Couldn't connect to server: {URL} at port:{port} / error: {e.Message}");
                    return new RestClientResponse(restResponse.StatusCode.ToString(), restResponse.Content);
                }

                return new RestClientResponse("","");
                
            }
            
        }
        public static RestClientResponse GETjson(string URL, ActionsEnum action, string port = "666")
        {


            //TEST
            //restClient = new RestClient("http://reqres.in/");
            //restRequest = new RestRequest("/api/users?page=2", Method.Get);
            IRestResponse restResponse = null;
            try
            {
                restClient.BaseUrl = new Uri($"http://{URL}:{port}");
                restRequest = new RestRequest(Method.GET);
                restRequest.Timeout = 15000;
                restRequest.AddHeader("Accept", "application/xml");
                restRequest.RequestFormat = DataFormat.Xml;


                restResponse = restClient.Execute(restRequest);

                return new RestClientResponse(restResponse.StatusCode.ToString(), restResponse.Content);
            }
            catch (Exception e)
            {

                if (restResponse != null)
                {
                    Log.Instance.W("Client",$"Couldn't connect to server: {URL} at port:{port} / error: {e.Message}");
                    return new RestClientResponse(restResponse.StatusCode.ToString(), restResponse.Content);
                }

                return new RestClientResponse("", "");

            }

        }
    }
}
