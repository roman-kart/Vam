using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vam.UI
{
    /// <summary>
    /// Класс, в котором находятся методы для вывода сообщений для пользователя
    /// </summary>
    public class Messages
    {
        /// <summary>
        /// Вывод сообщения об ошибке в консоль. Вывыодит символы красного цвета.
        /// </summary>
        /// <param name="message"></param>
        public static void WriteError(string message)
        {
            var originalForeign = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = originalForeign;
        }
        /// <summary>
        /// Выводит краткую интрукцию по исползованию программы.
        /// </summary>
        public static void WriteREADME()
        {
            Console.WriteLine($"Usage: Vam.exe [file]");
        }
    }
}
