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
        private static List<StringBuilder> splitContentStringBulder = new List<StringBuilder>();
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
            // для удобства редактирвоания текста конвертируем все строки в StringBuilder
            foreach (var row in splitContent)
            {
                splitContentStringBulder.Add(new StringBuilder(row));
            }

            var lengthOfLongestRow = WorkWithFileContent.LengthOfLongestString(splitContent); // определяем максимальную длину строки в исходном файле
            // если необходимо - увеличиваем буфер по горизонтали
            if (lengthOfLongestRow > Console.BufferWidth)
            {
                var diffBtwMaxLengthAndMaxWidth = maxWidth - lengthOfLongestRow; // разница между максимальной шириной окна и длиной наиболее длинной строки
                Console.WindowWidth = diffBtwMaxLengthAndMaxWidth <= 0 ? maxWidth : maxWidth - diffBtwMaxLengthAndMaxWidth; // устанавливаем размер окна
                Console.BufferWidth = lengthOfLongestRow; // увеличиваем буфер на размер длины максимальной по длине строки
            }
            var startCursorPosition = new { Left = Console.CursorLeft, Top = Console.CursorTop}; // позиция курсора до вывода файла
            
            Console.WriteLine(content); // выводим содержание файла на экран
            var endCursorPosition = new { Left = Console.CursorLeft, Top = Console.CursorTop }; // самая левая позиция курсора после вывода файла
            Console.WindowHeight += 1; // увеличиваем высоту окна на 1 линию (иначе ломается)
            Console.SetCursorPosition(startCursorPosition.Left, startCursorPosition.Top); // устанавливаем курсор на начало файла
            
            var terminateCommand = new ConsoleKeyInfo('D', ConsoleKey.D, false, false, true); // команда завершения работы - Ctrl + D
            
            var currentKey = Console.ReadKey(true); // принимаем первый введенный пользователем символ
            // пока не подана команда на остановку редактирования, исполняем команды пользователя
            while (currentKey.Modifiers != terminateCommand.Modifiers || terminateCommand.Key != currentKey.Key)
            {
                switch (currentKey.Key)
                {
                    case ConsoleKey.Backspace:
                        var row = Console.CursorTop;
                        var col = Console.CursorLeft;
                        var rowInList = row - startCursorPosition.Top;
                        // если индекс находится вне строки (строка короче предыдущей строки), то просто перемещаем курсор пользователя
                        if (splitContentStringBulder[rowInList].Length - 1 < col - 1 && col - 1 >= 0)
                        {
                            Console.CursorLeft -= 1;
                        }
                        // если существует индекс, идущей перед текущим символом, значит можно его удалить
                        else if (col - 1 >= 0)
                        {
                            splitContentStringBulder[rowInList].Remove(col - 1, 1);
                            Console.CursorLeft = 0;
                            Console.CursorTop = row;
                            Console.Write(GetOnlySpacebarsString());
                            Console.CursorLeft = 0;
                            Console.CursorTop = row;
                            Console.Write(splitContentStringBulder[rowInList]);
                            Console.SetCursorPosition(col - 1 < 0 ? 0 : col - 1, row);
                        }
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
                    case ConsoleKey.D1:
                    case ConsoleKey.D2:
                    case ConsoleKey.D3:
                    case ConsoleKey.D4:
                    case ConsoleKey.D5:
                    case ConsoleKey.D6:
                    case ConsoleKey.D7:
                    case ConsoleKey.D8:
                    case ConsoleKey.D9:
                    case ConsoleKey.A:
                    case ConsoleKey.B:
                    case ConsoleKey.C:
                    case ConsoleKey.D:
                    case ConsoleKey.E:
                    case ConsoleKey.F:
                    case ConsoleKey.G:
                    case ConsoleKey.H:
                    case ConsoleKey.I:
                    case ConsoleKey.J:
                    case ConsoleKey.K:
                    case ConsoleKey.L:
                    case ConsoleKey.M:
                    case ConsoleKey.N:
                    case ConsoleKey.O:
                    case ConsoleKey.P:
                    case ConsoleKey.Q:
                    case ConsoleKey.R:
                    case ConsoleKey.S:
                    case ConsoleKey.T:
                    case ConsoleKey.U:
                    case ConsoleKey.V:
                    case ConsoleKey.W:
                    case ConsoleKey.X:
                    case ConsoleKey.Y:
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
        /// <summary>
        /// Возвращает строку-заполнитель, состоящую из пробелов.
        /// Применяется для очищения экрана перед показом обновленной строки.
        /// </summary>
        /// <returns></returns>
        private static string GetOnlySpacebarsString()
        {
            var result = new StringBuilder();
            for (int i = 0; i < Console.BufferWidth; i++)
            {
                result.Append(" ");
            }
            return result.ToString();
        }
    }
}
