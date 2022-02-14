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
                if (p.Cat.Count() > 0)
                {
                    Console.WriteLine("Cat files: ");
                    foreach (var file in p.Cat)
                    {
                        Console.WriteLine(file);
                    }
                }
                else if (p.Vam.Count() > 0)
                {
                    Console.WriteLine("Vam files: ");
                    foreach (var file in p.Vam)
                    {
                        Console.WriteLine(file);
                    }
                }
            });
            #region Is File Argument Valid
            // если пользователь не указал файл, то выводим сообщение об ошибке и завершаем работу программы
            if (args.Length == 0)
            {
                Messages.WriteError("Please, write a file name with extension. (Example: Vam.exe testText.txt)");
                Messages.WriteREADME();
                return;
            }
            #endregion

            #region GetPathToFile
            var userPathToFile = args[0];
            var pathToFile = WorkWithFiles.GetPathToFile(userPathToFile);
            //Console.WriteLine($"Path to file: {pathToFile}");
            #endregion

            #region Is Command Argument Valid
            #endregion
            //var file = WorkWithFiles.GetFile(pathToFile);
        }
    }
}
