using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vam.Commands
{
    public class CommandList
    {
        public List<string> cmdArguments { get; } = new List<string>() { "--cat" };
    }
}
