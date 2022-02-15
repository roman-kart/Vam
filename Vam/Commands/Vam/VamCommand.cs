using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vam.Files;
using Vam.Commands.Vam;

namespace Vam.Commands
{
    public class VamCommand
    {
        private static int maxWidth = 120;
        private static int maxHeight = 120;
        public static void Do(string path)
        {
            var fullPathToFile = WorkWithFiles.GetPathToFile(path); // получение полного пути до файла
            bool isFileExists = File.Exists(fullPathToFile); // существует ли файл
            // слздаем новый файл, если файла не существует
            if (!isFileExists)
            {
                File.Create(fullPathToFile);
            }

            var content = WorkWithFiles.ReadAllFile(fullPathToFile); // получаем содержимое файла

            var splitContent = content.Split('\n');
            var lengthOfLongestRow = WorkWithFileContent.LengthOfLongestString(splitContent); // определяем максимальную длину строки в исходном файле
            // если необходимо - увеличиваем буфер по горизонтали
            Console.WriteLine(lengthOfLongestRow);
            if (lengthOfLongestRow > Console.BufferWidth)
            {
                var diffBtwMaxLengthAndMaxWidth = maxWidth - lengthOfLongestRow; // разница между максимальной шириной окна и длиной наиболее длинной строки
                Console.WindowWidth = diffBtwMaxLengthAndMaxWidth <= 0 ? maxWidth : maxWidth - diffBtwMaxLengthAndMaxWidth; // устанавливаем размер окна
                Console.BufferWidth = lengthOfLongestRow; // увеличиваем буфер на размер длины максимальной по длине строки
            }
            var startCursorPosition = new { Left = Console.CursorLeft, Top = Console.CursorTop}; // позиция курсора до вывода файла
            
            Console.WriteLine(content); // выводим содержание файла на экран
            var endCursorPosition = new { Left = Console.CursorLeft, Top = Console.CursorTop }; // самая левая позиция курсора после вывода файла
            Console.SetCursorPosition(startCursorPosition.Left, startCursorPosition.Top); // устанавливаем курсор на начало файла
            
            var terminateCommand = new ConsoleKeyInfo('D', ConsoleKey.D, false, false, true); // команда завершения работы - Ctrl + D
            
            var currentKey = Console.ReadKey(true); // принимаем первый введенный пользователем символ
            // пока не подана команда на остановку редактирования, исполняем команды пользователя
            while (currentKey.Modifiers != terminateCommand.Modifiers || currentKey.Key != currentKey.Key)
            {
                // если пользователь нажал на стрелку вверх
                switch (currentKey.Key)
                {
                    case ConsoleKey.Backspace:
                        break;
                    case ConsoleKey.Enter:
                        break;
                    case ConsoleKey.Escape:
                        break;
                    case ConsoleKey.Spacebar:
                        break;
                    case ConsoleKey.LeftArrow:
                        // если курсор не выходит за пределы экрана
                        if (NavigationCheck.IsCursorInBuffer(leftDifference: -1))
                        {
                            Console.CursorLeft -= 1;
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        // если курсор не выходит за пределы экрана
                        if (NavigationCheck.IsCursorInBuffer(topDifference: -1, startTop: startCursorPosition.Top, endTop: endCursorPosition.Top))
                        {
                            Console.CursorTop -= 1;
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        // если курсор не выходит за пределы экрана
                        if (NavigationCheck.IsCursorInBuffer(leftDifference: 1))
                        {
                            Console.CursorLeft += 1;
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        // если курсор не выходит за пределы экрана
                        if (NavigationCheck.IsCursorInBuffer(topDifference: 1, startTop: startCursorPosition.Top, endTop: endCursorPosition.Top))
                        {
                            Console.CursorTop += 1;
                        }
                        break;
                    case ConsoleKey.Insert:
                        break;
                    case ConsoleKey.Delete:
                        break;
                    case ConsoleKey.D0:
                        break;
                    case ConsoleKey.D1:
                        break;
                    case ConsoleKey.D2:
                        break;
                    case ConsoleKey.D3:
                        break;
                    case ConsoleKey.D4:
                        break;
                    case ConsoleKey.D5:
                        break;
                    case ConsoleKey.D6:
                        break;
                    case ConsoleKey.D7:
                        break;
                    case ConsoleKey.D8:
                        break;
                    case ConsoleKey.D9:
                        break;
                    case ConsoleKey.A:
                        break;
                    case ConsoleKey.B:
                        break;
                    case ConsoleKey.C:
                        break;
                    case ConsoleKey.D:
                        break;
                    case ConsoleKey.E:
                        break;
                    case ConsoleKey.F:
                        break;
                    case ConsoleKey.G:
                        break;
                    case ConsoleKey.H:
                        break;
                    case ConsoleKey.I:
                        break;
                    case ConsoleKey.J:
                        break;
                    case ConsoleKey.K:
                        break;
                    case ConsoleKey.L:
                        break;
                    case ConsoleKey.M:
                        break;
                    case ConsoleKey.N:
                        break;
                    case ConsoleKey.O:
                        break;
                    case ConsoleKey.P:
                        break;
                    case ConsoleKey.Q:
                        break;
                    case ConsoleKey.R:
                        break;
                    case ConsoleKey.S:
                        break;
                    case ConsoleKey.T:
                        break;
                    case ConsoleKey.U:
                        break;
                    case ConsoleKey.V:
                        break;
                    case ConsoleKey.W:
                        break;
                    case ConsoleKey.X:
                        break;
                    case ConsoleKey.Y:
                        break;
                    case ConsoleKey.Z:
                        break;
                    case ConsoleKey.Multiply:
                        break;
                    case ConsoleKey.Add:
                        break;
                    case ConsoleKey.Separator:
                        break;
                    case ConsoleKey.Subtract:
                        break;
                    case ConsoleKey.Decimal:
                        break;
                    case ConsoleKey.Divide:
                        break;
                }
                currentKey = Console.ReadKey(true); // получаем следующий введенный пользователем символ
            }
            Console.SetCursorPosition(endCursorPosition.Left, endCursorPosition.Top); // ставим курсор на строку после выведенного текста
        }
    }
}
