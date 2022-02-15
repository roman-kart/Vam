using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vam.UI;
using Vam.Files;
using Vam.Commands;
namespace Vam.Commands
{
    public class CatCommand
    {
        /// <summary>
        /// Выводит на экран содержимое файла в виде текста
        /// </summary>
        /// <param name="file"></param>
        public static void Do(string file)
        {
            var fullPathToFile = WorkWithFiles.GetPathToFile(file); // получение полного пути до файла
            bool fileExists = File.Exists(fullPathToFile); // существует ли файл?
            if (fileExists)
            {
                Console.WriteLine(WorkWithFiles.ReadAllFile(file));
            }
        }
    }
}
