using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookMyFood.ClientUI;
using Path = System.IO.Path;
using System.Net.Http;
using System.Reflection;
using System.IO;
using System.Net;
using System.Threading.Tasks;


using BookMyFood.Common;

namespace BookMyFood.ClientFuncion
{
    class HTTPConnectToServer
    {
        //Setting log up.
       


        public  HTTPConnectToServer(IViewClientUI Form)
        {
       

        var k = new List<string> {
            "HTTP://10.1.14.150:666", "Sashok", "localhost:433"
        };
        Log.Instance.W(this,"Hello World");

            //serverLaunch();
            ServerConnect();
        }

        private void ServerConnect()
        {
            //string _serverURL = settings.ServerConnectionString; //settingsIniFile.GetPrivateString("ServerConnection", "URL");


            Task.Run(async () => { await ClientGetCurrentVersionFromServer("HTTP://Vasyukov:666"); }).Wait();

            //GetRequest("https://jsonplaceholder.typicode.com/posts/42");
            //PostRequest("http://ptsv2.com/t/crvra-1538685494/post");

        }
        async static Task<int> ClientGetCurrentVersionFromServer(string uri)
        {

            HttpClient client = new HttpClient();
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 |
                                                       SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                WebRequest request = WebRequest.Create(uri);
                //request.Proxy = GenerateProxy();
                request.Method = "Get";
                WebResponse response = await request.GetResponseAsync();
                //HttpResponseMessage response = await client.GetAsync(uri);
                //HttpContent content = response.Content;
                //string stream = await content.ReadAsStringAsync();
                Stream stream = response.GetResponseStream();
                var reader = new StreamReader(stream);
                string xmlString = reader.ReadToEnd();
                reader.Close();
                //string s = reader


                Console.WriteLine(response.ContentLength);
                Console.WriteLine(response.ContentType);
            }
            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }
            return 0;

        }


        
    }


}
