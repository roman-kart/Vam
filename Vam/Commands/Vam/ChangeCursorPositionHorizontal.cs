using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vam.Commands.Vam
{
    public static class ChangeCursorPositionHorizontal
    {
        /// <summary>
        /// Изменяет положение курсора.
        /// В качестве аргумента принимает смещение по горизонтали и вертикали, 
        /// список строк и индекс текущей строки.
        /// </summary>
        /// <param name="leftDiff"></param>
        /// <param name="topDiff"></param>
        public static void Do(int horizontalDiff, List<StringBuilder> sourceString, int currentRowIndex, int currentColumnIndex)
        {
            var currentRowStr = sourceString[currentRowIndex];
            var currentRowStrLength = currentRowStr.Length;

            var previousRowIndex = currentRowIndex - 1;
            var previousRowStr = previousRowIndex < 0 ? null : sourceString[previousRowIndex];
            var previousRowStrLength = previousRowStr == null ? -1 : previousRowStr.Length;

            var lastRowIndex = sourceString.Count - 1;

            // если необходимо переместить курсор влево
            if (horizontalDiff < 0)
            {
                ToLeft(horizontalDiff, currentRowIndex, currentColumnIndex, currentRowStrLength, previousRowStrLength);
            }
            else if (horizontalDiff > 0)
            {
                ToRight(horizontalDiff, currentRowIndex, currentColumnIndex, currentRowStrLength, lastRowIndex);
            }
            // если необходимо переместить курсор вправо
        }
        /// <summary>
        /// leftDiff необходимо задавать отрицательным числом.
        /// </summary>
        /// <param name="leftDiff"></param>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        /// <param name="currentRowLength"></param>
        /// <param name="previousRowLength"></param>
        private static void ToLeft(int leftDiff, int rowIndex, int columnIndex, int currentRowLength, int previousRowLength)
        {
            var columnIndexAfterMoving = columnIndex + leftDiff;
            // если курсор находится на первой строке
            if (rowIndex == 0)
            {
                // если курсор находится в верхнем левом углу
                if (columnIndex == 0)
                {
                    // пока действий для данной ситуации нет,
                    // но в последствии можно добавить звуковой сигнал
                }
                // если после перемещения индекс окажется левее левой границы буфера
                else if (columnIndexAfterMoving < 0)
                {
                    // перемещаем курсор на левую границу текущей строки
                    Console.CursorLeft = 0;
                }
                // если мы можем просто переместить курсор влево
                else
                {
                    Console.CursorLeft += leftDiff; // перемещаем курсор влево
                }
            }
            // если курсор находится в самой левой позиции на текущей строке,
            // но текущая строка не является первой
            else if (columnIndex == 0)
            {
                // переносим курсор на последний символ предыдущей строки
                Console.CursorTop -= 1; // перемещаемся курсор на верхнюю строку
                Console.CursorLeft = previousRowLength; // перемещаем курсор на последний символ предыдущей строки
            }
            // если курсор находится не на первой строке, а также курсор не находится на первом символе строки,
            // но после перемещения курсор выйдет за границы буфера
            else if (columnIndexAfterMoving < 0)
            {
                Console.CursorLeft = 0; // перемещаем курсор на самый первый символ текущей строки
            }
            // если можно просто переместить курсор влево
            else
            {
                Console.CursorLeft += leftDiff; // перемещаем курсор влево
            }
        }
        /// <summary>
        /// rightDiff необходимо задавать положительным числом.
        /// </summary>
        /// <param name="rightDiff"></param>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        /// <param name="currentRowLength"></param>
        private static void ToRight(int rightDiff, int rowIndex, int columnIndex, int currentRowLength, int lastRowIndex)
        {
            var columnIndexAfterMoving = columnIndex + rightDiff;
            // если курсор находится на последней строке файла
            if (rowIndex == lastRowIndex)
            {
                // и если курсор находится на последнем символе последней строки
                if (columnIndex == currentRowLength)
                {
                    // пока ничего не делаем
                    // впоследствии можно будет добавить звуковой сигнал
                }
                // если после перемещения курсор выйдет за границы текущий строки
                else if (columnIndexAfterMoving > currentRowLength)
                {
                    Console.CursorLeft = currentRowLength; // перемещаем курсор на последний символ текущей строки
                }
                // если мы можем просто переместить курсор вправо
                else
                {
                    Console.CursorLeft += rightDiff; // перемещаем курсор вправо
                }
            }
            // если курсор находится на последнем символе и текущая строка не является последней
            else if (columnIndex == currentRowLength)
            {
                Console.CursorTop += 1; // перемещаем курсор на следующую строку
                Console.CursorLeft = 0; // ставим курсор на первый символ следующей строки
            }
            // если после перемещения курсор выйдет за границы текущий строки
            else if (columnIndexAfterMoving > currentRowLength)
            {
                Console.CursorLeft = currentRowLength; // перемещаем курсор на последний символ текущей строки
            }
            // если можно просто переместить курсор вправо
            else
            {
                Console.CursorLeft += rightDiff;
            }
        }
    }
}
