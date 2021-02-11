using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMyFood.ServerFunction
{
    public enum ActionsEnum
    {
        //Проверка компьютера на наличие запущенной серверной части
        CheckServer = 0,

        UsersListRetrieving = 1,

        //Обмен информацией с сервером.
        InfoRetrieving = 2
    }
}
