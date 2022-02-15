using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vam.Commands.Vam
{
    public class WorkWithFileContent
    {
        /// <summary>
        /// Определяет размер наибольшей строки в массиве строк.
        /// Если массив пуст - возвращает 0.
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static int LengthOfLongestString(string[] rows)
        {
            if (rows.Length == 0)
            {
                return 0;
            }
            int maxLength = rows[0].Length;
            foreach (var row in rows)
            {
                if (maxLength < row.Length)
                {
                    maxLength = row.Length;
                }
            }
            return maxLength;
        }
    }
}
