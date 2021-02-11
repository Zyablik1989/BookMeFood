

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;

namespace BookMyFood { 

public class HTTPServer
{
    private readonly string[] _indexFiles = {
        //"index.html",
        //"index.htm",
        //"default.html",
        //"default.htm"
        "CurrentVersions.xml"
    };

    private static IDictionary<string, string> _mimeTypeMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {
        #region extension to MIME type list
        {".asf", "video/x-ms-asf"},
        {".asx", "video/x-ms-asf"},
        {".avi", "video/x-msvideo"},
        {".bin", "application/octet-stream"},
        {".cco", "application/x-cocoa"},
        {".crt", "application/x-x509-ca-cert"},
        {".css", "text/css"},
        {".deb", "application/octet-stream"},
        {".der", "application/x-x509-ca-cert"},
        {".dll", "application/octet-stream"},
        {".dmg", "application/octet-stream"},
        {".ear", "application/java-archive"},
        {".eot", "application/octet-stream"},
        {".exe", "application/octet-stream"},
        {".flv", "video/x-flv"},
        {".gif", "image/gif"},
        {".hqx", "application/mac-binhex40"},
        {".htc", "text/x-component"},
        {".htm", "text/html"},
        {".html", "text/html"},
        {".ico", "image/x-icon"},
        {".img", "application/octet-stream"},
        {".iso", "application/octet-stream"},
        {".jar", "application/java-archive"},
        {".jardiff", "application/x-java-archive-diff"},
        {".jng", "image/x-jng"},
        {".jnlp", "application/x-java-jnlp-file"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".js", "application/x-javascript"},
        {".mml", "text/mathml"},
        {".mng", "video/x-mng"},
        {".mov", "video/quicktime"},
        {".mp3", "audio/mpeg"},
        {".mpeg", "video/mpeg"},
        {".mpg", "video/mpeg"},
        {".msi", "application/octet-stream"},
        {".msm", "application/octet-stream"},
        {".msp", "application/octet-stream"},
        {".pdb", "application/x-pilot"},
        {".pdf", "application/pdf"},
        {".pem", "application/x-x509-ca-cert"},
        {".pl", "application/x-perl"},
        {".pm", "application/x-perl"},
        {".png", "image/png"},
        {".prc", "application/x-pilot"},
        {".ra", "audio/x-realaudio"},
        {".rar", "application/x-rar-compressed"},
        {".rpm", "application/x-redhat-package-manager"},
        {".rss", "text/xml"},
        {".run", "application/x-makeself"},
        {".sea", "application/x-sea"},
        {".shtml", "text/html"},
        {".sit", "application/x-stuffit"},
        {".swf", "application/x-shockwave-flash"},
        {".tcl", "application/x-tcl"},
        {".tk", "application/x-tcl"},
        {".txt", "text/plain"},
        {".war", "application/java-archive"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".wmv", "video/x-ms-wmv"},
        {".xml", "text/xml"},
        {".xpi", "application/x-xpinstall"},
        {".zip", "application/zip"},
        #endregion
    };
    private Thread _serverThread;
    private string _rootDirectory;
    private HttpListener _listener;
    private int _port;

    public int Port
    {
        get { return _port; }
        private set { }
    }

   
    /// <param name="path">Directory path to serve.</param>
    /// <param name="port">Port of the server.</param>
    public HTTPServer(string path, int port)
    {
        this.Initialize(path, port);
    }

    
    /// <param name="path">Directory path to serve.</param>
    public HTTPServer(string path)
    {
        //get an empty port
        TcpListener l = new TcpListener(IPAddress.Loopback, 0);
           

            l.Start();
        int port = ((IPEndPoint)l.LocalEndpoint).Port;
            
            
        l.Stop();
        this.Initialize(path, port);
    }

 
    public void Stop()
    {
        _serverThread.Abort();
        _listener.Stop();
    }

    private void Listen()
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add(@"http://*:" + _port.ToString() + "/");
           
