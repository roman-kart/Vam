using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CommandLine;
using Vam.UI;
using Vam.Files;
using Vam.Commands;

namespace Vam
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var result = CommandLine.Parser.Default.ParseArguments<OptionsCommandLine>(args);
            result.WithParsed(p =>
            {
                // если была набрана команда --cat
                if (p.Cat.Count() > 0)
                {
                    foreach (var file in p.Cat)
                    {
                        CatCommand.Do(file);
                    }
                }
                // если была набрана команда --vam
                else if (p.Vam != null)
                {
                    VamCommand.Do(p.Vam);
                }
            });
            //var file = WorkWithFiles.GetFile(pathToFile);
        }
    }
}
