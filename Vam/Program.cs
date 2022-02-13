using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vam.UI;
using Vam.Files;

namespace Vam
{
    internal class Program
    {
        static void Main(string[] args)
        {
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

        }
    }
}
