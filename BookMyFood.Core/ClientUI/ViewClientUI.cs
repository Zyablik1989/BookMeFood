using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMyFood.ClientUI
{
    public interface IViewClientUI
    {
        /// <summary>
        /// Надпись призывающая написать в чат
        /// </summary>
        string tbChatMessageSend { get; set; }
        
        /// <summary>
        /// Надпись рядом режимом редактирования
        /// </summary>
        string EditingModeRadioButtonLabel { get; set; }
        /// <summary>
        /// Надпись рядом режимом подключения к лидеру
        /// </summary>
        string JoiningModeRadioButtonLabel { get; set; }
        /// <summary>
        /// Надпись рядом режимом ведения заказа
        /// </summary>
        string LeadModeRadioButtonLabel { get; set; }

        /// <summary>
        /// Надпись рядом режимом ведения заказа
        /// </summary>
        string tbServersListFieldLabel { get; set; }
        

        //LeaderAddressOrIP

        /// <summary>
        /// Надпись рядом с адресом лидера
        /// </summary>
        string LeaderAddressFieldLabel { get; set; }
        /// <summary>
        /// Надпись рядом с именем текущего пользователя
        /// </summary>
        string YourNameFieldLabel { get; set; }
        
        /// /// <summary>
        /// Надпись рядом полем для ввода порта
        /// </summary>
        string PortFieldLabel { get; set; }

        /// /// <summary>
        /// Надпись рядом полем для выбора языка
        /// </summary>
        string LanguageFieldLabel { get; set; }

        /// /// <summary>
        /// Надпись рядом экспандером
        /// </summary>
        string ExpanderLabel { get; set; }


    }
}
