using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
//using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;


namespace BookMyFood.Common
{
    /// <remarks></remarks>
    public class Log
    {
        /// <summary>
        /// Уровень дебаглога для локов
        /// </summary>
        public const int LockDebugLevel = 2;

        /// <summary>
        /// Перечисление уровней ошибок системы ведения лога
        /// </summary>
        public enum eLogLevel
        {
            /// <summary>
            /// Отладка
            /// </summary>
            debug,
            /// <summary>
            /// Запись ошибок
            /// </summary>
            error,
            /// <summary>
            /// Запись кртических ошибок
            /// </summary>
            critical
        };

        /// <summary>
        /// Текущий уровень ведения лога в системе
        /// </summary>
        private eLogLevel level;
        /// <summary>
        /// Путь к файлу-логу
        /// </summary>
        private static string path;
        /// <summary>
        /// Ограничение по строкам
        /// </summary>
        private int restrict;

        private static Log instance;

        protected Log(eLogLevel level, string _path, int restrict)
        {
            Buffer = new StringBuilder();

            this.level = level;
            path = "Log.log";
            this.restrict = restrict;
        }
        /// <summary>
        /// Синглтон лога
        /// </summary>
        /// <param name="logLevel">Уровень ведения лога</param>
        /// <param name="logPath">Путь к файлу-логу</param>
        /// <param name="logRestrict">Ограничение по строкам. Число 0 для работы без ограничения.</param>
        public static Log Instance(eLogLevel logLevel, string logPath, int logRestrict)
        {
            // Use 'Lazy initialization' 
            if (instance == null)
            {
                lock (syncLock)
                {
                    if (instance == null)
                    {
                        instance = new Log(logLevel, logPath, logRestrict);
                    }
                }
                
            }

            return instance;
        }


        // Lock synchronization object
        private static object syncLock = new object();


        /// <summary>
        /// Записать сообщение в лог
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        public static void W(string message)
        {
            Write(message);
        }

        /// <summary>
        /// Буффер
        /// </summary>
        public static StringBuilder Buffer
        {
            get;
            set;
        }

        /// <summary>
        /// Запись в буфер включена
        /// </summary>
        public static bool BufferEnabled
        {
            get;
            set;
        }

        

        
        

        /// <summary>
        /// Записать сообщение в лог
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        /// <param name="eventLogMnem">Мнемоника в текстовом представлении</param>
        /// <remarks>Пользоваться аккуратно! Легко принять eventLogMnem за элемент форматирования</remarks>
       
   
        public static void WriteToLog(Exception e)
        {
            string message = String.Format("\r\nИсключение: {0} {2}\r\n" +
                                            "Message:\t{1}\r\n",
                                            e.GetType().ToString(),
                                            e.Message ?? "<null>",
                                            e.StackTrace);

            

            //переданные в исключении данные
            if (e.Data != null)
                if (e.Data.Count > 0) message += "\r\nДанные исключения:\r\n";
            foreach (DictionaryEntry item in e.Data)
            {
                message += String.Format("Key:\t{0}\r\nValue:\t{1}\r\n\r\n",
                    item.Key.ToString(), item.Value==null?"<не задан>":item.Value.ToString());
            }

            Write(message);
        }

       


        public static StreamWriter writer = null;
        static string dateFormat = "dd.MM.yy HH:mm:ss.f";

        public static void Release()
        {
            if (writer != null)
                writer.Close();
        }

        /// <summary>
        /// Записать сообщение в лог
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        public static void Write(string message)
        {
            // Текущая дата
            DateTime date = DateTime.Now;

            if (instance == null)
            {
                instance=new Log(eLogLevel.error,"Log.log",1);
            }

            // Имя файла-лога
            string fileName = path;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            lock (syncLock)
            {
                try
                {
                    if (!File.Exists(path) || writer == null)
                    {

                        if (File.Exists(path))
                        {
                            FileInfo f = new FileInfo(path);
                            if (f.Length > 50000000)
                            {
                                File.Delete(path);
                            }
                        }

                       

                        //новый поток
                        writer = new StreamWriter(path, true, Encoding.GetEncoding(1251));
                    }
                    var line = string.Format("{0} {1}", date.ToString(dateFormat + "|"), message);

                    //Stopwatch sw = new Stopwatch();
                    //sw.Start();

                    //var line = string.Format("{0} {1}", date.ToString(dateFormat), message);

                    writer.WriteLine(line);
                    
                    
                    //дополнительная запись в буфер
                    if (BufferEnabled)
                    {
                        Buffer.AppendLine(line);
                        
                    }


                    //sw.Stop();
                    }
                catch
                {
                    writer = null;
                }

                #region [закомментировано] Обработка, если уровень ошибок выше, чем уровень по настройкам

                /*if (logLevel >= instance.level)
                {
                    // Если файл существует
                    if (File.Exists(fileName))
                    {
                        // Если ограничение по строкам не равно 0
                        if (instance.restrict != 0)
                        {
                            // FIFO
                            Queue<string> queue = new Queue<string>();
                            using (StreamReader sr = new StreamReader(fileName, Encoding.Default))
                            {
                                string line;
                                while ((line = sr.ReadLine()) != null)
                                {
                                    queue.Enqueue(line);
                                    if (queue.Count > instance.restrict)
                                        queue.Dequeue();
                                }
                            }

                            // Удалить файл
                            File.Delete(fileName);

                            // Записать всё содержимое в файл
                            File.WriteAllLines(fileName, queue.ToArray(), Encoding.Default);

                        }

                        // Дописать актуальное сообщение
                        File.AppendAllText(fileName, string.Format("{0} {1}\r\n", date.ToString("dd.MM.yy HH:mm:ss.f"), message),
                            Encoding.Default);

                    }
                    else
                    {
                        // Записать строку в файл
                        File.WriteAllText(fileName, string.Format("{0} {1}\r\n", date.ToString("dd.MM.yy HH:mm:ss.f"), message),
                            Encoding.Default);
                    }
                }*/

                #endregion

            }

            sw.Stop();
            sw.Reset();
            
        }


        /// <summary>
        /// Флаг вывода debug-сообщений (если true, то Debug() и DebugStackTrace() выводят)
        /// </summary>
        public static bool LogDebug { get; set; }

        /// <summary>
        /// Максимальный уровень лога отладки
        /// </summary>
        public static int DebugLevel { get; set; }



        

        

        

       

       
    }

    
}
