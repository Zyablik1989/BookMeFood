using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookMyFood.Settings;
using BookMyFood.ClientUI;

namespace BookMyFood.ClientUI
{
    
   public class ClientUIPresenter
    {
        private IViewClientUI ClientUi;
        delegate string ResourceName(string x);

        public ClientUIPresenter(IViewClientUI iClientUi)
        {
            ClientUi = iClientUi;
        }

        public static string GetString(string res)
        {
          return  ResourceHandler.GetResource(res);
        }

        
        public void FillUpTextFields()
        {
            ResourceName r = (x) => ResourceHandler.GetResource(x);

            ClientUi.tbChatMessageSend = r("tbChatMessageSend");
            ClientUi.tbServersListFieldLabel = r("tbServersListFieldLabel");
            ClientUi.EditingModeRadioButtonLabel = r("EditingModeRadioButtonLabel");
            ClientUi.ExpanderLabel = r("ExpanderLabel");
            ClientUi.JoiningModeRadioButtonLabel = r("JoiningModeRadioButtonLabel");
            ClientUi.LanguageFieldLabel = r("LanguageFieldLabel");
            ClientUi.LeaderAddressFieldLabel = r("LeaderAddressFieldLabel");
            ClientUi.LeadModeRadioButtonLabel = r("LeadModeRadioButtonLabel");
            ClientUi.PortFieldLabel = r("PortFieldLabel");
            ClientUi.YourNameFieldLabel = r("YourNameFieldLabel");

            //    public void LeaderAddressFieldLabel { LeaderAddressFieldLabel = r("YourNameLabel"); }
            //public void YourNameFieldLabel { get; set; }

            //     = r("LeaderName");
            //        LeaderAddressFieldLabel = r("YourNameLabel");
            //        cbLocale.DataSource = new List<string> { "ru-RU", "en-US", "ms-MY" };
        }


    }
}