        _listener.Start();
        while (true)
        {
            try
            {
                    //Получен Get или Post запрос
                HttpListenerContext context = _listener.GetContext();
                    if (context.Request.HttpMethod.ToUpper() == "Get")
                    {
                         Console.WriteLine("\n||||||||||||||||");
                        Console.WriteLine(System.DateTime.Now);
                        Console.WriteLine("Получен Get запрос");
                        ProcessGETrequest(context);
                    }
                    else if (context.Request.HttpMethod.ToUpper() == "POST")
                    {
                        Console.WriteLine("\n||||||||||||||||");
                        Console.WriteLine(System.DateTime.Now);
                        Console.WriteLine("Получен POST запрос");
                        
                        ProcessPOSTrequest(context);


                    }
                    else
                    {
                        Console.WriteLine("\nКлиент обратился с неподдерживаемым методом");
                    }
                
            }
            catch (Exception )
            {
                    throw;   
            }

        }
    }

    private void ProcessGETrequest(HttpListenerContext context)
    {
            

        string filename = context.Request.Url.AbsolutePath;
        Console.WriteLine("IP клиента "+context.Request.RemoteEndPoint.Address);

            filename = filename.Substring(1);

            if (string.IsNullOrEmpty(filename))
        {
            foreach (string indexFile in _indexFiles)
            {
                if (File.Exists(Path.Combine(_rootDirectory, indexFile)))
                {
                    filename = indexFile;
                    break;
                }
            }
        }

        filename = Path.Combine(_rootDirectory, filename);

        if (File.Exists(filename))
        {
            try
            {
                Stream input = new FileStream(filename, FileMode.Open);

                //Adding permanent http response headers
                string mime;
                context.Response.ContentType = _mimeTypeMappings.TryGetValue(Path.GetExtension(filename), out mime) ? mime : "application/octet-stream";
                context.Response.ContentLength64 = input.Length;
                context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                context.Response.AddHeader("Last-Modified", System.IO.File.GetLastWriteTime(filename).ToString("r"));

                byte[] buffer = new byte[1024 * 16];
                int nbytes;
                while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
                    context.Response.OutputStream.Write(buffer, 0, nbytes);
                input.Close();

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.OutputStream.Flush();
                    Console.WriteLine("\nУспешно отправлена информация \nоб актуальных версиях BARS");
                }
            catch (Exception )
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        context.Response.OutputStream.Close();
    }

        private void ProcessPOSTrequest(HttpListenerContext context)
        {

            var request = context.Request;
           
            var stream = context.Request.InputStream;
            BinaryFormatter bf = new BinaryFormatter();
            object DeserializedInfoToSend = bf.Deserialize(stream);
            stream.Close();
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.OutputStream.Flush();
            Console.WriteLine(DeserializedInfoToSend.ToString());
            context.Response.Close();

           
        }

        private void Initialize(string path, int port)
    {
        this._rootDirectory = path;
        this._port = port;

            //Вывод строки об IP и порте в консоль

            Console.WriteLine("\nПеречень IP, по которым доступен этот сервер");

            String strHostName = string.Empty;
            strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
            
                
            

            foreach (IPAddress ip in addr)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    Console.WriteLine(ip.ToString()); 
                }

            }
            string externalip = "";
            try
            {
                externalip = new WebClient().DownloadString("http://icanhazip.com");
                Console.WriteLine(externalip);
            }
            catch (Exception)
            {
                Console.WriteLine(
                    "Не удалось получить внешний IP. Компьютер подключён к интернету?");

                


            }
            




           Console.WriteLine("\nПапка пересылки:\t\t" + this._rootDirectory+
                "\nПапка собранной информации:\tInfoAboutClients\\"+
                "\nПорт:\t\t\t\t" + this._port); //+ "\n\nIP:\t" + IPAddress.Loopback
            _serverThread = new Thread(this.Listen);
        
        _serverThread.Start();
    }


}
}