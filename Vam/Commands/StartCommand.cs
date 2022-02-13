using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vam.Files;

namespace Vam.Commands
{
    public class StarterForCommand
    {
        public static void SelectAndStartCommand(UserFile file, string commandArg)
        {
            Console.WriteLine(file.Path);
            Console.WriteLine(commandArg);
        }
    }
}
