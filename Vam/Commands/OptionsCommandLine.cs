using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Vam.Commands
{
    public class OptionsCommandLine
    {
        [Option(shortName: 'v', longName: "vam", Group = "commands", HelpText = "Start simple console editor.")]
        public IEnumerable<string> Vam { get; set; }
        [Option(shortName: 'c', longName: "cat", Group = "commands", HelpText = "Write all content from files")]
        public IEnumerable<string> Cat { get; set; }
    }
}
