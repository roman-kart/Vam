using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vam.Commands.Vam
{
    // Простите меня, дядюшка Мартин, я совершил один из смертных грехов - нарушил DRY
    public class ChangeCursorPositionVertical
    {
        /// <summary>
        /// Изменяет положение курсора.
        /// В качестве аргумента принимает смещение по вертикали (пока смещение по вертикали может быть только на 1 строку), 
        /// список строк и индекс текущей строки.
        /// </summary>
        /// <param name="rightDiff"></param>
        /// <param name="topDiff"></param>
        public static void Do(int verticalDiff, List<StringBuilder> sourceString, int currentRowIndex, int currentColumnIndex)
        {
            var currentRowStr = sourceString[currentRowIndex];
            var currentRowStrLength = currentRowStr.Length;

            var previousRowIndex = currentRowIndex - 1;
            var previousRowStr = previousRowIndex < 0 ? null : sourceString[previousRowIndex];
            var previousRowStrLength = previousRowStr == null ? -1 : previousRowStr.Length;

            var nextRowIndex = currentRowIndex + 1;
            var nextRowStr = nextRowIndex >= sourceString.Count ? null : sourceString[nextRowIndex];
            var nextRowStrLength = nextRowStr == null ? -1 : nextRowStr.Length;

            // если необходимо переместить курсор вверх
            if (verticalDiff < 0)
            {
                ToUp(verticalDiff, currentRowIndex, currentColumnIndex, currentRowStrLength, previousRowStrLength);
            }
            // если необходимо перенести курсор вниз
            else if (verticalDiff > 0)
            {
                ToDown(verticalDiff, currentRowIndex, currentColumnIndex, currentRowStrLength, nextRowStrLength, nextRowStrLength);
            }
            // если необходимо переместить курсор вправо
        }
        /// <summary>
        /// rightDiff необходимо задавать отрицательным числом.
        /// </summary>
        /// <param name="rightDiff"></param>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        /// <param name="currentRowLength"></param>
        /// <param name="previousRowLength"></param>
        private static void ToUp(int rightDiff, int rowIndex, int columnIndex, int currentRowLength, int previousRowLength)
        {
            // если курсор находится на первой строке
            if (rowIndex == 0)
            {
                // ничего не делаем
                // в будущем можно будет добавить звуковой сигнал
            }
            // если текущая позиция курсора больше, чем длина предыдущей строки
            else if (columnIndex > previousRowLength)
            {
                // переносим курсор на последний символ предыдущей строки
                Console.CursorTop -= 1; // перемещаемся курсор на верхнюю строку
                Console.CursorLeft = previousRowLength; // перемещаем курсор на последний символ предыдущей строки
            }
            // если можно просто переместить курсор на предыдущую строку
            else
            {
                Console.CursorTop -= 1; // перемещаем курсор на предыдущую строку
            }
        }
        /// <summary>
        /// rightDiff необходимо задавать положительным числом.
        /// </summary>
        /// <param name="rightDiff"></param>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        /// <param name="currentRowLength"></param>
        private static void ToDown(int rightDiff, int rowIndex, int columnIndex, int currentRowLength, int nextRowLength, int lastRowIndex)
        {
            // если курсор находится на последней строке файла
            if (rowIndex == lastRowIndex)
            {
                // ничего не делаем
                // в будущем можно будет добавить звуковой сигнал
            }
            // если текущая позиция индекса больше, чем длина следующей строки
            else if (columnIndex > nextRowLength)
            {
                // переносим курсор на последний символ предыдущей строки
                Console.CursorTop += 1; // перемещаемся курсор на верхнюю строку
                Console.CursorLeft = nextRowLength; // перемещаем курсор на последний символ предыдущей строки
            }
            // если можно просто переместить курсор на предыдущую строку
            else
            {
                Console.CursorTop += 1; // перемещаем курсор на предыдущую строку
            }
        }
    }
}
