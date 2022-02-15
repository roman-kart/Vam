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
                        var fullPathToFile = WorkWithFiles.GetPathToFile(file); // получение полного пути до файла
                        bool fileExists = File.Exists(fullPathToFile); // существует ли файл?
                        if (fileExists)
                        {
                            Console.WriteLine(WorkWithFiles.ReadAllFile(file));
                        }
                    }
                }
                // если была набрана команда --vam
                else if (p.Vam != null)
                {
                    var fullPathToFile = WorkWithFiles.GetPathToFile(p.Vam); // получение полного пути до файла
                    bool isFileExists = File.Exists(fullPathToFile); // существует ли файл?

                }
            });
            //var file = WorkWithFiles.GetFile(pathToFile);
        }
    }
}
