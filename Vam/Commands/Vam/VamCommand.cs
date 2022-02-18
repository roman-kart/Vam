using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vam.Files;
using Vam.Commands.Vam;
using System.Threading;

namespace Vam.Commands
{
    /// <summary>
    /// Содержит методы для реализации функционала блокнота в консоли.
    /// Все методы статические, так как предполагается, что не понадобится вызывать более одного метода.
    /// </summary>
    public static class VamCommand
    {
        /// <summary>
        /// Содержимое файла, поделенное на строки. Каждая строка хранится в отдельном экземпляре StringBuilder
        /// </summary>
        private static List<StringBuilder> splitContentStringBulder = new List<StringBuilder>();
        /// <summary>
        /// Максимальная ширина терминала
        /// </summary>
        private static int maxWidth = 120;
        /// <summary>
        /// Максимальная высота терминала
        /// </summary>
        private static int maxHeight = 120;
        /// <summary>
        /// Полный путь до файла
        /// </summary>
        private static string fullPathToFile;
        /// <summary>
        /// Длина самой длинной строки в текущем файле.
        /// </summary>
        private static int lengthOfLongestRow;
        public static void Do(string path)
        {
            #region получение пути файла и содержимого файла
            fullPathToFile = WorkWithFiles.CreateNewFileIfNecessaryThenGetFullPath(path); // получаем полный путь до файла
            var content = WorkWithFiles.ReadAllFile(fullPathToFile); // получаем содержимое файла
            #endregion

            #region форматирование исходных данных
            var splitContent = content.Split('\n'); // получаем отдельные строки вместо одной строки
            // для удобства редактирвоания текста конвертируем все строки в StringBuilder
            foreach (var row in splitContent)
            {
                splitContentStringBulder.Add(new StringBuilder(row));
            }
            #endregion

            lengthOfLongestRow = WorkWithFileContent.LengthOfLongestString(splitContent); // определяем максимальную длину строки в исходном файле
            ChangeBufferSizeIfNecessary(lengthOfLongestRow);
            var startCursorPosition = new { Left = Console.CursorLeft, Top = Console.CursorTop}; // позиция курсора до вывода файла

            RenderRows(0, splitContentStringBulder.Count, startCursorPosition.Top, 0);
            var endCursorPosition = new { Left = 0, Top = startCursorPosition.Top + splitContentStringBulder.Count}; // самая левая позиция курсора после вывода файла
            Console.WindowHeight += 1; // увеличиваем высоту окна на 1 линию (иначе ломается)
            Console.SetCursorPosition(startCursorPosition.Left, startCursorPosition.Top); // устанавливаем курсор на начало файла
            
            var terminateCommand = new ConsoleKeyInfo('D', ConsoleKey.D, false, false, true); // команда завершения работы - Ctrl + D
            
            var currentKey = Console.ReadKey(true); // принимаем первый введенный пользователем символ
            // пока не подана команда на остановку редактирования, исполняем команды пользователя
            while (currentKey.Modifiers != terminateCommand.Modifiers || terminateCommand.Key != currentKey.Key)
            {
                // управляющие команды:
                if (currentKey.Modifiers == ConsoleModifiers.Control && currentKey.Key == ConsoleKey.S)
                {
                    SaveFile(fullPathToFile);
                }
                // если не является управляющей командой
                else
                {
                    var row = Console.CursorTop;
                    var col = Console.CursorLeft;
                    var rowInList = row - startCursorPosition.Top;
                    var previousRowInList = rowInList - 1;
                    switch (currentKey.Key)
                    {
                        case ConsoleKey.Backspace:
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
                                endCursorPosition = new { Left = endCursorPosition.Left, Top = endCursorPosition.Top - 1 };
                                ChangeBufferSizeIfNecessary(splitContentStringBulder[previousRowInList].ToString());
                                RenderRows(previousRowInList, splitContentStringBulder.Count + 1, row - 1, previousStrLen); // перерендериваем все строки
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
                            // если курсор ниже предела текущего массива - создаем новые строчки до этого положения
                            if (rowInList > splitContentStringBulder.Count - 1)
                            {
                                var countOfOnlySpacesRowsBeforeNewRow = rowInList - (splitContentStringBulder.Count - 1);
                                // вставляем пустые строки до новой строки + новую строку
                                InsertSpacesRowsToSplitRowsList(splitContentStringBulder.Count - 1, countOfOnlySpacesRowsBeforeNewRow + 1);
                            }
                            // если курсор находится в пределах текущего массива строк - переносим содержимое (если есть) справа от курсора на новую строку
                            else if (rowInList >= 0 && rowInList <= splitContentStringBulder.Count - 1)
                            {
                                var currentStrBld = splitContentStringBulder[rowInList];
                                var newRowStrBld = new StringBuilder();
                                // если курсор находится вне пределов текущей строчки, то на новую строку ничего не переносим
                                if (col > currentStrBld.Length - 1)
                                {
                                    // не изменяем новую строку
                                }
                                else
                                {
                                    var newRowStr = currentStrBld.ToString().Substring(col);
                                    currentStrBld = currentStrBld.Remove(col, currentStrBld.Length - col);
                                    newRowStr = DeleteEscapeSequenceFromString(newRowStr);
                                    newRowStrBld.Append(newRowStr);
                                }
                                splitContentStringBulder.Insert(rowInList + 1, newRowStrBld);
                                endCursorPosition = new { Left = endCursorPosition.Left, Top = endCursorPosition.Top + 1 };
                                RenderRows(rowInList, splitContentStringBulder.Count, row, 0);
                                Console.CursorTop++;
                            }
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

                        // функциональные клавиши, для которых пока нет реализации

                        case ConsoleKey.Insert:
                        case ConsoleKey.Delete:
                        case ConsoleKey.Escape:
                        case ConsoleKey.Applications:
                        case ConsoleKey.Attention:
                        case ConsoleKey.BrowserBack:
                        case ConsoleKey.BrowserForward:
                        case ConsoleKey.BrowserHome:
                        case ConsoleKey.BrowserFavorites:
                        case ConsoleKey.BrowserRefresh:
                        case ConsoleKey.BrowserSearch:
                        case ConsoleKey.BrowserStop:
                        case ConsoleKey.Clear:
                        case ConsoleKey.CrSel:
                        case ConsoleKey.EraseEndOfFile:
                        case ConsoleKey.Execute:
                        case ConsoleKey.ExSel:
                        case ConsoleKey.Tab:
                            break;

                        // для всех остальных клавишь просто добавляем значение символа в текущую строку
                        default:
                            // добавляем пустые строки, если пользователь ввел симвом вне пределов массива строк
                            if (rowInList > splitContentStringBulder.Count - 1)
                            {
                                var countOfOnlySpacesRowsBeforeNewRow = rowInList - (splitContentStringBulder.Count - 1);
                                // вставляем пустые строки до новой строки + новую строку
                                InsertSpacesRowsToSplitRowsList(splitContentStringBulder.Count - 1, countOfOnlySpacesRowsBeforeNewRow + 1);
                            }
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
                            // (пока рендеим только одну) если размер буфера был увеличен - рендерим все строчки
                            if (ChangeBufferSizeIfNecessary(currentRowStrBld.ToString()))
                            {
                                RenderRows(0, splitContentStringBulder.Count, 0, 0);
                                Console.CursorTop = row;
                                Console.CursorLeft = col + 1;
                            }
                            // иначе только одну
                            else
                            {
                                RenderRows(rowInList, rowInList + 1, row, col + 2); // рендерим данную строку
                            }
                            break;
                    }
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
        /// true - если размер был увеличен, false - если остался прежним
        /// </summary>
        private static bool ChangeBufferSizeIfNecessary(string row)
        {
            return ChangeBufferSizeIfNecessary(row.Length);
        }
        /// <summary>
        /// Увеличивает ширину буфера, если переданная в качестве аргумента длина строки больше, чем текущая ширина.
        /// true - если размер был увеличен, false - если остался прежним
        /// </summary>
        private static bool ChangeBufferSizeIfNecessary(int rowLength)
        {
            if (rowLength > Console.BufferWidth)
            {
                Console.BufferWidth = rowLength;
                return true;
            }
            return false;
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
            startLeft = startLeft - 1 < 0 ? startLeft : startLeft - 1; // курсор перемещается влево, так как это было необходимо для Backspace
            Console.SetCursorPosition(startLeft, startTop);
        }
        private static string DeleteEscapeSequenceFromString(string source)
        {
            return source.Replace("\n", "").Replace("\r", "");
        }
        /// <summary>
        /// Добавляет в указанном диапазоне пустые строки в массив строк.
        /// startIndex - включительно.
        /// endIndex - не включительно.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        private static void InsertSpacesRowsToSplitRowsList(int startIndex, int count)
        {
            var onlySpacesString = GetOnlySpacebarsRow();
            var onlySpacesStringsList = new List<StringBuilder>();
            for (int i = 0; i < count; i++)
            {
                onlySpacesStringsList.Add(new StringBuilder(onlySpacesString));
            }
            splitContentStringBulder.InsertRange(startIndex, onlySpacesStringsList);
        }
        private static string GetTextForWriteToFile(List<StringBuilder> source)
        {
            string result = "";
            foreach (var line in source)
            {
                result += line.ToString() + "\n";
            }
            return result;
        }
        private static void SaveFile(string path)
        {
            using (var file = new StreamWriter(path))
            {
                file.Write(GetTextForWriteToFile(splitContentStringBulder));
            }
        }
    }
}
