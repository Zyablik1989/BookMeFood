using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookMyFood.ClientUI
{
    public class ResourceHandler
    {
        public static string GetResource(string name)
        {
            return Properties.strings.ResourceManager.GetString(name);

        }
    }
}
