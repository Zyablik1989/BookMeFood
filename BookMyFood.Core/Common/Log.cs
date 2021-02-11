using System;

using NLog;


namespace BookMyFood.Common
{
    public class ExternalEventArgs : System.EventArgs
    {
        public string Information { get; protected set; }

        public object[] Arguments { get; protected set; }

        public ExternalEventArgs(string information, params object[] args)
        {
            Information = information;
            Arguments = args;
        }
    }
    public sealed class Log
    {
        private static readonly object Sync = new object();

        private static volatile Log _instance;

        public static Log Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Sync)
                    {
                        if (_instance == null)
                        {
                            var temp = new Log();
                            temp.Init();

                            _instance = temp;
                        }
                    }
                }

                return _instance;
            }
        }

        private LogFactory _factory;

        private void Init()
        {
            if (_factory == null)
            {
                _factory = new LogFactory();
            }
        }

        private Logger Logger(object obj)
        {
            return _factory.GetLogger(obj.GetType().Name);
        }

        private Logger Logger(string obj)
        {
            return _factory.GetLogger(obj);
        }

        public EventHandler<ExternalEventArgs> ExternalEvent;


        private void External(string message, params object[] args)
        {
            try
            {
                if (ExternalEvent != null && !string.IsNullOrWhiteSpace(message))
                {
                    ExternalEvent(this, new ExternalEventArgs(message, args));
                }
            }
            catch (Exception ex)
            {
                Warn(this, ex);
            }
        }

        public void W(object obj, string message, params object[] args)
        {
            if(obj is string)
                Logger(obj as string).Info(message, args);
            else
                Logger(obj).Info(message, args);

            External(message, args);
        }

        public void Warn(object obj, Exception ex, string message, params object[] args)
        {
            if (obj is string)
                Logger(obj as string).Warn(message + "\n" + ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace, args);
            else
                Logger(obj).Warn(message + "\n" + ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace, args);
            

            External(message + "\n" + ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace, args);
        }

        public void Warn(object obj, Exception ex)
        {
            if (obj is string)
                Logger(obj as string).Warn(ex.Message);
            else
                Logger(obj).Warn(ex.Message);

            External(ex.Message, ex);
        }
    }
}