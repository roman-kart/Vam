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
                        var previousRowInList = rowInList - 1;
                        // если при нажатии клавиши BackSpace курсор находился на левом краю консоли и если курсор не находится в первой строке, то необходимо 
                        if (col - 1 < 0 && previousRowInList >= 0)
                        {
                            if (splitContentStringBulder.Count - 1 < rowInList)
                            {
                                Console.CursorLeft = 0;
                                Console.CursorTop -= 1;
                                break;
                            }
                            var currentStr = DeleteEscapeSequenceFromString(splitContentStringBulder[rowInList].ToString());
                            var previousStr = splitContentStringBulder[previousRowInList];
                            var previousStrLen = previousStr.Length;
                            splitContentStringBulder[previousRowInList].Append(DeleteEscapeSequenceFromString(currentStr.ToString())); // переносим содержимое текущей строки на предыдущую строку
                            splitContentStringBulder.RemoveAt(rowInList); // удаляем текущую строку из списка
                            ChangeBufferSizeIfNecessary(splitContentStringBulder[previousRowInList].ToString());
                            RenderRows(previousRowInList, splitContentStringBulder.Count, row - 1, previousStrLen); // перерендериваем все строки
                        }
                        else if (splitContentStringBulder.Count - 1 < rowInList && col - 1 >= 0)
                        {
                            Console.CursorLeft -= 1;
                        }
                        // если индекс находится вне строки (строка короче предыдущей строки), то просто перемещаем курсор пользователя
                        else if (splitContentStringBulder[rowInList].Length - 1 < col - 1 && col - 1 >= 0)
                        {
                            Console.CursorLeft -= 1;
                        }
                        // если существует индекс, идущей перед текущим символом, значит можно его удалить
                        else if (col - 1 >= 0)
                        {
                            splitContentStringBulder[rowInList].Remove(col - 1, 1);
                            RenderRows(rowInList, rowInList + 1, row, col - 1);
                            Console.SetCursorPosition(col - 1 < 0 ? 0 : col - 1, row);
                        }
                        break;
                    case ConsoleKey.Enter:
                        break;
                    case ConsoleKey.Escape:
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
                    case ConsoleKey.Spacebar:
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
                    case ConsoleKey.Multiply:
                    case ConsoleKey.Add:
                    case ConsoleKey.Separator:
                    case ConsoleKey.Subtract:
                    case ConsoleKey.Decimal:
                    case ConsoleKey.Divide:
                        row = Console.CursorTop;
                        col = Console.CursorLeft;
                        rowInList = row - startCursorPosition.Top;
                        var currentRowStrBld = splitContentStringBulder[rowInList];
                        // если символ необходимо вставить внутри строки
                        if (col < currentRowStrBld.Length)
                        {
                            currentRowStrBld.Insert(col, currentKey.KeyChar);
                        }
                        else
                        {
                            var distanceBetweenLastSymbolAndCurrentPosition = col - currentRowStrBld.Length;
                            for (int i = 0; i < distanceBetweenLastSymbolAndCurrentPosition; i++)
                            {
                                currentRowStrBld.Append(' ');
                            }
                            currentRowStrBld.Append(currentKey.KeyChar);
                        }
                        ChangeBufferSizeIfNecessary(currentRowStrBld.ToString());
                        RenderRows(rowInList, splitContentStringBulder.Count, row, col + 2);
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
        private static string GetOnlySpacebarsRow()
        {
            var result = new StringBuilder();
            for (int i = 0; i < Console.BufferWidth; i++)
            {
                result.Append(" ");
            }
            return result.ToString();
        }
        /// <summary>
        /// Увеличивает ширину буфера, если переданная в качестве аргумента строка больше, чем текущая ширина.
        /// </summary>
        private static void ChangeBufferSizeIfNecessary(string row)
        {
            if (row.Length > Console.BufferWidth)
            {
                Console.BufferWidth = row.Length;
            }
        }
        private static void RenderRows(int startIndex, int endIndex, int startTop, int startLeft)
        {
            int currentTop = startTop;
            for (int i = startIndex; i < endIndex; i++)
            {
                Console.CursorLeft = 0;
                Console.CursorTop = currentTop;
                Console.Write(GetOnlySpacebarsRow());

                Console.CursorLeft = 0;
                Console.CursorTop = currentTop;
                // выводим строку-заполнитель, если закончились строки в массиве
                if (i >= splitContentStringBulder.Count)
                {
                    Console.Write(GetOnlySpacebarsRow());
                }
                // в противном случае выводим строку из массива
                else
                {
                    Console.Write(DeleteEscapeSequenceFromString(splitContentStringBulder[i].ToString()));
                }
                currentTop++;
            }
            startLeft = startLeft - 1 < 0 ? startLeft : startLeft - 1;
            Console.SetCursorPosition(startLeft, startTop);
        }
        private static string DeleteEscapeSequenceFromString(string source)
        {
            return source.Replace("\n", "").Replace("\r", "");
        }
    }
}
