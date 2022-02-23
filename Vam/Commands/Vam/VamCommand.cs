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
        private static CursorPosition startCursorPosition;
        private static CursorPosition endCursorPosition;
        private static ConsoleKeyInfo terminateCommand = new ConsoleKeyInfo('D', ConsoleKey.D, false, false, true); // команда завершения работы - Ctrl + D
        private static int _lastIndexInSequenceOfRows => splitContentStringBulder.Count - 1;
        private static int _countOfRowsInSequenceOfRows => splitContentStringBulder.Count;
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
                splitContentStringBulder.Add(new StringBuilder(row.TrimEnd()));
            }
            #endregion

            #region первоначальная настройка окна консоли
            lengthOfLongestRow = WorkWithFileContent.LengthOfLongestString(splitContent); // определяем максимальную длину строки в исходном файле
            ChangeBufferSizeIfNecessary(2048); // маловероятно, что строка кода будет превышать длину в 2048 символов
            startCursorPosition = new CursorPosition() { Left = Console.CursorLeft, Top = Console.CursorTop }; // позиция курсора до вывода файла
            endCursorPosition = new CursorPosition() { Left = 0, Top = startCursorPosition.Top + _countOfRowsInSequenceOfRows }; // самая левая позиция курсора после вывода файла
            #endregion

            #region выводим содержимое файла на экран
            RenderRows(0, _countOfRowsInSequenceOfRows, startCursorPosition.Top, 0, 0);
            #endregion

            #region установка размера окна и положения курсора
            Console.WindowHeight += 1; // увеличиваем высоту окна на 1 линию (иначе ломается)
            Console.SetCursorPosition(startCursorPosition.Left, startCursorPosition.Top); // устанавливаем курсор на начало файла
            #endregion
            
            var currentKey = Console.ReadKey(true); // принимаем первый введенный пользователем символ
            // пока не подана команда на остановку редактирования, исполняем команды пользователя
            while (currentKey.Modifiers != terminateCommand.Modifiers || terminateCommand.Key != currentKey.Key)
            {
                // управляющие команды:
                if (currentKey.Modifiers == ConsoleModifiers.Control && currentKey.Key == ConsoleKey.S)
                {
                    SaveFile(fullPathToFile);
                }
                // пропускаем, если нажата клавиша Windows
                else if (currentKey.Key == ConsoleKey.LeftWindows || currentKey.Key == ConsoleKey.RightWindows)
                {
                    // пропуск
                }
                // если не является управляющей командой
                else
                {
                    var row = Console.CursorTop;
                    var col = Console.CursorLeft;
                    var rowInList = row - startCursorPosition.Top;
                    var lastRowIndex = startCursorPosition.Top + splitContentStringBulder.Count - 1;
                    var previousRowInList = rowInList - 1;
                    switch (currentKey.Key)
                    {
                        case ConsoleKey.Backspace:
                            // если при нажатии клавиши BackSpace курсор находился на левом краю консоли и если курсор не находится в первой строке, то необходимо перенести данную строку на следующую строку
                            if (col - 1 < 0 && previousRowInList >= 0)
                            {
                                // если курсор находится за пределами строк - просто перемещаем курсор влево
                                if (_lastIndexInSequenceOfRows < rowInList)
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
                                endCursorPosition = new CursorPosition() { Left = endCursorPosition.Left, Top = endCursorPosition.Top - 1 };
                                ChangeBufferSizeIfNecessary(splitContentStringBulder[previousRowInList].ToString());
                                //RenderRows(previousRowInList, _countOfRowsInSequenceOfRows + 1, row - 1, previousStrLen, 0); // перерендериваем все строки
                                // копируем все неизмененные строки на одну строку ниже
                                // если можем что-либо перенести на следующую строку
                                if (_countOfRowsInSequenceOfRows - (rowInList + 1) >= 0)
                                {
                                    // переносим
                                    Console.MoveBufferArea(0, row + 1, lengthOfLongestRow, _countOfRowsInSequenceOfRows - (rowInList + 1), 0, row);
                                }
                                RenderRows(rowInList - 1, rowInList + 1, row - 1, previousStrLen, 0); // рендерим измененные строки
                            }
                            else if (_lastIndexInSequenceOfRows < rowInList && col - 1 >= 0)
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
                                RenderRows(rowInList, rowInList + 1, row, col, -2);
                                Console.SetCursorPosition(col - 1 < 0 ? 0 : col - 1, row);
                            }
                            break;
                        case ConsoleKey.Enter:
                            // если курсор ниже предела текущего массива - создаем новые строчки до этого положения
                            if (rowInList > _lastIndexInSequenceOfRows)
                            {
                                var countOfOnlySpacesRowsBeforeNewRow = rowInList - (_lastIndexInSequenceOfRows);
                                // вставляем пустые строки до новой строки + новую строку
                                InsertSpacesRowsToSplitRowsList(_lastIndexInSequenceOfRows, countOfOnlySpacesRowsBeforeNewRow + 1);
                            }
                            // если курсор находится в пределах текущего массива строк - переносим содержимое (если есть) справа от курсора на новую строку
                            else if (rowInList >= 0 && rowInList <= _lastIndexInSequenceOfRows)
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
                                endCursorPosition =  new CursorPosition() { Left = endCursorPosition.Left, Top = endCursorPosition.Top + 1 };
                                // копируем все неизмененные строки на одну строку ниже
                                Console.MoveBufferArea(0, row + 1, lengthOfLongestRow, _countOfRowsInSequenceOfRows - rowInList, 0, row + 2);
                                RenderRows(rowInList, rowInList + 2, row, 0, 0); // рендерим измененные строки
                                Console.CursorTop++;
                            }
                            break;
                        case ConsoleKey.LeftArrow:
                            //// если курсор не выходит за пределы экрана
                            //if (NavigationCheck.IsCursorInBuffer(leftDifference: -1))
                            //{
                            //    Console.CursorLeft -= 1;
                            //}
                            ChangeCursorPositionHorizontal.Do(-1, splitContentStringBulder, rowInList, col);
                            break;
                        case ConsoleKey.UpArrow:
                            // если курсор не выходит за пределы экрана
                            if (NavigationCheck.IsCursorInBuffer(topDifference: -1, startTop: startCursorPosition.Top, endTop: lastRowIndex))
                            {
                                Console.CursorTop -= 1;
                            }
                            break;
                        case ConsoleKey.RightArrow:
                            //// если курсор не выходит за пределы экрана
                            //if (NavigationCheck.IsCursorInBuffer(leftDifference: 1))
                            //{
                            //    Console.CursorLeft += 1;
                            //}
                            ChangeCursorPositionHorizontal.Do(+1, splitContentStringBulder, rowInList, col);
                            break;
                        case ConsoleKey.DownArrow:
                            // если курсор не выходит за пределы экрана
                            if (NavigationCheck.IsCursorInBuffer(topDifference: 1, startTop: startCursorPosition.Top, endTop: lastRowIndex))
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
                            break;

                        // клавиши, которые добавляют несколько символов:
                        case ConsoleKey.Tab:
                            AddOnlySpacesRowsIfNecessary(_lastIndexInSequenceOfRows, rowInList);
                            var currentRowStrBldTmp = splitContentStringBulder[rowInList];
                            InsertSequenceOfSymbolsToStringBuilderWithCheck(currentRowStrBldTmp, col, col, ' ', ' ', ' ', ' ');
                            ChangeBufferSizeIfNecessaryThenRenderRowsAndSetCursorPosition(currentRowStrBldTmp.ToString(), rowInList, new CursorPosition() { Top = row, Left = col });
                            Console.CursorLeft += 3;
                            break;

                        // для всех остальных клавишь просто добавляем значение символа в текущую строку
                        default:
                            var cl = Console.CursorLeft;
                            var ct = Console.CursorTop;
                            var cr = splitContentStringBulder[rowInList];
                            var crl = cr.Length;
                            // добавляем пустые строки, если пользователь ввел симвом вне пределов массива строк
                            AddOnlySpacesRowsIfNecessary(_lastIndexInSequenceOfRows, rowInList);
                            var currentRowStrBld = splitContentStringBulder[rowInList];
                            InsertSequenceOfSymbolsToStringBuilderWithCheck(currentRowStrBld, col, col, currentKey.KeyChar);
                            ChangeBufferSizeIfNecessaryThenRenderRowsAndSetCursorPosition(currentRowStrBld.ToString(), rowInList, new CursorPosition() { Top = row, Left = col });
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
        private static string GetOnlySpacebarsRow(int length = -1)
        {
            length = length < 0 ? Console.BufferWidth : length;
            var result = new StringBuilder();
            for (int i = 0; i < length; i++)
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
                Console.BufferWidth = rowLength * 2;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Выводит строки на экран и устанавливает курсор.
        /// previousLength и isDelete предназначены для оптимизации. 
        /// previousLength - длина предыдущей версии строки. По умолчанию перед выводом строки на экран выводится пустая строка,
        /// длина которой равна ширине буфера. 
        /// isDelete - произошла ли в строке операция удаления. 
        /// Если не произошла - просто выводим новую строку, так как её длина увеличилась.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="startTop"></param>
        /// <param name="startLeft"></param>
        /// <param name="offset"></param>
        private static void RenderRows(int startIndex, int endIndex, int startTop, int startLeft, int offset, int previousRowLength = -1, bool isDelete = true)
        {
            previousRowLength = previousRowLength < 0 ? Console.BufferWidth : previousRowLength; // устанавливаем валидное значение
            int currentTop = startTop;
            for (int i = startIndex; i < endIndex; i++)
            {
                // если произошла операция удаления - перед рендерингом строки выводим пустую строку
                Console.CursorLeft = 0;
                Console.CursorTop = currentTop;
                Console.Write(GetOnlySpacebarsRow(previousRowLength));

                Console.CursorLeft = 0;
                Console.CursorTop = currentTop;
                // выводим строку-заполнитель, если закончились строки в массиве
                if (i >= _countOfRowsInSequenceOfRows)
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
            startLeft = startLeft + offset < 0 ? startLeft : startLeft + offset; // курсор перемещается на определенное пользователем расстояние, если он не выходит за пределы экрана
            Console.SetCursorPosition(startLeft, startTop);
        }
        /// <summary>
        /// Удаляет escape-последовательности из строки.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static string DeleteEscapeSequenceFromString(string source)
        {
            return source.Replace("\n", "").Replace("\r", "");
        }
        /// <summary>
        /// Добавляет в указанном диапазоне пустые строки в массив строк.
        /// startIndex - включительно.
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
        /// <summary>
        /// Форматирует текст для записи в файл
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static string GetTextForWriteToFile(List<StringBuilder> source)
        {
            string result = "";
            foreach (var line in source)
            {
                result += line.ToString() + "\n";
            }
            return result;
        }
        /// <summary>
        /// Сохраняет файл, расположенный по определенному пути
        /// </summary>
        /// <param name="path"></param>
        private static void SaveFile(string path)
        {
            using (var file = new StreamWriter(path))
            {
                file.Write(GetTextForWriteToFile(splitContentStringBulder));
            }
        }
        /// <summary>
        /// Добавляет новые пустые строки, если indexOfCurrentRowInSequence выходит за пределы последовательности строк.
        /// </summary>
        /// <param name="lastIndexInSequenceOfRows"></param>
        /// <param name="indexOfCurrentRowInSequence"></param>
        private static void AddOnlySpacesRowsIfNecessary(int lastIndexInSequenceOfRows, int indexOfCurrentRowInSequence)
        {
            if (indexOfCurrentRowInSequence > lastIndexInSequenceOfRows)
            {
                var countOfOnlySpacesRowsBeforeNewRow = indexOfCurrentRowInSequence - (lastIndexInSequenceOfRows);
                // вставляем пустые строки до новой строки + новую строку
                InsertSpacesRowsToSplitRowsList(lastIndexInSequenceOfRows, countOfOnlySpacesRowsBeforeNewRow + 1);
            }
        }
        /// <summary>
        /// Вставка последовательности символов в строку. 
        /// Если индекс, по которому необходимо вставить элемент, 
        /// находится вне текущей строки -  добавляет в строку пробелы и только потом вставляет символ.
        /// </summary>
        /// <param name="currentRowStrBld"></param>
        /// <param name="indexInsert"></param>
        /// <param name="currentCol"></param>
        /// <param name="symbols"></param>
        private static void InsertSequenceOfSymbolsToStringBuilderWithCheck(StringBuilder currentRowStrBld, int indexInsert, int currentCol, string symbols)
        {
            var currentLastIndexInRow = currentRowStrBld.Length - 1;
            // если символ необходимо вставить внутри строки
            if (currentCol <= currentLastIndexInRow)
            {
                InsertSequenceOfSymbolsToStringBuilder(currentRowStrBld, indexInsert, symbols);
            }
            else
            {
                var distanceBetweenLastSymbolAndCurrentPosition = indexInsert - currentLastIndexInRow;
                for (int i = 0; i < distanceBetweenLastSymbolAndCurrentPosition; i++)
                {
                    currentRowStrBld.Append(' ');
                }
                InsertSequenceOfSymbolsToStringBuilder(currentRowStrBld, indexInsert, symbols);
            }
        }
        private static void ChangeBufferSizeIfNecessaryThenRenderRowsAndSetCursorPosition(string newRow, int rowIndexInRowsSequence, CursorPosition origignalCursorPosition)
        {
            if (ChangeBufferSizeIfNecessary(newRow))
            {
                RenderRows(0, _countOfRowsInSequenceOfRows, startCursorPosition.Top, startCursorPosition.Left, 0);
                Console.CursorTop = origignalCursorPosition.Top;
                Console.CursorLeft = origignalCursorPosition.Left + 1;
            }
            // иначе только одну
            else
            {
                RenderRows(rowIndexInRowsSequence, rowIndexInRowsSequence + 1, origignalCursorPosition.Top, origignalCursorPosition.Left, 1); // рендерим данную строку
            }
        }
        private static void InsertSequenceOfSymbolsToStringBuilderWithCheck(StringBuilder currentRowStrBld, int indexInsert, int currentCol, params char[] symbols)
        {
            var symbolsStr = String.Join("", symbols);
            InsertSequenceOfSymbolsToStringBuilderWithCheck(currentRowStrBld, indexInsert, currentCol, symbolsStr);
        }
        private static void InsertSequenceOfSymbolsToStringBuilder(StringBuilder sourceStrBld, int indexInsert, params char[] symbols)
        {
            var symbolsStr = String.Join("", symbols);
            InsertSequenceOfSymbolsToStringBuilder(sourceStrBld, indexInsert, symbolsStr);
        }
        private static void InsertSequenceOfSymbolsToStringBuilder(StringBuilder sourceStrBld, int indexInsert, string symbols)
        {
            sourceStrBld.Insert(indexInsert, symbols);
        }
    }
}
