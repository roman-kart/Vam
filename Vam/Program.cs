using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Vam.UI;
using Vam.Files;

namespace Vam
{
    internal class Program
    {
        static void Main(string[] args)
        {
            #region Is File Argument Valid
            // если пользователь не указал файл, то выводим сообщение об ошибке и завершаем работу программы
            if (args.Length == 0)
            {
                Messages.WriteError("Please, write a file name with extension. (Example: Vam.exe testText.txt)");
                Messages.WriteREADME();
                return;
            }
            // если пользователь хочет открыть более одного файла
            else if (args.Length > 1)
            {
                Messages.WriteError("Please, write one file name.");
                Messages.WriteREADME();
                return;
            }
            #endregion

            #region GetPathToFile
            var userPathToFile = args[0];
            var pathToFile = WorkWithFiles.GetPathToFile(userPathToFile);
            Console.WriteLine($"Path to file: {pathToFile}");
            #endregion

            #region Is Command Argument Valid

            #endregion
            //var file = WorkWithFiles.GetFile(pathToFile);
        }
    }
}
