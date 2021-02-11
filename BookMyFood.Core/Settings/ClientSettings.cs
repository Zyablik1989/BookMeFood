using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;

namespace BookMyFood.Settings
{
   public class ClientSettings
   {
       /// <summary>
       /// Имя пользователя, который собирает заказ, к нему должны подключаться остальные.
       /// </summary>
       public string LeaderName { get; set; }

       public string YourNameLabel { get; set; }

       
       public static ClientSettings Current (ClientSettings instance) => instance ?? new ClientSettings();

       /// <summary>
        /// Язык интерфейса
        /// </summary>
        private string Locale;
       public void SetLocale(string locale)
       {

           try
           {
               Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(locale);
               LeaderName = Properties.strings.LeaderAddressFieldLabel;


                if (LocaleChanged!=null)
               LocaleChanged(locale);
           }
           catch (Exception) { }
        }
       public class LocaleEventArgs
        {
            public string _locale { get; }

            public LocaleEventArgs(string locale)
            {
                _locale = locale;
            }
       }
       public delegate void LocaleHandler(string locale);
       public event LocaleHandler LocaleChanged;

        public ClientSettings(
            string leaderName="", 
            string locale = "en-US"
            
            )
       {

           try
           {
               Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentCulture;
           }
            catch (Exception){}

                LeaderName = Properties.strings.LeaderAddressFieldLabel;

            if (!string.IsNullOrEmpty(leaderName))
            { LeaderName = leaderName;}


        }
       
        
   }
}
